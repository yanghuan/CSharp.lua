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

namespace CSharpLua {
  public sealed class PackageException : Exception {
    public PackageException() {
    }
    public PackageException(string message) : base(message) {
    }
  }

  internal sealed class VersionStatus {
    private bool triedSelect;
    public VersionRange Allowed { get; private set; }
    public NuGetVersion Selected { get; private set; }
    public NuGetFramework Framework { get; set; }
    public VersionStatus(VersionRange versionRange) {
      triedSelect = false;
      Allowed = versionRange;
    }
    public bool SelectBestMatch(string id) {
      if (!triedSelect) {
        triedSelect = true;
        Selected = Allowed.FindBestMatch(PackageHelper.GetAvailableVersions(id));
        return Selected != null;
      } else {
        return false;
      }
    }
    public void UpdateAllowedRange(VersionRange range) {
      Allowed = VersionRange.Combine(new[] { Allowed, range });
    }
    public override string ToString() {
      return Selected?.ToString() ?? Allowed.ToString();
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

    public static IEnumerable<string> EnumerateLibs(string packagePath, string frameworkFolderName) {
      const string SearchPattern = "*.dll";
      var libPath = Path.Combine(packagePath, "lib");
      if (!Directory.Exists(libPath)) {
        return Array.Empty<string>();
      }
      var frameworkPath = Path.Combine(libPath, frameworkFolderName ?? Directory.EnumerateDirectories(libPath).Single());
      if (!Directory.Exists(frameworkPath)) {
        var targetFramework = NuGetFramework.Parse(frameworkFolderName);
        var compatibleFrameworks = Directory.EnumerateDirectories(libPath)
          .Where(folder => NuGetFrameworkUtility.IsCompatibleWithFallbackCheck(targetFramework, NuGetFramework.ParseFolder(new DirectoryInfo(folder).Name)));
        // TODO: how to select best match framework?
        frameworkPath = compatibleFrameworks.FirstOrDefault();
        if (frameworkPath is null) {
          // Ignore folders that contain no .dll files (these usually have a file called "_._" in it).
          compatibleFrameworks = Directory.EnumerateDirectories(libPath)
          .Where(folder => NuGetFrameworkUtility.IsCompatibleWithFallbackCheck(NuGetFramework.ParseFolder(new DirectoryInfo(folder).Name), targetFramework))
          .Where(folder => Directory.EnumerateFiles(folder, SearchPattern, SearchOption.AllDirectories).FirstOrDefault() != null);
          // TODO: how to select best match framework?
          frameworkPath = compatibleFrameworks.First();
        }
      }
      return Directory.EnumerateFiles(frameworkPath, SearchPattern, SearchOption.TopDirectoryOnly);
    }

    public static IEnumerable<(string folder, string frameworkFolderName)> EnumeratePackages(string targetFrameworkVersion, IEnumerable<Cake.Incubator.Project.CustomProjectParserResult> projects) {
      var targetFramework = NuGetFramework.Parse(targetFrameworkVersion);
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
        foreach (var newDependency in newDependencies) {
          var dependencyGroup = GetDependencyGroup(newDependency, targetFramework);
          if (dependencyGroup != null) {
            packages[newDependency.Id].Framework = dependencyGroup.TargetFramework;
            foreach (var package in dependencyGroup.Packages) {
              AddPackageReference(package.Id, package.VersionRange);
            }
          }
        }
        newDependencies.Clear();
      }
      foreach (var package in packages) {
        if (package.Value.Selected != null) {
          yield return (Path.Combine(_globalPackagesPath, package.Key, package.Value.Selected.ToNormalizedString()), package.Value.Framework?.GetShortFolderName());
        }
      }
    }

    private static PackageDependencyGroup GetDependencyGroup(PackageIdentity package, NuGetFramework targetFramework) {
      var nuspecFile = Path.Combine(_globalPackagesPath, package.Id, package.Version.ToNormalizedString(), $"{package.Id}.nuspec");
      if (!NuGet.Packaging.PackageHelper.IsNuspec(nuspecFile)) {
        throw new PackageException($"Could not locate the .nuspec file for package: {package}");
      }
      var reader = new NuspecReader(nuspecFile);
      var dependencyGroups = reader.GetDependencyGroups();
      if (dependencyGroups.FirstOrDefault() != null) {
        var compatibleGroups = dependencyGroups.Where(dependencyGroup => NuGetFrameworkUtility.IsCompatibleWithFallbackCheck(targetFramework, dependencyGroup.TargetFramework));
        // TODO: how to select best match framework?
        return compatibleGroups.First();
      }
      return null;
    }
  }
}
