using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua.LuaAst {
    public abstract class LuaTypeDeclarationSyntax : LuaWrapFunctionStatementSynatx {
        public LuaLocalVariablesStatementSyntax Local { get; } = new LuaLocalVariablesStatementSyntax();
        public LuaStatementListSyntax MethodList { get; } = new LuaStatementListSyntax();
        public LuaTableInitializerExpression ResultTable { get; } = new LuaTableInitializerExpression();

        public LuaTypeDeclarationSyntax() {
            Add(Local);
            Add(MethodList);
            LuaReturnStatementSyntax returnNode = new LuaReturnStatementSyntax();
            returnNode.Expressions.Add(ResultTable);
            Add(returnNode);
        }

        public void AddMethod(LuaIdentifierNameSyntax name, LuaFunctionExpressSyntax method, bool isPrivate) {
            Local.Variables.Add(name);
            LuaAssignmentExpressionSyntax assignmentNode = new LuaAssignmentExpressionSyntax(name, method);
            MethodList.Statements.Add(new LuaExpressionStatementSyntax(assignmentNode));
            if(!isPrivate) {
                LuaKeyValueTableItemSyntax itemNode = new LuaKeyValueTableItemSyntax(new LuaTableLiteralKeySyntax(name), name);
                ResultTable.Items.Add(itemNode);
            }
        }

        public void AddField(LuaIdentifierNameSyntax name, LuaExpressionSyntax value, bool isStatic, bool isImmutable) {
            if(isImmutable) {
                LuaKeyValueTableItemSyntax itemNode = new LuaKeyValueTableItemSyntax(new LuaTableLiteralKeySyntax(name), value);
                ResultTable.Items.Add(itemNode);
            }
            else {
                if(isStatic) {

                }
                else {

                }
            }
        }
    }

    public sealed class LuaClassDeclarationSyntax : LuaTypeDeclarationSyntax {
        public LuaClassDeclarationSyntax(LuaIdentifierNameSyntax name) {
            UpdateIdentifiers(name, LuaIdentifierNameSyntax.Namespace, LuaIdentifierNameSyntax.Class, LuaIdentifierNameSyntax.Namespace);
        }
    }

    public sealed class LuaStructDeclarationSyntax : LuaTypeDeclarationSyntax {
        public LuaStructDeclarationSyntax(LuaIdentifierNameSyntax name) {
            UpdateIdentifiers(name, LuaIdentifierNameSyntax.Namespace, LuaIdentifierNameSyntax.Struct, LuaIdentifierNameSyntax.Namespace);
        }
    }

    public sealed class LuaInterfaceDeclarationSyntax : LuaTypeDeclarationSyntax {
        public LuaInterfaceDeclarationSyntax(LuaIdentifierNameSyntax name) {
            UpdateIdentifiers(name, LuaIdentifierNameSyntax.Namespace, LuaIdentifierNameSyntax.Interface);
        }
    }

    public sealed class LuaEnumDeclarationSyntax : LuaTypeDeclarationSyntax {
        public LuaEnumDeclarationSyntax(LuaIdentifierNameSyntax name) {
            UpdateIdentifiers(name, LuaIdentifierNameSyntax.Namespace, LuaIdentifierNameSyntax.Enum);
        }
    }
}
