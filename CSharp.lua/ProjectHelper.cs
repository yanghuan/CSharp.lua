using System;
using System.Collections.Generic;
using System.Linq;

using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Incubator.Project;

namespace CSharpLua {
  internal static class ProjectHelper {
    private static readonly ICakeEnvironment _environment;
    private static readonly IFileSystem _fileSystem;

    static ProjectHelper() {
      var platform = new CakePlatform();
      var runtime = new CakeRuntime();
      var cakeConsole = new CakeConsole();
      var cakeLog = new CakeBuildLog(cakeConsole, Verbosity.Quiet);

      _environment = new CakeEnvironment(platform, runtime, cakeLog);
      _fileSystem = new FileSystem();
    }

    public static CustomProjectParserResult ParseProject(string path, string configuration, string platform = "AnyCPU") {
      return ParseProject(new FilePath(path), Environment.CurrentDirectory, configuration, platform);
    }

    private static CustomProjectParserResult ParseProject(FilePath path, string workingDirectory, string configuration, string platform) {
      if (path.IsRelative) {
        _environment.WorkingDirectory = workingDirectory;
        path = path.MakeAbsolute(_environment);
      }

      return _fileSystem.GetFile(path).ParseProjectFile(configuration, platform);
    }

    public static IEnumerable<(string folder, CustomProjectParserResult project)> EnumerateProjects(this CustomProjectParserResult mainProject) {
      var result = new Dictionary<string, CustomProjectParserResult>();
      void AddReferences(string folderPath, CustomProjectParserResult project) {
        foreach (var referencedProject in project.ProjectReferences.Select(p => ParseProject(p.FilePath, folderPath, project.Configuration, project.Platform))) {
          var directory = referencedProject.GetDirectory();
          if (result.TryAdd(directory, referencedProject)) {
            AddReferences(directory, referencedProject);
          } else if (referencedProject.ProjectFilePath != result[directory].ProjectFilePath) {
            throw new Exception("Found two or more projects in the same directory.");
          }
        }
      }
      var mainDirectory = mainProject.GetDirectory();
      result.Add(mainDirectory, mainProject);
      AddReferences(mainDirectory, mainProject);
      foreach (var pair in result) {
        yield return (pair.Key, pair.Value);
      }
    }

    public static string GetDirectory(this CustomProjectParserResult project) {
      return project.ProjectFilePath.GetDirectory().FullPath;
    }

    public static IEnumerable<string> EnumerateSourceFiles(this CustomProjectParserResult project, string folder = null) {
      folder ??= project.GetDirectory();
      const string ObjFolder = "obj";
      const string OutputFolder = "bin";
      const string SearchPattern = "*.cs";
      foreach (var file in System.IO.Directory.EnumerateFiles(folder, SearchPattern, System.IO.SearchOption.TopDirectoryOnly)) {
        yield return file;
      }
      var ignoredSubFolders = new HashSet<string>() { ObjFolder, OutputFolder };
      foreach (var subFolder in System.IO.Directory.EnumerateDirectories(folder)) {
        var subFolderName = new System.IO.DirectoryInfo(subFolder).Name;
        if (ignoredSubFolders.Contains(subFolderName)) {
          continue;
        }
        foreach (var file in System.IO.Directory.EnumerateFiles(subFolder, SearchPattern, System.IO.SearchOption.AllDirectories)) {
          yield return file;
        }
      }
    }
  }
}
