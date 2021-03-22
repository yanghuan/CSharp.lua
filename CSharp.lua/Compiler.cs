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
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CSharpLua {
  public sealed class Compiler {
    private const string kDllSuffix = ".dll";
    private const string kSystemMeta = "~/System.xml";
    private const char kLuaModuleSuffix = '!';

    private readonly string input_;
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
    public bool IsNotConstantForEnum { get; set; }
    public bool IsNoConcurrent { get; set; }
    public string Include { get; set; }

    public Compiler(string input, string output, string lib, string meta, string csc, bool isClassic, string atts, string enums) {
      input_ = input;
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

    public void Compile() {
      if (Include == null) {
        GetGenerator().Generate(output_);
      } else {
        var luaSystemLibs = GetIncludeCorSysemPaths(Include);
        GetGenerator().GenerateSingleFile("out.lua", output_, luaSystemLibs);
      }
    }

    private static IEnumerable<string> GetIncludeCorSysemPaths(string dir) {
      const string kBeinMark = "load(\"";

      string allFilePath = Path.Combine(dir, "All.lua");
      if (!File.Exists(allFilePath)) {
        throw new ArgumentException($"-include: {dir} is not root directory of the CoreSystem library");
      }

      List<string> luaSystemLibs = new();
      var lines = File.ReadAllLines(allFilePath);
      foreach (string line in lines) {
        int i = line.IndexOf(kBeinMark);
        if (i != -1) {
          int begin = i + kBeinMark.Length;
          int end = line.IndexOf('"', begin);
          Contract.Assert(end != -1);
          string name = line[begin..end].Replace('.', '/');
          string path = Path.Combine(dir, "CoreSystem", $"{name}.lua");
          luaSystemLibs.Add(path);
        }
      }
      return luaSystemLibs;
    }

    private IEnumerable<string> GetSourceFiles(out bool isDirectory) {
      if (Directory.Exists(input_)) {
        isDirectory = true;
        return Directory.EnumerateFiles(input_, "*.cs", SearchOption.AllDirectories);
      }

      isDirectory = false;
      return Utility.Split(input_, true);
    }

    private LuaSyntaxGenerator GetGenerator() {
      var files = GetSourceFiles(out bool isDirectory);
      var codes = files.Select(i => (File.ReadAllText(i), i));
      var libs = GetLibs(libs_, out var luaModuleLibs);
      var setting = new LuaSyntaxGenerator.SettingInfo() {
        IsClassic = isClassic_,
        IsExportMetadata = IsExportMetadata,
        BaseFolder = isDirectory ? input_ : null,
        Attributes = attributes_,
        Enums = enums_,
        LuaModuleLibs = new HashSet<string>(luaModuleLibs),
        IsModule = IsModule,
        IsInlineSimpleProperty = IsInlineSimpleProperty,
        IsPreventDebugObject = IsPreventDebugObject,
        IsNotConstantForEnum = IsNotConstantForEnum,
      };
      return new LuaSyntaxGenerator(codes, libs, cscArguments_, Metas, setting);
    }

    public static string CompileSingleCode(string code) {
      var codes = new (string, string)[] { (code, "") };
      var generator = new LuaSyntaxGenerator(codes, GetSystemLibs(), null, GetMetas(null), new LuaSyntaxGenerator.SettingInfo());
      return generator.GenerateSingle();
    }

    /// <summary>
    /// for Blazor to use
    /// </summary>
    public static string CompileSingleCode(string code, IEnumerable<Stream> libs, IEnumerable<Stream> metas) {
      var codes = new (string, string)[] { (code, "") };
      var generator = new LuaSyntaxGenerator(codes, libs, null, metas, new LuaSyntaxGenerator.SettingInfo() { 
        IsNoConcurrent = true
      });
      return generator.GenerateSingle();
    }
  }
}
