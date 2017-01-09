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
using System.Collections.Immutable;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using CSharpLua.LuaAst;

namespace CSharpLua {
    public sealed class LuaSyntaxGenerator {
        public sealed class SettingInfo {
            public bool HasSemicolon { get; }
            private int indent_;
            public string IndentString { get; private set; }
            public bool IsNewest { get; }

            public SettingInfo() {
                Indent = 4;
                HasSemicolon = true;
                IsNewest = true;
            }

            public int Indent {
                get {
                    return indent_;
                }
                set {
                    if(indent_ != value) {
                        indent_ = value;
                        IndentString = new string(' ', indent_);
                    }
                }
            }
        }

        public XmlMetaProvider XmlMetaProvider { get; }
        public SettingInfo Setting { get; }
        private HashSet<string> exportEnums_ = new HashSet<string>();

        public LuaSyntaxGenerator(IEnumerable<string> metas) {
            XmlMetaProvider = new XmlMetaProvider(metas);
            Setting = new SettingInfo();
        }

        private List<LuaCompilationUnitSyntax> Create(CSharpCompilation compilation) {
            List<LuaCompilationUnitSyntax> luaCompilationUnits = new List<LuaCompilationUnitSyntax>();
            foreach(SyntaxTree syntaxTree in compilation.SyntaxTrees) {
                SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                CompilationUnitSyntax compilationUnitSyntax = (CompilationUnitSyntax)syntaxTree.GetRoot();
                LuaSyntaxNodeTransfor transfor = new LuaSyntaxNodeTransfor(this, semanticModel);
                var luaCompilationUnit = (LuaCompilationUnitSyntax)compilationUnitSyntax.Accept(transfor);
                if(!luaCompilationUnit.IsEmpty) {
                    luaCompilationUnits.Add(luaCompilationUnit);
                }
            }
            return luaCompilationUnits;
        }

        public void Generate(CSharpCompilation compilation, Func<LuaCompilationUnitSyntax, TextWriter> writerFunctor) {
            foreach(var luaCompilationUnit in Create(compilation)) {
                using(var writer = writerFunctor(luaCompilationUnit)) {
                    LuaRenderer rener = new LuaRenderer(this, writer);
                    luaCompilationUnit.Render(rener);
                }
            }
        }

        public bool IsEnumExport(string enumName) {
            return exportEnums_.Contains(enumName);
        }

        public void AddExportEnum(string enumName) {
            exportEnums_.Add(enumName);
        }
    }
}
