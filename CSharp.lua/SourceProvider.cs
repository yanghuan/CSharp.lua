using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Incubator.Project;

namespace CSharpLua {
  [Obsolete]
  internal static class SourceProvider {
    internal static IEnumerable<string> GetCSharpFilesFromFolder(string folder) {
      return new FolderReference(folder).EnumerateFiles("*.cs", SearchOption.AllDirectories, null, "bin", "obj");
    }

    internal static IEnumerable<string> GetLibsFromFolder(string folder) {
      return new FolderReference(folder).EnumerateFiles("*.dll", SearchOption.AllDirectories, "lib");
    }

    internal static IEnumerable<string> GetCSharpFilesFromProject(string csProjFolderPath) {
      var csProjFilePath = Directory.EnumerateFiles( csProjFolderPath, "*.csproj", SearchOption.TopDirectoryOnly ).SingleOrDefault();
      if (csProjFilePath is null) {
        throw new FileNotFoundException();
      }

      var fileSystem = new Cake.Core.IO.FileSystem();
      var platform = new CakePlatform();
      var runtime = new CakeRuntime();
      var cakeConsole = new CakeConsole();
      var cakeLog = new CakeBuildLog(cakeConsole, Verbosity.Diagnostic);
      var environment = new CakeEnvironment(platform, runtime, cakeLog);

      var projectPath = new Cake.Core.IO.FilePath(csProjFilePath);
      if (projectPath.IsRelative) {
        projectPath = projectPath.MakeAbsolute(environment);
      }
      var projectFile = fileSystem.GetFile(projectPath);
      var parseResult = projectFile.ParseProjectFile("Debug");

      // TODO: do not assume all .cs files are included by default?
      // TODO: do not return .cs files that are removed from Compile in the .csproj
      foreach (var sourceFile in GetCSharpFilesFromFolder(csProjFolderPath)) {
        yield return sourceFile;
      }

      // TODO: if a project or package is encountered multiple times through recursion, ensure it only gets evaluated once (use HashSet?)
      // TODO: handle the case where a library is referenced as both a projectReference and a packageReference (NuGet gives error when this happens)

      foreach (var projectReference in parseResult.ProjectReferences) {
        // TODO: test that this works correctly
        foreach (var sourceFile in GetCSharpFilesFromProject(projectReference.FilePath.FullPath)) {
          yield return sourceFile;
        }
      }

      // Assume package reference is either a .Sources package, or it's not and has no .cs files packed.
      foreach (var packageReference in parseResult.PackageReferences) {
        // TODO: try to get source files from package's dependencies (recursively)
        foreach (var sourceFile in DiscoverPackageReferenceSourceFiles(packageReference.Name, packageReference.Version)) {
          yield return sourceFile;
        }
      }
    }

    private static IEnumerable<string> DiscoverPackageReferenceSourceFiles(string name, string version) {
      // TODO: use 'dotnet nuget locals global-packages --list' to find global package locations, instead of hardcoded default path

      var globalPackageDirectory = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), @".nuget\packages");

      // TODO: handle case where package has multiple target frameworks
      var sourceFilesDirectory = Path.Combine(globalPackageDirectory, name, version/*, @"contentFiles\cs\netstandard1.0"*/);

      return Directory.EnumerateFiles(sourceFilesDirectory, "*.cs", SearchOption.AllDirectories);
    }
  }
}
