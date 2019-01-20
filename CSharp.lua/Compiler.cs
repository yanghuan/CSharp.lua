/*
Copyright 2017 YANG Huan (sy.yanghuan@gmail.com).

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

  http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CSharpLua {
  public sealed class Compiler {
    private const string kDllSuffix = ".dll";
    private const string kSystemMeta = "~/System.xml";

    private readonly string folder_;
    private readonly string output_;
    private readonly string[] libs_;
    private readonly string[] metas_;
    private readonly string[] cscArguments_;
    private readonly bool isNewest_;
    private readonly string[] attributes_;
    public bool IsExportReflectionFile { get; set; }
    public bool IsExportMetadata { get; set; }

    public Compiler(string folder, string output, string lib, string meta, string csc, bool isClassic, string atts) {
      folder_ = folder;
      output_ = output;
      libs_ = Utility.Split(lib);
      metas_ = Utility.Split(meta);
      cscArguments_ = string.IsNullOrEmpty(csc) ? Array.Empty<string>() : csc.Trim().Split(' ', '\t');
      isNewest_ = !isClassic;
      if (atts != null) {
        attributes_ = Utility.Split(atts, false);
      }
    }

    private static IEnumerable<string> GetMetas(IEnumerable<string> additionalMetas) {
      List<string> metas = new List<string>() { Utility.GetCurrentDirectory(kSystemMeta) };
      if (additionalMetas != null) {
        metas.AddRange(additionalMetas);
      }
      return metas;
    }

    private IEnumerable<string> Metas => GetMetas(metas_);

    private static bool IsCorrectSystemDll(string path) {
      try {
        Assembly.LoadFile(path);
        return true;
      } catch (Exception) {
        return false;
      }
    }

    private static IEnumerable<string> GetLibs(IEnumerable<string> additionalLibs) {
      string privateCorePath = typeof(object).Assembly.Location;
      List<string> libs = new List<string>() { privateCorePath };

      string systemDir = Path.GetDirectoryName(privateCorePath);
      foreach (string path in Directory.EnumerateFiles(systemDir, "*.dll")) {
        if (IsCorrectSystemDll(path)) {
          libs.Add(path);
        }
      }

      if (additionalLibs != null) {
        foreach (string lib in additionalLibs) {
          string path = lib.EndsWith(kDllSuffix) ? lib : lib + kDllSuffix;
          if (File.Exists(path)) {
            libs.Add(path);
          } else {
            string file = Path.Combine(systemDir, Path.GetFileName(path));
            if (!File.Exists(file)) {
              throw new CmdArgumentException($"-l {path} is not found");
            }
          }
        }
      }

      return libs;
    }

    private IEnumerable<string> Libs => GetLibs(libs_);

    private static LuaSyntaxGenerator Build(
      IEnumerable<string> cscArguments,
      IEnumerable<(string Text, string Path)> codes, 
      IEnumerable<string> libs,
      IEnumerable<string> metas,
      bool isNewest,
      string[] attributes,
      string folder,
      bool isExportReflectionFile = false,
      bool IsExportMetadata = false
      ) {
      var commandLineArguments = CSharpCommandLineParser.Default.Parse((cscArguments ?? Array.Empty<string>()).Concat(new string[] { "-define:__CSharpLua__" }), null, null);
      var parseOptions = commandLineArguments.ParseOptions.WithLanguageVersion(LanguageVersion.Latest).WithDocumentationMode(DocumentationMode.Parse);
      var syntaxTrees = codes.Select(code => CSharpSyntaxTree.ParseText(code.Text, parseOptions, code.Path));
      var references = libs.Select(i => MetadataReference.CreateFromFile(i));
      var setting = new LuaSyntaxGenerator.SettingInfo() {
        IsNewest = isNewest,
        HasSemicolon = false,
        IsExportReflectionFile = isExportReflectionFile,
        IsExportMetadata = IsExportMetadata
      };
      return new LuaSyntaxGenerator(syntaxTrees, references, commandLineArguments.CompilationOptions, metas, setting, attributes, folder);
    }

    public void Compile() {
      var files = Directory.EnumerateFiles(folder_, "*.cs", SearchOption.AllDirectories);
      var codes = files.Select(i => (File.ReadAllText(i), i));
      var generator = Build(cscArguments_, codes, Libs, Metas, isNewest_, attributes_, folder_, IsExportReflectionFile, IsExportMetadata);
      generator.Generate(output_);
    }

    public static string CompileSingleCode(string code) {
      var codes = new (string, string)[] { (code, "") };
      var generator = Build(null, codes, GetLibs(null), GetMetas(null), true, null, "");
      return generator.GenerateSingle();
    }
  }
}
