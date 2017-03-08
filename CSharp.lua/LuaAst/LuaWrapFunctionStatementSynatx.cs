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
using System.Text;

namespace CSharpLua.LuaAst {
    public abstract class LuaWrapFunctionStatementSynatx : LuaStatementSyntax {
        public LuaExpressionStatementSyntax Statement { get; private set; }
        private LuaFunctionExpressionSyntax function_ = new LuaFunctionExpressionSyntax();

        protected void UpdateIdentifiers(LuaIdentifierNameSyntax name, LuaIdentifierNameSyntax target, LuaIdentifierNameSyntax memberName, LuaIdentifierNameSyntax parameter = null) {
            LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(target, memberName);
            LuaInvocationExpressionSyntax invoke = new LuaInvocationExpressionSyntax(memberAccess);
            invoke.AddArgument(new LuaStringLiteralExpressionSyntax(name));
            invoke.AddArgument(function_);
            if(parameter != null) {
                function_.AddParameter(parameter);
            }
            Statement = new LuaExpressionStatementSyntax(invoke);
        }

        public LuaBlockSyntax Body {
            get {
                return function_.Body;
            }
        }

        public void AddMemberDeclaration(LuaWrapFunctionStatementSynatx statement) {
            if(statement == null) {
                throw new ArgumentNullException(nameof(statement));
            }
            Body.Statements.Add(statement);
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}