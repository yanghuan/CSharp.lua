using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace CSharp.lua {
  public sealed class PackageException : Exception {
    public PackageException() {
    }
    public PackageException(string message) : base(message) {
    }
  }

  internal sealed class VersionStatus {
    public VersionRange Allowed { get; private set; }
    public NuGetVersion Selected { get; private set; }
    public VersionStatus(VersionRange versionRange) {
      Allowed = versionRange;
    }
    public bool SelectBestMatch(string id) {
      if (Selected is null) {
        Selected = Allowed.FindBestMatch(PackageHelper.GetAvailableVersions(id));
        return true;
      } else {
        return false;
      }
    }
    public void UpdateAllowedRange(VersionRange range) {
      Allowed = VersionRange.Combine(new[] { Allowed, range });
    }
  }

  internal static class PackageHelper {
    private static readonly string _globalPackagesPath = Path.Combine(NuGetEnvironment.GetFolderPath(NuGetFolderPath.NuGetHome), SettingsUtility.DefaultGlobalPackagesFolderPath);

    internal static IEnumerable<NuGetVersion> GetAvailableVersions(string packageName) {
      var packagePath = Path.Combine(_globalPackagesPath, packageName);
      // TODO: run package restore (always or only when directory doesn't exist?)
      if (!Directory.Exists(packagePath)) {
        yield break;
      }
      foreach (var packageVersionPath in Directory.EnumerateDirectories(packagePath)) {
        yield return NuGetVersion.Parse(new DirectoryInfo(packageVersionPath).Name);
      }
    }

    public static IEnumerable<string> EnumeratePackages(string targetFrameworkVersion, IEnumerable<Cake.Incubator.Project.CustomProjectParserResult> projects) {
      // TODO: check if this cast is correct
      var targetFramework = NuGetFramework.ParseFrameworkName(targetFrameworkVersion, DefaultFrameworkNameProvider.Instance);
      var packages = new Dictionary<string, VersionStatus>();
      void AddPackageReference(string id, VersionRange versionRange) {
        if (packages.TryGetValue(id, out var versionStatus)) {
          if (versionStatus.Selected is null) {
            versionStatus.UpdateAllowedRange(versionRange);
          } else if (!versionRange.Satisfies(versionStatus.Selected)) {
            throw new PackageException($"Incompatible package dependency for package {id} {versionStatus.Selected}: {versionRange}");
          }
        } else {
          packages.Add(id, new VersionStatus(versionRange));
        }
      }
      foreach (var package in projects.SelectMany(project => project.PackageReferences)) {
        AddPackageReference(package.Name, VersionRange.Parse(package.Version));
      }
      var newDependencies = new HashSet<PackageIdentity>();
      while (true) {
        foreach (var package in packages) {
          if (package.Value.SelectBestMatch(package.Key)) {
            newDependencies.Add(new PackageIdentity(package.Key, package.Value.Selected));
          }
        }
        if (newDependencies.Count == 0) {
          break;
        }
        foreach (var package in newDependencies.SelectMany(package => GetDependencies(package, targetFramework))) {
          AddPackageReference(package.Id, package.VersionRange);
        }
        newDependencies.Clear();
      }
      foreach (var package in packages) {
        yield return Path.Combine(_globalPackagesPath, package.Key, package.Value.Selected.ToNormalizedString());
      }
    }

    private static IEnumerable<PackageDependency> GetDependencies(PackageIdentity package, NuGetFramework targetFramework) {
      var nuspecFile = Path.Combine(_globalPackagesPath, package.Id, package.Version.ToNormalizedString(), $"{package.Id}.nuspec");
      if (!NuGet.Packaging.PackageHelper.IsNuspec(nuspecFile)) {
        throw new PackageException($"Could not locate the .nuspec file for package: {package}");
      }
      var reader = new NuspecReader(nuspecFile);
      var compatibleGroups = reader.GetDependencyGroups().Where(dependencyGroup => NuGetFrameworkUtility.IsCompatibleWithFallbackCheck(targetFramework, dependencyGroup.TargetFramework));
      // TODO: how to select best match dependencyGroup?
      return compatibleGroups.First().Packages;
    }
  }
}
