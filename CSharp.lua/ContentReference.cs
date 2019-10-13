using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Incubator.Project;

namespace CSharpLua {
  [Obsolete]
  public abstract class ContentReference : IEquatable<ContentReference> {
    public const string LibraryFolder = @"lib";
    public const string SourceCodeFolder = @"contentFiles\cs";
    public const string ContentFolder = @"content";

    public abstract string Folder { get; }

    public IEnumerable<string> EnumerateFiles(string searchPattern, SearchOption searchOption, string? searchSubFolder, params string[] ignoredSubFolders) {
      var searchPath = string.IsNullOrWhiteSpace(searchSubFolder) ? Folder : Path.Combine(Folder, searchSubFolder);
      if (!Directory.Exists(searchPath)) {
        yield break;
      }
      if (searchOption == SearchOption.TopDirectoryOnly || ignoredSubFolders.Length == 0) {
        foreach (var file in Directory.EnumerateFiles(searchPath, searchPattern, searchOption)) {
          yield return file;
        }
      } else {
        foreach (var file in Directory.EnumerateFiles(searchPath, searchPattern, SearchOption.TopDirectoryOnly)) {
          yield return file;
        }

        foreach (var subFolder in Directory.EnumerateDirectories(searchPath, "*", SearchOption.TopDirectoryOnly)) {
          var subFolderName = new DirectoryInfo(subFolder).Name;
          if (ignoredSubFolders.Contains(subFolderName)) {
            continue;
          }
          foreach (var file in Directory.EnumerateFiles(subFolder, searchPattern, SearchOption.AllDirectories)) {
            yield return file;
          }
        }
      }
    }

    internal virtual IEnumerable<string> EnumerateLibraries() {
      yield break;
    }

    internal virtual IEnumerable<string> EnumerateSourceFiles() {
      return EnumerateFiles("*.cs", SearchOption.AllDirectories, null, "bin", "obj");
    }

    public abstract IEnumerable<ContentReference> GetReferences(bool searchRecursively);

    protected IEnumerable<ContentReference> GetReferences(ContentReference reference, bool searchRecursively) {
      if (Folder is null) {
        yield break;
      }

      yield return reference;
      if (searchRecursively) {
        foreach (var recursiveReference in reference.GetReferences(true)) {
          yield return recursiveReference;
        }
      }
    }

    bool IEquatable<ContentReference>.Equals(ContentReference other) {
      return GetType() == other.GetType() && Folder == other.Folder;
    }
  }

  public sealed class FolderReference : ContentReference {
    private readonly string _path;

    public FolderReference(string path) {
      _path = path;
    }

    public override string Folder => _path;

    public override IEnumerable<ContentReference> GetReferences(bool searchRecursively) {
      yield break;
    }
  }

  public sealed class ProjectReference : ContentReference {
    private static readonly ICakeEnvironment _environment;
    private static readonly Cake.Core.IO.IFileSystem _fileSystem;

    private readonly CustomProjectParserResult _project;
    private readonly string _folder;

    private readonly string _configuration;
    private readonly string _platform;
    private readonly string _target;

    static ProjectReference() {
      var platform = new CakePlatform();
      var runtime = new CakeRuntime();
      var cakeConsole = new CakeConsole();
      var cakeLog = new CakeBuildLog(cakeConsole, Verbosity.Diagnostic);

      _environment = new CakeEnvironment(platform, runtime, cakeLog);
      _fileSystem = new Cake.Core.IO.FileSystem();
    }

    public ProjectReference(string path, string configuration = "Release", string platform = "AnyCPU")
      : this(new Cake.Core.IO.FilePath(path.EndsWith(".csproj") ? path : Directory.EnumerateFiles(path, "*.csproj", SearchOption.TopDirectoryOnly).Single()),
          Environment.CurrentDirectory,
          null,
          configuration,
          platform) {
    }

    internal ProjectReference(Cake.Core.IO.FilePath path, string workingDirectory, string targetFramework, string configuration, string platform = "AnyCPU") {
      if (path.IsRelative) {
        _environment.WorkingDirectory = workingDirectory;
        path = path.MakeAbsolute(_environment);
      }

      _project = _fileSystem.GetFile(path).ParseProjectFile(_configuration = configuration, _platform = platform);
      _folder = _project.ProjectFilePath.GetDirectory().FullPath;

      _target = targetFramework ?? _project.TargetFrameworkVersions.Single();
    }

    public override string Folder => _folder;

    public override IEnumerable<ContentReference> GetReferences(bool searchRecursively) {
      foreach (var projectReference in _project.ProjectReferences) {
        foreach (var reference in GetReferences(GetReferencedProject(projectReference), searchRecursively)) {
          yield return reference;
        }
      }

      foreach (var packageReference in _project.PackageReferences) {
        foreach (var reference in GetReferences(new PackageReference(packageReference.Name, packageReference.Version), searchRecursively)) {
          yield return reference;
        }
      }
    }

    private ProjectReference GetReferencedProject(Cake.Common.Solution.Project.ProjectReference projectReference) {
      return new ProjectReference(projectReference.FilePath, Folder, _target, _configuration, _platform);
    }
  }

  public sealed class PackageReference : ContentReference {
    private readonly string _path;
    private readonly string _name;
    private readonly string _version;

    internal PackageReference(string name, string version) {
      _path = NuGetHelper.GetPath(name, ref version);
      _name = name;
      _version = version;
    }

    internal PackageReference(string name, NuGet.Versioning.VersionRange versionRange) {
      _path = NuGetHelper.GetPath(name, versionRange, out _version);
      _name = name;
    }

    public override string Folder => _path;

    public string Name => _name;

    public string Version => _version;

    public override IEnumerable<ContentReference> GetReferences(bool searchRecursively) {
      if (_path is null) {
        yield break;
      }

      foreach (var (PackageName, PackageVersion) in NuGetHelper.GetNuspecDependencies(Path.Combine(_path, $"{_name}.nuspec"))) {
        foreach (var reference in GetReferences(new PackageReference(PackageName, PackageVersion), searchRecursively)) {
          yield return reference;
        }
      }
    }

    internal override IEnumerable<string> EnumerateLibraries() {
      return EnumerateFiles("*.dll", SearchOption.AllDirectories, LibraryFolder);
    }

    internal override IEnumerable<string> EnumerateSourceFiles() {
      var sourceFolder = Path.Combine(Folder, SourceCodeFolder);
      if (!Directory.Exists(sourceFolder)) {
        return Array.Empty<string>();
      }
      // var bestMatchFramework = NuGetHelper.GetNearest(Directory.EnumerateDirectories(sourceFolder));
      var bestMatchFramework = Directory.EnumerateDirectories(sourceFolder).Single();
      return EnumerateFiles("*.cs", SearchOption.AllDirectories, Path.Combine(sourceFolder, bestMatchFramework.ToString()));
    }
  }
}
