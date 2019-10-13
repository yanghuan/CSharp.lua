using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace CSharpLua {
  [Obsolete]
  internal static class NuGetHelper {
    private static readonly string _globalPackagesPath;
    private static readonly string _runtimeFramework;
    private static readonly string _targetFramework;

    static NuGetHelper() {
      // 'dotnet nuget locals global-packages --list'
      // SettingsUtility.GetGlobalPackagesFolder(Settings.LoadMachineWideSettings(string.Empty));
      _globalPackagesPath = Path.Combine(NuGetEnvironment.GetFolderPath(NuGetFolderPath.NuGetHome), SettingsUtility.DefaultGlobalPackagesFolderPath);
      _runtimeFramework = Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName;
      _targetFramework = RuntimeInformation.FrameworkDescription;
    }

    internal static IEnumerable<NuGetVersion> GetAvailableVersions(string packageName) {
      var packagePath = Path.Combine(_globalPackagesPath, packageName);
      if (!Directory.Exists(packagePath)) {
        // TODO: try to restore package
        yield break;
      }

      var pathLength = packagePath.Length + 1;
      foreach (var packageVersionPath in Directory.EnumerateDirectories(packagePath)) {
        if (NuGetVersion.TryParse(packageVersionPath.Substring(pathLength), out var nuGetVersion)) {
          yield return nuGetVersion;
        }
      }
    }

    internal static T GetNearest<T>(IEnumerable<T> items) where T : IFrameworkSpecific {
      return NuGetFrameworkUtility.GetNearest(items, NuGetFramework.Parse(_targetFramework));
    }

    internal static IFrameworkSpecific GetNearest(IEnumerable<string> items) {
      return NuGetFrameworkUtility.GetNearest(items.Select(item => (IFrameworkSpecific)NuGetFramework.Parse(item)), NuGetFramework.Parse(_targetFramework));
    }

    internal static string GetPath(string name, ref string version) {
      return GetPath(name, VersionRange.Parse(version), out version);
    }

    internal static string GetPath(string name, VersionRange versionRange, out string version) {
      // var nuGetVersion = versionRange.FindBestMatch(GetAvailableVersions(name));
      var nuGetVersion = (NuGetVersion)null;
      foreach (var availableVersion in GetAvailableVersions(name)) {
        if (versionRange.Satisfies(availableVersion)) {
          if (nuGetVersion is null || availableVersion > nuGetVersion) {
            nuGetVersion = availableVersion;
          }
        }
      }

      if (nuGetVersion is null) {
        version = null;
        return null;
      }

      version = nuGetVersion.ToNormalizedString();
      return Path.Combine(_globalPackagesPath, name, version);
    }

    internal static IEnumerable<(string PackageName, VersionRange PackageVersion)> GetNuspecDependencies(string nuspecFilePath) {
      if (!NuGet.Packaging.PackageHelper.IsNuspec(nuspecFilePath)) {
        throw new Exception();
      }

      using var fileStream = File.OpenRead(nuspecFilePath);
      var reader = new NuspecReader(fileStream);
      // var currentFramework = new NuGetFramework(FrameworkConstants.FrameworkIdentifiers.NetCore, FrameworkConstants.FrameworkIdentifiers.
      // var targetFramework = NuGetFramework.Parse(_targetFramework);
      // var dependencyGroup = NuGetFrameworkUtility.GetNearest(reader.GetDependencyGroups(), targetFramework);
      // var dependencyGroup = GetNearest(reader.GetDependencyGroups());
      var dependencyGroup = reader.GetDependencyGroups().Single();

      return dependencyGroup.Packages.Select(package => (package.Id, package.VersionRange));
    }

    [Obsolete]
    private static void ListGlobalPackages() {
      var processStartInfo = new ProcessStartInfo("dotnet", "nuget locals global-packages --list");
      processStartInfo.RedirectStandardOutput = true;

      var process = Process.Start(processStartInfo);
      process.WaitForExit();

      using var output = process.StandardOutput;
      while (!output.EndOfStream) {
        var line = output.ReadLine();
      }
    }
  }
}
