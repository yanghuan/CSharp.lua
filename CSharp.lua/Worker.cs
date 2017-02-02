/*
Copyright 2016 YANG Huan (sy.yanghuan@gmail.com).
Copyright 2016 Redmoon Inc.

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
        private const string kLuaSuffix = ".lua";
        private const string kSystemMeta = "~/System.xml";

        private string folder_;
        private string output_;
        private string[] libs_;
        private string[] metas_;
        private string[] defines_;

        public Worker(string folder, string output, string lib, string meta, string defines) {
            folder_ = folder;
            output_ = output;
            libs_ = Utility.Split(lib);
            metas_ = Utility.Split(meta);
            defines_ = Utility.Split(defines, false);
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
                foreach(string lib in libs_) {
                    string path = lib.EndsWith(kDllSuffix) ? lib : lib + kDllSuffix;
                    if(!File.Exists(path)) {
                        string file = Path.Combine(runtimeDir, Path.GetFileName(path));
                        if(!File.Exists(file)) {
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
            CSharpParseOptions parseOptions = new CSharpParseOptions(preprocessorSymbols: defines_);
            var files = Directory.EnumerateFiles(folder_, "*.cs", SearchOption.AllDirectories);
            var syntaxTrees = files.Select(file => CSharpSyntaxTree.ParseText(File.ReadAllText(file), parseOptions, file));
            var references = Libs.Select(i => MetadataReference.CreateFromFile(i));
            CSharpCompilation compilation = CSharpCompilation.Create("_", syntaxTrees, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using(MemoryStream ms = new MemoryStream()) {
                EmitResult result = compilation.Emit(ms);
                if(!result.Success) {
                    var errors = result.Diagnostics.Where(i => i.Severity == DiagnosticSeverity.Error);
                    string message = string.Join("\n", errors);
                    throw new CompilationErrorException(message);
                }
            }

            LuaSyntaxGenerator generator = new LuaSyntaxGenerator(Metas, compilation);
            generator.Generate(folder_, output_);
        }
    }
}
