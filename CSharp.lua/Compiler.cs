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
    private readonly int indent_;
    private readonly string[] attributes_;

    public Compiler(string folder, string output, string lib, string meta, string csc, bool isClassic, string indent, string atts) {
      folder_ = folder;
      output_ = output;
      libs_ = Utility.Split(lib);
      metas_ = Utility.Split(meta);
      cscArguments_ = string.IsNullOrEmpty(csc) ? Array.Empty<string>() : csc.Trim().Split(' ', '\t');
      isNewest_ = !isClassic;
      int.TryParse(indent, out indent_);
      if (atts != null) {
        attributes_ = Utility.Split(atts, false);
      }
    }

    private IEnumerable<string> Metas {
      get {
        List<string> metas = new List<string>() { Utility.GetCurrentDirectory(kSystemMeta) };
        metas.AddRange(metas_);
        return metas;
      }
    }

    private bool IsCorrectSystemDll(string path) {
      try {
        Assembly.LoadFile(path);
        return true;
      } catch (Exception) {
        return false;
      }
    }

    private IEnumerable<string> Libs {
      get {
        string privateCorePath = typeof(object).Assembly.Location;
        List<string> libs = new List<string>() { privateCorePath };

        string systemDir = Path.GetDirectoryName(privateCorePath);
        foreach (string path in Directory.EnumerateFiles(systemDir, "*.dll")) {
          if (IsCorrectSystemDll(path)) {
            libs.Add(path);
          }
        }

        foreach (string lib in libs_) {
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
        return libs;
      }
    }

    public void Do() {
      Compile();
    }

    private void Compile() {
      var commandLineArguments = CSharpCommandLineParser.Default.Parse(cscArguments_.Concat(new string[] { "-define:__CSharpLua__" }), null, null);
      var parseOptions = commandLineArguments.ParseOptions.WithLanguageVersion(LanguageVersion.Latest).WithDocumentationMode(DocumentationMode.Parse);
      var files = Directory.EnumerateFiles(folder_, "*.cs", SearchOption.AllDirectories);
      var syntaxTrees = files.Select(file => CSharpSyntaxTree.ParseText(File.ReadAllText(file), parseOptions, file));
      var references = Libs.Select(i => MetadataReference.CreateFromFile(i));
      LuaSyntaxGenerator.SettingInfo setting = new LuaSyntaxGenerator.SettingInfo() {
        IsNewest = isNewest_,
        HasSemicolon = false,
        Indent = indent_,
      };
      LuaSyntaxGenerator generator = new LuaSyntaxGenerator(syntaxTrees, references, commandLineArguments.CompilationOptions, Metas, setting, attributes_, folder_);
      generator.Generate(output_);
    }
  }
}
