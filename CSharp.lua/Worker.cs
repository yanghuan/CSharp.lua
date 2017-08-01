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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using CSharpLua.LuaAst;

namespace CSharpLua {
  public sealed class Worker {
    private static readonly string[] SystemDlls = new string[] {
      "mscorlib.dll",
      "System.dll",
      "System.Core.dll",
      "Microsoft.CSharp.dll",
    };
    private const string kDllSuffix = ".dll";
    private const string kSystemMeta = "~/System.xml";

    private string folder_;
    private string output_;
    private string[] libs_;
    private string[] metas_;
    private string[] cscArguments_;
    private bool isNewest_;
    private int indent_;
    private bool hasSemicolon_;
    private string[] attributes_;

    public Worker(string folder, string output, string lib, string meta, string csc, bool isClassic, string indent, bool hasSemicolon, string atts) {
      folder_ = folder;
      output_ = output;
      libs_ = Utility.Split(lib);
      metas_ = Utility.Split(meta);
      cscArguments_ = string.IsNullOrEmpty(csc) ? Array.Empty<string>() : csc.Trim().Split(';', ',', ' ');
      isNewest_ = !isClassic;
      hasSemicolon_ = hasSemicolon;
      int.TryParse(indent, out indent_);
      if (atts != null) {
        attributes_ = Utility.Split(atts, false);
      }
    }

    private IEnumerable<string> Metas {
      get {
        List<string> metas = new List<string>();
        metas.Add(Utility.GetCurrentDirectory(kSystemMeta));
        metas.AddRange(metas_);
        return metas;
      }
    }

    private IEnumerable<string> Libs {
      get {
        string runtimeDir = RuntimeEnvironment.GetRuntimeDirectory();
        List<string> libs = new List<string>();
        libs.AddRange(SystemDlls.Select(i => Path.Combine(runtimeDir, i)));
        foreach (string lib in libs_) {
          string path = lib.EndsWith(kDllSuffix) ? lib : lib + kDllSuffix;
          if (!File.Exists(path)) {
            string file = Path.Combine(runtimeDir, Path.GetFileName(path));
            if (!File.Exists(file)) {
              throw new CmdArgumentException($"lib '{path}' is not found");
            }
            path = file;
          }
          libs.Add(path);
        }
        return libs;
      }
    }

    public void Do() {
      Compiler();
    }

    private void Compiler() {
      var commandLineArguments = CSharpCommandLineParser.Default.Parse(cscArguments_, null, null);
      var parseOptions = commandLineArguments.ParseOptions.WithDocumentationMode(DocumentationMode.Parse);
      var files = Directory.EnumerateFiles(folder_, "*.cs", SearchOption.AllDirectories);
      var syntaxTrees = files.Select(file => CSharpSyntaxTree.ParseText(File.ReadAllText(file), parseOptions, file));
      var references = Libs.Select(i => MetadataReference.CreateFromFile(i));
      LuaSyntaxGenerator.SettingInfo setting = new LuaSyntaxGenerator.SettingInfo() {
        IsNewest = isNewest_,
        HasSemicolon = hasSemicolon_,
        Indent = indent_,
      };
      LuaSyntaxGenerator generator = new LuaSyntaxGenerator(syntaxTrees, references, commandLineArguments.CompilationOptions, Metas, setting, attributes_);
      generator.Generate(folder_, output_);
    }
  }
}
