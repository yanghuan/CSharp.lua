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
    private const char kLuaModuleSuffix = '!';

    private readonly string folder_;
    private readonly string output_;
    private readonly string[] libs_;
    private readonly string[] metas_;
    private readonly string[] cscArguments_;
    private readonly bool isClassic_;
    private readonly string[] attributes_;
    private readonly string[] enums_;

    public bool IsExportMetadata { get; set; }
    public bool IsModule { get; set; }
    public bool IsInlineSimpleProperty { get; set; }
    public bool IsPreventDebugObject { get; set; }

    public Compiler(string folder, string output, string lib, string meta, string csc, bool isClassic, string atts, string enums) {
      folder_ = folder;
      output_ = output;
      libs_ = Utility.Split(lib);
      metas_ = Utility.Split(meta);
      cscArguments_ = string.IsNullOrEmpty(csc) ? Array.Empty<string>() : csc.Trim().Split(' ', '\t');
      isClassic_ = isClassic;
      if (atts != null) {
        attributes_ = Utility.Split(atts, false);
      }
      if (enums != null) {
        enums_ = Utility.Split(enums, false);
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

    private static List<string> GetSystemLibs() {
      string privateCorePath = typeof(object).Assembly.Location;
      List<string> libs = new List<string>() { privateCorePath };

      string systemDir = Path.GetDirectoryName(privateCorePath);
      foreach (string path in Directory.EnumerateFiles(systemDir, "*.dll")) {
        if (IsCorrectSystemDll(path)) {
          libs.Add(path);
        }
      }

      return libs;
    }

    private static List<string> GetLibs(IEnumerable<string> additionalLibs, out List<string> luaModuleLibs) {
      luaModuleLibs = new List<string>();
      var libs = GetSystemLibs();
      if (additionalLibs != null) {
        foreach (string additionalLib in additionalLibs) {
          string lib = additionalLib;
          bool isLuaModule = false;
          if (lib.Last() == kLuaModuleSuffix) {
            lib = lib.TrimEnd(kLuaModuleSuffix);
            isLuaModule = true;
          }

          string path = lib.EndsWith(kDllSuffix) ? lib : lib + kDllSuffix;
          if (File.Exists(path)) {
            if (isLuaModule) {
              luaModuleLibs.Add(Path.GetFileNameWithoutExtension(path));
            }

            libs.Add(path);
          } else {
            throw new CmdArgumentException($"-l {path} is not found");
          }
        }
      }
      return libs;
    }

    private static LuaSyntaxGenerator Build(
      IEnumerable<string> cscArguments,
      IEnumerable<(string Text, string Path)> codes, 
      IEnumerable<string> libs,
      IEnumerable<string> metas,
      LuaSyntaxGenerator.SettingInfo setting) {
      var commandLineArguments = CSharpCommandLineParser.Default.Parse((cscArguments ?? Array.Empty<string>()).Concat(new string[] { "-define:__CSharpLua__" }), null, null);
      var parseOptions = commandLineArguments.ParseOptions.WithLanguageVersion(LanguageVersion.CSharp8).WithDocumentationMode(DocumentationMode.Parse);
      var syntaxTrees = codes.Select(code => CSharpSyntaxTree.ParseText(code.Text, parseOptions, code.Path));
      var references = libs.Select(i => MetadataReference.CreateFromFile(i));
      return new LuaSyntaxGenerator(syntaxTrees, references, commandLineArguments, metas, setting);
    }

    public void Compile() {
      var files = Directory.EnumerateFiles(folder_, "*.cs", SearchOption.AllDirectories);
      var codes = files.Select(i => (File.ReadAllText(i), i));
      var libs = GetLibs(libs_, out var luaModuleLibs);
      var setting = new LuaSyntaxGenerator.SettingInfo() {
        IsClassic = isClassic_,
        IsExportMetadata = IsExportMetadata,
        BaseFolder = folder_,
        Attributes = attributes_,
        Enums = enums_,
        LuaModuleLibs = new HashSet<string>(luaModuleLibs),
        IsModule = IsModule,
        IsInlineSimpleProperty = IsInlineSimpleProperty,
        IsPreventDebugObject = IsPreventDebugObject,
      };
      var generator = Build(cscArguments_, codes, libs, Metas, setting);
      generator.Generate(output_);
    }

    public static string CompileSingleCode(string code) {
      var codes = new (string, string)[] { (code, "") };
      var generator = Build(null, codes, GetSystemLibs(), GetMetas(null), new LuaSyntaxGenerator.SettingInfo());
      return generator.GenerateSingle();
    }
  }
}
