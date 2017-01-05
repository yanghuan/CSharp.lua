using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using CSharpLua.LuaAst;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

namespace CSharpLua {
    public sealed partial class LuaSyntaxNodeTransfor : CSharpSyntaxVisitor<LuaSyntaxNode> {
        private LuaSyntaxGenerator generator_;
        private SemanticModel semanticModel_;

        private Stack<LuaCompilationUnitSyntax> compilationUnits_ = new Stack<LuaCompilationUnitSyntax>();
        private Stack<LuaTypeDeclarationSyntax> typeDeclarations_ = new Stack<LuaTypeDeclarationSyntax>();
        private Stack<LuaFunctionExpressionSyntax> functions_ = new Stack<LuaFunctionExpressionSyntax>();
        private Stack<LuaSwitchAdapterStatementSyntax> switchs_ = new Stack<LuaSwitchAdapterStatementSyntax>();
        private Stack<LuaBlockSyntax> blocks_ = new Stack<LuaBlockSyntax>();

        private static readonly Dictionary<string, string> operatorTokenMapps_ = new Dictionary<string, string>() {
            ["!="] = LuaSyntaxNode.Tokens.NotEquals,
            ["!"] = LuaSyntaxNode.Tokens.Not,
            ["&&"] = LuaSyntaxNode.Tokens.And,
            ["||"] = LuaSyntaxNode.Tokens.Or,
            ["??"] = LuaSyntaxNode.Tokens.Or,
            ["^"] = "~"
        };

        public LuaSyntaxNodeTransfor(LuaSyntaxGenerator generator, SemanticModel semanticModel) {
            generator_ = generator;
            semanticModel_ = semanticModel;
        }

        private XmlMetaProvider XmlMetaProvider {
            get {
                return generator_.XmlMetaProvider;
            }
        }

        private static string GetOperatorToken(SyntaxToken operatorToken) {
            string token = operatorToken.ValueText;
            return operatorTokenMapps_.GetOrDefault(token, token);
        }

        private bool IsLuaNewest {
            get {
                return generator_.Setting.IsNewest;
            }
        }

        private LuaCompilationUnitSyntax CurCompilationUnit {
            get {
                return compilationUnits_.Peek();
            }
        }

        private LuaTypeDeclarationSyntax CurType {
            get {
                return typeDeclarations_.Peek();
            }
        }

        private LuaFunctionExpressionSyntax CurFunction {
            get {
                return functions_.Peek();
            }
        }

        private void PushFunction(LuaFunctionExpressionSyntax function) {
            functions_.Push(function);
            ++localMappingCounter_;
        }

        private void PopFunction() {
            functions_.Pop();
            --localMappingCounter_;
            if(localMappingCounter_ == 0) {
                localReservedNames_.Clear();
            }
        }

        private LuaBlockSyntax CurBlock {
            get {
                return blocks_.Peek();
            }
        }

        public override LuaSyntaxNode VisitCompilationUnit(CompilationUnitSyntax node) {
            LuaCompilationUnitSyntax compilationUnit = new LuaCompilationUnitSyntax() { FilePath = node.SyntaxTree.FilePath };
            compilationUnits_.Push(compilationUnit);
            foreach(var member in node.Members) {
                LuaStatementSyntax memberNode = (LuaStatementSyntax)member.Accept(this);
                var typeDeclaration = memberNode as LuaTypeDeclarationSyntax;
                if(typeDeclaration != null) {
                    compilationUnit.AddTypeDeclaration(typeDeclaration);
                }
                else {
                    compilationUnit.Statements.Add(memberNode);
                }
            }
            compilationUnits_.Pop();
            return compilationUnit;
        }

        public override LuaSyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node) {
            LuaIdentifierNameSyntax name = (LuaIdentifierNameSyntax)node.Name.Accept(this);
            LuaNamespaceDeclarationSyntax namespaceDeclaration = new LuaNamespaceDeclarationSyntax(name);
            foreach(var member in node.Members) {
                var memberNode = (LuaTypeDeclarationSyntax)member.Accept(this);
                namespaceDeclaration.Add(memberNode);
            }
            return namespaceDeclaration;
        }

        private void VisitTypeDeclaration(TypeDeclarationSyntax node, LuaTypeDeclarationSyntax typeDeclaration) {
            typeDeclarations_.Push(typeDeclaration);
            if(node.TypeParameterList != null) {
                foreach(var typeParameter in node.TypeParameterList.Parameters) {
                    var typeIdentifier = (LuaIdentifierNameSyntax)typeParameter.Accept(this);
                    typeDeclaration.AddTypeIdentifier(typeIdentifier);
                }
            }
            if(node.BaseList != null) {
                List<LuaIdentifierNameSyntax> baseTypes = new List<LuaIdentifierNameSyntax>();
                foreach(var baseType in node.BaseList.Types) {
                    var baseTypeName = (LuaIdentifierNameSyntax)baseType.Accept(this);
                    baseTypes.Add(baseTypeName);
                }
                typeDeclaration.AddBaseTypes(baseTypes);
            }
            foreach(var member in node.Members) {
                var newMember = member.Accept(this);
                SyntaxKind kind = member.Kind();
                if(kind >= SyntaxKind.ClassDeclaration && kind <= SyntaxKind.EnumDeclaration) {
                    typeDeclaration.Add((LuaStatementSyntax)newMember);
                }
            }
            typeDeclarations_.Pop();
        }

        public override LuaSyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node) {
            LuaIdentifierNameSyntax name = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
            LuaClassDeclarationSyntax classDeclaration = new LuaClassDeclarationSyntax(name);
            VisitTypeDeclaration(node, classDeclaration);
            return classDeclaration;
        }

        public override LuaSyntaxNode VisitStructDeclaration(StructDeclarationSyntax node) {
            LuaIdentifierNameSyntax name = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
            LuaStructDeclarationSyntax structDeclaration = new LuaStructDeclarationSyntax(name);
            VisitTypeDeclaration(node, structDeclaration);
            return structDeclaration;
        }

        public override LuaSyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node) {
            LuaIdentifierNameSyntax name = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
            LuaInterfaceDeclarationSyntax interfaceDeclaration = new LuaInterfaceDeclarationSyntax(name);
            VisitTypeDeclaration(node, interfaceDeclaration);
            return interfaceDeclaration;
        }

        public override LuaSyntaxNode VisitEnumDeclaration(EnumDeclarationSyntax node) {
            var symbol = semanticModel_.GetDeclaredSymbol(node);
            string fullName = symbol.ToString();
            LuaIdentifierNameSyntax name = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
            LuaEnumDeclarationSyntax enumDeclaration = new LuaEnumDeclarationSyntax(fullName, name);
            typeDeclarations_.Push(enumDeclaration);
            foreach(var member in node.Members) {
                var statement = (LuaKeyValueTableItemSyntax)member.Accept(this);
                enumDeclaration.Add(statement);
            }
            typeDeclarations_.Pop();
            return enumDeclaration;
        }

        private void VisitYield(MethodDeclarationSyntax node, LuaFunctionExpressionSyntax function) {
            Contract.Assert(function.HasYield);

            var nameSyntax = (SimpleNameSyntax)node.ReturnType;
            string name = LuaSyntaxNode.Tokens.Yield + nameSyntax.Identifier.ValueText;
            LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.System, new LuaIdentifierNameSyntax(name));
            LuaInvocationExpressionSyntax invokeExpression = new LuaInvocationExpressionSyntax(memberAccess);
            LuaFunctionExpressionSyntax wrapFunction = new LuaFunctionExpressionSyntax();

            var parameters = function.ParameterList.Parameters;
            wrapFunction.ParameterList.Parameters.AddRange(parameters);
            wrapFunction.Body.Statements.AddRange(function.Body.Statements);
            invokeExpression.AddArgument(wrapFunction);
            if(node.ReturnType.IsKind(SyntaxKind.GenericName)) {
                var genericNameSyntax = (GenericNameSyntax)nameSyntax;
                var typeName = genericNameSyntax.TypeArgumentList.Arguments.First();
                var expression = (LuaExpressionSyntax)typeName.Accept(this);
                invokeExpression.AddArgument(expression);
            }
            else {
                invokeExpression.AddArgument(LuaIdentifierNameSyntax.Object);
            }
            invokeExpression.ArgumentList.Arguments.AddRange(parameters.Select(i => new LuaArgumentSyntax(i.Identifier)));

            LuaReturnStatementSyntax returnStatement = new LuaReturnStatementSyntax(invokeExpression);
            function.Body.Statements.Clear();
            function.Body.Statements.Add(returnStatement);
        }

        private LuaStatementSyntax BuildMethodDefaultParameterInit(LuaIdentifierNameSyntax parameterIdentifier, LuaExpressionSyntax defaultValue) {
            LuaBinaryExpressionSyntax binaryExpression = new LuaBinaryExpressionSyntax(parameterIdentifier, LuaSyntaxNode.Tokens.Or, defaultValue);
            LuaAssignmentExpressionSyntax initAssignment = new LuaAssignmentExpressionSyntax(parameterIdentifier, binaryExpression);
            return new LuaExpressionStatementSyntax(initAssignment);
        }

        public override LuaSyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node) {
            IMethodSymbol symbol = semanticModel_.GetDeclaredSymbol(node);
            string methodName = XmlMetaProvider.GetMethodMapName(symbol);
            LuaIdentifierNameSyntax name = new LuaIdentifierNameSyntax(methodName);
            LuaFunctionExpressionSyntax function = new LuaFunctionExpressionSyntax();
            PushFunction(function);
            if(!node.Modifiers.IsStatic()) {
                function.AddParameter(LuaIdentifierNameSyntax.This);
            }

            foreach(var parameter in node.ParameterList.Parameters) {
                var luaParameter = (LuaParameterSyntax)parameter.Accept(this);
                CheckParameterName(ref luaParameter, parameter);
                function.ParameterList.Parameters.Add(luaParameter);
                if(parameter.Default != null) {
                    var expression = (LuaExpressionSyntax)parameter.Default.Value.Accept(this);
                    var intiStatement = BuildMethodDefaultParameterInit(luaParameter.Identifier, expression);
                    function.Body.Statements.Add(intiStatement);
                }
                else {
                    if(parameter.Modifiers.IsParams()) {
                        var typeName = (LuaIdentifierNameSyntax)((ArrayTypeSyntax)parameter.Type).ElementType.Accept(this);
                        LuaInvocationExpressionSyntax invocation = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.ArrayEmpty);
                        invocation.AddArgument(typeName);
                        var intiStatement = BuildMethodDefaultParameterInit(luaParameter.Identifier, invocation);
                        function.Body.Statements.Add(intiStatement);
                    }
                }
            }
            if(node.TypeParameterList != null) {
                foreach(var typeParameter in node.TypeParameterList.Parameters) {
                    var typeName = (LuaIdentifierNameSyntax)typeParameter.Accept(this);
                    function.AddParameter(typeName);
                }
            }

            LuaBlockSyntax block = (LuaBlockSyntax)node.Body.Accept(this);
            function.Body.Statements.AddRange(block.Statements);
            if(function.HasYield) {
                VisitYield(node, function);
            }
            PopFunction();
            CurType.AddMethod(name, function, node.Modifiers.IsPrivate());
            return function;
        }

        private static LuaExpressionSyntax GetPredefinedTypeDefaultValue(ITypeSymbol typeSymbol) {
            switch(typeSymbol.SpecialType) {
                case SpecialType.System_Boolean: {
                        return new LuaIdentifierNameSyntax(default(bool).ToString());
                    }
                case SpecialType.System_Char: {
                        return new LuaCharacterLiteralExpression(default(char));
                    }
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64: {
                        return new LuaIdentifierNameSyntax(0.ToString());
                    }
                case SpecialType.System_Single:
                case SpecialType.System_Double: {
                        return new LuaIdentifierNameSyntax(0.0.ToString());
                    }
                default:
                    return null;
            }
        }

        private LuaIdentifierNameSyntax GetTempIdentifier(SyntaxNode node) {
            int index = CurFunction.TempIndex++;
            string name = LuaSyntaxNode.TempIdentifiers.GetOrDefault(index);
            if(name == null) {
                throw new CompilationErrorException($"{node.GetLocationString()} : Your code is startling, {LuaSyntaxNode.TempIdentifiers.Length} " 
                    + "temporary variables is not enough, please refactor your code.");
            }
            return new LuaIdentifierNameSyntax(name);
        }

        private LuaInvocationExpressionSyntax BuildDefaultValueExpression(TypeSyntax type) {
            var identifierName = (LuaIdentifierNameSyntax)type.Accept(this);
            return new LuaInvocationExpressionSyntax(new LuaMemberAccessExpressionSyntax(identifierName, LuaIdentifierNameSyntax.Default));
        }

        private void VisitBaseFieldDeclarationSyntax(BaseFieldDeclarationSyntax node) {
            if(!node.Modifiers.IsConst()) {
                bool isStatic = node.Modifiers.IsStatic();
                bool isPrivate = node.Modifiers.IsPrivate();
                bool isReadOnly = node.Modifiers.IsReadOnly();
                var type = node.Declaration.Type;
                ITypeSymbol typeSymbol = (ITypeSymbol)semanticModel_.GetSymbolInfo(type).Symbol;
                bool isImmutable = typeSymbol.IsImmutable();
                foreach(var variable in node.Declaration.Variables) {
                    if(node.IsKind(SyntaxKind.EventFieldDeclaration)) {
                        var eventSymbol = (IEventSymbol)semanticModel_.GetDeclaredSymbol(variable);
                        if(eventSymbol.IsOverridable() || eventSymbol.IsInterfaceImplementation()) {
                            bool valueIsLiteral;
                            LuaExpressionSyntax valueExpression = GetFieldValueExpression(type, typeSymbol, variable.Initializer?.Value, out valueIsLiteral);
                            CurType.AddEvent(variable.Identifier.ValueText, valueExpression, isImmutable && valueIsLiteral, isStatic, isPrivate);
                            continue;
                        }
                    }
                    AddField(type, typeSymbol, variable.Identifier, variable.Initializer?.Value, isImmutable, isStatic, isPrivate, isReadOnly);
                }
            }
        }

        public override LuaSyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node) {
            VisitBaseFieldDeclarationSyntax(node);
            return base.VisitFieldDeclaration(node);
        }

        private LuaExpressionSyntax GetFieldValueExpression(TypeSyntax type, ITypeSymbol typeSymbol, ExpressionSyntax expression, out bool valueIsLiteral) {
            LuaExpressionSyntax valueExpression = null;
            valueIsLiteral = false;
            if(expression != null) {
                valueExpression = (LuaExpressionSyntax)expression.Accept(this);
                valueIsLiteral = expression is LiteralExpressionSyntax;
            }
            if(valueExpression == null) {
                if(typeSymbol.IsValueType) {
                    if(typeSymbol.IsDefinition) {
                        LuaExpressionSyntax defalutValue = GetPredefinedTypeDefaultValue(typeSymbol);
                        if(defalutValue != null) {
                            valueExpression = defalutValue;
                        }
                        else {
                            valueExpression = BuildDefaultValueExpression(type);
                        }
                        valueIsLiteral = true;
                    }
                    else {
                        valueExpression = BuildDefaultValueExpression(type);
                    }
                }
            }
            return valueExpression;
        }

        private void AddField(TypeSyntax type, ITypeSymbol typeSymbol, SyntaxToken identifier, ExpressionSyntax expression, bool isImmutable, bool isStatic, bool isPrivate, bool isReadOnly) {
            LuaIdentifierNameSyntax name = new LuaIdentifierNameSyntax(identifier.ValueText);
            bool valueIsLiteral;
            LuaExpressionSyntax valueExpression = GetFieldValueExpression(type, typeSymbol, expression, out valueIsLiteral);
            CurType.AddField(name, valueExpression, isImmutable && valueIsLiteral, isStatic, isPrivate, isReadOnly);
        }

        public override LuaSyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node) {
            bool isStatic = node.Modifiers.IsStatic();
            bool isPrivate = node.Modifiers.IsPrivate();
            bool hasGet = false;
            bool hasSet = false;
            if(node.AccessorList != null) {
                foreach(var accessor in node.AccessorList.Accessors) {
                    if(accessor.Body != null) {
                        LuaFunctionExpressionSyntax functionExpression = new LuaFunctionExpressionSyntax();
                        if(!isStatic) {
                            functionExpression.AddParameter(LuaIdentifierNameSyntax.This);
                        }
                        PushFunction(functionExpression);
                        var block = (LuaBlockSyntax)accessor.Body.Accept(this);
                        PopFunction();
                        functionExpression.Body.Statements.AddRange(block.Statements);
                        LuaPropertyOrEventIdentifierNameSyntax name = new LuaPropertyOrEventIdentifierNameSyntax(true, node.Identifier.ValueText);
                        CurType.AddMethod(name, functionExpression, isPrivate);
                        if(accessor.IsKind(SyntaxKind.GetAccessorDeclaration)) {
                            Contract.Assert(!hasGet);
                            hasGet = true;
                        }
                        else {
                            Contract.Assert(!hasSet);
                            functionExpression.AddParameter(LuaIdentifierNameSyntax.Value);
                            name.IsGetOrAdd = false;
                            hasSet = true;
                        }
                    }
                }
            }
            else {
                Contract.Assert(!hasGet);
                LuaPropertyOrEventIdentifierNameSyntax name = new LuaPropertyOrEventIdentifierNameSyntax(true, node.Identifier.ValueText);
                var expression = (LuaExpressionSyntax)node.ExpressionBody.Expression.Accept(this);
                LuaFunctionExpressionSyntax functionExpress = new LuaFunctionExpressionSyntax();
                if(!isStatic) {
                    functionExpress.AddParameter(LuaIdentifierNameSyntax.This);
                }
                LuaReturnStatementSyntax returnStatement = new LuaReturnStatementSyntax(expression);
                functionExpress.Body.Statements.Add(returnStatement);
                CurType.AddMethod(name, functionExpress, isPrivate);
                hasGet = true;
            }

            if(!hasGet && !hasSet) {
                if(!node.Parent.IsKind(SyntaxKind.InterfaceDeclaration)) {
                    var type = node.Type;
                    ITypeSymbol typeSymbol = (ITypeSymbol)semanticModel_.GetSymbolInfo(type).Symbol;
                    bool isImmutable = typeSymbol.IsImmutable();
                    if(isStatic) {
                        bool isReadOnly = node.AccessorList.Accessors.Count == 1 && node.AccessorList.Accessors[0].Body == null;
                        AddField(type, typeSymbol, node.Identifier, node.Initializer?.Value, isImmutable, isStatic, isPrivate, isReadOnly);
                    }
                    else {
                        bool isAuto = semanticModel_.GetDeclaredSymbol(node).IsPropertyField();
                        if(isAuto) {
                            bool isReadOnly = node.AccessorList.Accessors.Count == 1 && node.AccessorList.Accessors[0].Body == null;
                            AddField(type, typeSymbol, node.Identifier, node.Initializer?.Value, isImmutable, isStatic, isPrivate, isReadOnly);
                        }
                        else {
                            bool valueIsLiteral;
                            LuaExpressionSyntax valueExpression = GetFieldValueExpression(type, typeSymbol, node.Initializer?.Value, out valueIsLiteral);
                            CurType.AddProperty(node.Identifier.ValueText, valueExpression, isImmutable && valueIsLiteral, isStatic, isPrivate);
                        }
                    }
                }
            }

            return base.VisitPropertyDeclaration(node);
        }

        public override LuaSyntaxNode VisitEventDeclaration(EventDeclarationSyntax node) {
            bool isStatic = node.Modifiers.IsStatic();
            bool isPrivate = node.Modifiers.IsPrivate();
            foreach(var accessor in node.AccessorList.Accessors) {
                var block = (LuaBlockSyntax)accessor.Body.Accept(this);
                LuaFunctionExpressionSyntax functionExpress = new LuaFunctionExpressionSyntax();
                if(!isStatic) {
                    functionExpress.AddParameter(LuaIdentifierNameSyntax.This);
                }
                functionExpress.AddParameter(LuaIdentifierNameSyntax.Value);
                functionExpress.Body.Statements.AddRange(block.Statements);
                LuaPropertyOrEventIdentifierNameSyntax name = new LuaPropertyOrEventIdentifierNameSyntax(false, node.Identifier.ValueText);
                CurType.AddMethod(name, functionExpress, isPrivate);
                if(accessor.IsKind(SyntaxKind.RemoveAccessorDeclaration)) {
                    name.IsGetOrAdd = false;
                }
            }

            return base.VisitEventDeclaration(node);
        }

        public override LuaSyntaxNode VisitEventFieldDeclaration(EventFieldDeclarationSyntax node) {
            VisitBaseFieldDeclarationSyntax(node);
            return base.VisitEventFieldDeclaration(node);
        }

        public override LuaSyntaxNode VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node) {
            IFieldSymbol symbol = semanticModel_.GetDeclaredSymbol(node);
            Contract.Assert(symbol.HasConstantValue);
            LuaIdentifierNameSyntax identifier = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
            LuaExpressionSyntax value = GetConstLiteralExpression(symbol.ConstantValue);
            return new LuaKeyValueTableItemSyntax(new LuaTableLiteralKeySyntax(identifier), value);
        }

        public override LuaSyntaxNode VisitIndexerDeclaration(IndexerDeclarationSyntax node) {
            bool isStatic = node.Modifiers.IsStatic();
            bool isPrivate = node.Modifiers.IsPrivate();
            bool hasGet = false;
            bool hasSet = false;
            foreach(var accessor in node.AccessorList.Accessors) {
                LuaFunctionExpressionSyntax functionExpression = new LuaFunctionExpressionSyntax();
                if(!isStatic) {
                    functionExpression.AddParameter(LuaIdentifierNameSyntax.This);
                }
                PushFunction(functionExpression);
                var block = (LuaBlockSyntax)accessor.Body.Accept(this);
                PopFunction();
                functionExpression.Body.Statements.AddRange(block.Statements);
                LuaPropertyOrEventIdentifierNameSyntax name = new LuaPropertyOrEventIdentifierNameSyntax(true, string.Empty);
                CurType.AddMethod(name, functionExpression, isPrivate);
                if(accessor.IsKind(SyntaxKind.GetAccessorDeclaration)) {
                    Contract.Assert(!hasGet);
                    hasGet = true;
                }
                else {
                    Contract.Assert(!hasSet);
                    functionExpression.AddParameter(LuaIdentifierNameSyntax.Value);
                    name.IsGetOrAdd = false;
                    hasSet = true;
                }
            }

            return base.VisitIndexerDeclaration(node);
        }

        public override LuaSyntaxNode VisitParameterList(ParameterListSyntax node) {
            LuaParameterListSyntax parameterList = new LuaParameterListSyntax();
            foreach(var parameter in node.Parameters) {
                var newNode = (LuaParameterSyntax)parameter.Accept(this);
                parameterList.Parameters.Add(newNode);
            }
            return parameterList;
        }

        public override LuaSyntaxNode VisitParameter(ParameterSyntax node) {
            LuaIdentifierNameSyntax identifier = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
            return new LuaParameterSyntax(identifier);
        }

        private sealed class BlockCommonNode : IComparable<BlockCommonNode> {
            const int kCommentCharCount = 2;
            public SyntaxTrivia Comment { get; }
            public StatementSyntax Statement { get; }
            public FileLinePositionSpan LineSpan { get; }

            public BlockCommonNode(SyntaxTrivia comment) {
                Comment = comment;
                LineSpan = comment.SyntaxTree.GetLineSpan(comment.Span);
            }

            public BlockCommonNode(StatementSyntax statement) {
                Statement = statement;
                LineSpan = statement.SyntaxTree.GetLineSpan(statement.Span);
            }

            public int CompareTo(BlockCommonNode other) {
                return LineSpan.StartLinePosition.CompareTo(other.LineSpan.StartLinePosition);
            }

            public void Visit(LuaSyntaxNodeTransfor transfor, LuaBlockSyntax block, ref int lastLine) {
                if(lastLine != -1) {
                    int count = LineSpan.StartLinePosition.Line - lastLine - 1;
                    if(count > 0) {
                        block.Statements.Add(new LuaBlankLinesStatement(count));
                    }
                }

                if(Statement != null) {
                    LuaStatementSyntax statementNode = (LuaStatementSyntax)Statement.Accept(transfor);
                    block.Statements.Add(statementNode);
                }
                else {
                    string content = Comment.ToString();
                    if(Comment.IsKind(SyntaxKind.SingleLineCommentTrivia)) {
                        string commentContent = content.Substring(kCommentCharCount);
                        LuaShortCommentStatement singleComment = new LuaShortCommentStatement(commentContent);
                        block.Statements.Add(singleComment);
                    }
                    else {
                        string commentContent = content.Substring(kCommentCharCount, content.Length - kCommentCharCount - kCommentCharCount);
                        LuaLongCommentStatement longComment = new LuaLongCommentStatement(commentContent);
                        block.Statements.Add(longComment);
                    }
                }

                lastLine = LineSpan.EndLinePosition.Line;
            }
        }

        public override LuaSyntaxNode VisitBlock(BlockSyntax node) {
            LuaBlockSyntax block = new LuaBlockSyntax();
            blocks_.Push(block);

            var comments = node.DescendantTrivia().Where(i => i.IsKind(SyntaxKind.SingleLineCommentTrivia) || i.IsKind(SyntaxKind.MultiLineCommentTrivia));
            List<BlockCommonNode> commonNodes = new List<BlockCommonNode>();
            commonNodes.AddRange(comments.Select(i => new BlockCommonNode(i)));
            bool hasComments = commonNodes.Count > 0;
            commonNodes.AddRange(node.Statements.Select(i => new BlockCommonNode(i)));
            if(hasComments) {
                commonNodes.Sort();
            }

            int lastLine = -1;
            foreach(var common in commonNodes) {
                common.Visit(this, block, ref lastLine);
            }

            blocks_.Pop();
            SyntaxKind kind = node.Parent.Kind();
            if(kind == SyntaxKind.Block || kind == SyntaxKind.SwitchSection) {
                return new LuaBlockBlockSyntax(block);
            }
            else {
                return block;
            }
        }

        public override LuaSyntaxNode VisitReturnStatement(ReturnStatementSyntax node) {
            if(CurFunction is LuaSpecialAdapterFunctionExpressionSyntax) {
                LuaMultipleReturnStatementSyntax returnStatement = new LuaMultipleReturnStatementSyntax();
                returnStatement.Expressions.Add(LuaIdentifierNameSyntax.True);
                if(node.Expression != null) {
                    var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
                    returnStatement.Expressions.Add(expression);
                }
                return returnStatement;
            }
            else {
                if(node.Expression != null) {
                    var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
                    return new LuaReturnStatementSyntax(expression);
                }
                return new LuaReturnStatementSyntax();
            }
        }

        public override LuaSyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node) {
            LuaExpressionSyntax expressionNode = (LuaExpressionSyntax)node.Expression.Accept(this);
            return new LuaExpressionStatementSyntax(expressionNode);
        }

        private LuaExpressionSyntax BuildCommonAssignmentExpression(LuaExpressionSyntax left, LuaExpressionSyntax right, string operatorToken) {
            var propertyAdapter = left as LuaPropertyAdapterExpressionSyntax;
            if(propertyAdapter != null) {
                propertyAdapter.InvocationExpression.AddArgument(new LuaBinaryExpressionSyntax(propertyAdapter.GetCloneOfGet(), operatorToken, right));
                return propertyAdapter;
            }
            else {
                return new LuaAssignmentExpressionSyntax(left, new LuaBinaryExpressionSyntax(left, operatorToken, right));
            }
        }

        private LuaExpressionSyntax BuildDelegateAssignmentExpression(LuaExpressionSyntax left, LuaExpressionSyntax right, bool isPlus) {
            if(right is LuaInternalMethodIdentifierNameSyntax) { 
                LuaInvocationExpressionSyntax invocation = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.DelegateBind);
                invocation.AddArgument(LuaIdentifierNameSyntax.This);
                invocation.AddArgument(right);
                right = invocation;
            }
            else if(right is LuaMemberAccessExpressionSyntax) {
                LuaMemberAccessExpressionSyntax memberAccess = (LuaMemberAccessExpressionSyntax)right;
                if(memberAccess.IsObjectColon) {
                    LuaInvocationExpressionSyntax invocation = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.DelegateBind);
                    invocation.AddArgument(memberAccess.Expression);
                    invocation.AddArgument(new LuaMemberAccessExpressionSyntax(memberAccess.Expression, memberAccess.Name));
                    right = invocation;
                }
            }

            var methodName = isPlus ? LuaIdentifierNameSyntax.DelegateCombine : LuaIdentifierNameSyntax.DelegateRemove;
            var propertyAdapter = left as LuaPropertyAdapterExpressionSyntax;
            if(propertyAdapter != null) {
                if(propertyAdapter.IsProperty) {
                    propertyAdapter.InvocationExpression.AddArgument(BuildBinaryInvokeExpression(propertyAdapter.GetCloneOfGet(), right, methodName));
                    return propertyAdapter;
                }
                else {
                    propertyAdapter.IsGetOrAdd = isPlus;
                    propertyAdapter.InvocationExpression.AddArgument(right);
                    return propertyAdapter;
                }
            }
            else {
                return new LuaAssignmentExpressionSyntax(left, BuildBinaryInvokeExpression(left, right, methodName));
            }
        }

        private LuaExpressionSyntax BuildLuaAssignmentExpression(ExpressionSyntax leftNode, ExpressionSyntax rightNode, SyntaxKind kind) {
            var left = (LuaExpressionSyntax)leftNode.Accept(this);
            var right = (LuaExpressionSyntax)rightNode.Accept(this);

            switch(kind) {
                case SyntaxKind.SimpleAssignmentExpression: {
                        var propertyAdapter = left as LuaPropertyAdapterExpressionSyntax;
                        if(propertyAdapter != null) {
                            propertyAdapter.IsGetOrAdd = false;
                            propertyAdapter.InvocationExpression.AddArgument(right);
                            return propertyAdapter;
                        }
                        else {
                            return new LuaAssignmentExpressionSyntax(left, right);
                        }
                    }
                case SyntaxKind.AddAssignmentExpression: {
                        var leftType = semanticModel_.GetTypeInfo(leftNode).Type;
                        if(leftType.IsStringType()) {
                            return BuildCommonAssignmentExpression(left, right, LuaSyntaxNode.Tokens.Concatenation);
                        }
                        else if(leftType.IsDelegateType()) {
                            return BuildDelegateAssignmentExpression(left, right, true);
                        }
                        else {
                            return BuildCommonAssignmentExpression(left, right, LuaSyntaxNode.Tokens.Plus);
                        }
                    }
                case SyntaxKind.SubtractAssignmentExpression: {
                        var leftType = semanticModel_.GetTypeInfo(leftNode).Type;
                        if(leftType.IsDelegateType()) {
                            return BuildDelegateAssignmentExpression(left, right, false);
                        }
                        else {
                            return BuildCommonAssignmentExpression(left, right, LuaSyntaxNode.Tokens.Sub);
                        }
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        public override LuaSyntaxNode VisitAssignmentExpression(AssignmentExpressionSyntax node) {
            List<LuaExpressionSyntax> assignments = new List<LuaExpressionSyntax>();

            while(true) {
                var leftExpression = node.Left;
                var rightExpression = node.Right;
                var kind = node.Kind();

                var assignmentRight = rightExpression as AssignmentExpressionSyntax;
                if(assignmentRight == null) {
                    assignments.Add(BuildLuaAssignmentExpression(leftExpression, rightExpression, kind));
                    break;
                }
                else {
                    assignments.Add(BuildLuaAssignmentExpression(leftExpression, assignmentRight.Left, kind));
                    node = assignmentRight;
                }
            }

            if(assignments.Count == 1) {
                return assignments.First();
            }
            else {
                assignments.Reverse();
                LuaLineMultipleExpressionSyntax multipleAssignment = new LuaLineMultipleExpressionSyntax();
                multipleAssignment.Assignments.AddRange(assignments);
                return multipleAssignment;
            }
        }

        private LuaSyntaxNode BuildInvokeRefOrOut(InvocationExpressionSyntax node, LuaExpressionSyntax invocation, IEnumerable<LuaExpressionSyntax> refOrOutArguments) {
            if(node.Parent.IsKind(SyntaxKind.ExpressionStatement)) {
                LuaMultipleAssignmentExpressionSyntax multipleAssignment = new LuaMultipleAssignmentExpressionSyntax();
                SymbolInfo symbolInfo = semanticModel_.GetSymbolInfo(node);
                IMethodSymbol symbol = (IMethodSymbol)symbolInfo.Symbol;
                if(!symbol.ReturnsVoid) {
                    var temp = GetTempIdentifier(node);
                    CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(new LuaVariableDeclaratorSyntax(temp)));
                    multipleAssignment.Lefts.Add(temp);
                }
                multipleAssignment.Lefts.AddRange(refOrOutArguments);
                multipleAssignment.Rights.Add(invocation);
                return multipleAssignment;
            }
            else {
                var temp = GetTempIdentifier(node);
                LuaMultipleAssignmentExpressionSyntax multipleAssignment = new LuaMultipleAssignmentExpressionSyntax();
                multipleAssignment.Lefts.Add(temp);
                multipleAssignment.Lefts.AddRange(refOrOutArguments);
                multipleAssignment.Rights.Add(invocation);

                CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(new LuaVariableDeclaratorSyntax(temp)));
                CurBlock.Statements.Add(new LuaExpressionStatementSyntax(multipleAssignment));
                return temp;
            }
        }

        public override LuaSyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node) {
            var symbol = (IMethodSymbol)semanticModel_.GetSymbolInfo(node).Symbol;
            if(node.Expression.IsKind(SyntaxKind.SimpleMemberAccessExpression)) {
                string codeTemplate = XmlMetaProvider.GetMethodCodeTemplate(symbol);
                if(codeTemplate != null) {
                    List<ExpressionSyntax> argumentExpressions = new List<ExpressionSyntax>();
                    var memberAccessExpression = (MemberAccessExpressionSyntax)node.Expression;
                    if(symbol.IsExtensionMethod) {
                        argumentExpressions.Add(memberAccessExpression.Expression);
                        if(symbol.ContainingType.IsSystemLinqEnumerable()) {
                            CurCompilationUnit.ImportLinq();
                        }
                    }
                    argumentExpressions.AddRange(node.ArgumentList.Arguments.Select(i => i.Expression));
                    var invocationExpression = BuildCodeTemplateExpression(codeTemplate, memberAccessExpression.Expression, argumentExpressions, symbol.TypeArguments);
                    var refOrOuts = node.ArgumentList.Arguments.Where(i => i.RefOrOutKeyword.IsKind(SyntaxKind.RefKeyword) || i.RefOrOutKeyword.IsKind(SyntaxKind.OutKeyword));
                    if(refOrOuts.Any()) {
                        return BuildInvokeRefOrOut(node, invocationExpression, refOrOuts.Select(i => ((LuaArgumentSyntax)i.Accept(this)).Expression));
                    }
                    else {
                        return invocationExpression;
                    }
                }
            }

            List<LuaArgumentSyntax> arguments = new List<LuaArgumentSyntax>();
            List<LuaExpressionSyntax> refOrOutArguments = new List<LuaExpressionSyntax>();

            foreach(var argument in node.ArgumentList.Arguments) {
                var luaArgument = (LuaArgumentSyntax)argument.Accept(this);
                arguments.Add(luaArgument);
                if(argument.RefOrOutKeyword.IsKind(SyntaxKind.RefKeyword) || argument.RefOrOutKeyword.IsKind(SyntaxKind.OutKeyword)) {
                    refOrOutArguments.Add(luaArgument.Expression);
                }
            }

            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            LuaInvocationExpressionSyntax invocation;
            if(!symbol.IsExtensionMethod) {
                invocation = new LuaInvocationExpressionSyntax(expression);
                if(expression is LuaInternalMethodIdentifierNameSyntax) {
                    invocation.AddArgument(LuaIdentifierNameSyntax.This);
                }
            }
            else {
                LuaMemberAccessExpressionSyntax memberAccess = (LuaMemberAccessExpressionSyntax)expression;
                IMethodSymbol reducedFrom = symbol.ReducedFrom;
                string name = reducedFrom.ContainingType.ToString() + '.' + reducedFrom.Name;
                invocation = new LuaInvocationExpressionSyntax(new LuaIdentifierNameSyntax(name));
                invocation.AddArgument(memberAccess.Expression);
            }
            invocation.ArgumentList.Arguments.AddRange(arguments);
            if(symbol.TypeArguments.Length > 0) {
                int optionalCount = symbol.Parameters.Length - node.ArgumentList.Arguments.Count;
                while(optionalCount > 0) {
                    invocation.AddArgument(LuaIdentifierNameSyntax.Nil);
                    --optionalCount;
                }
                foreach(var typeArgument in symbol.TypeArguments) {
                    string typeName = XmlMetaProvider.GetTypeMapName(typeArgument);
                    invocation.AddArgument(new LuaIdentifierNameSyntax(typeName));
                }
            }
            if(refOrOutArguments.Count > 0) {
                return BuildInvokeRefOrOut(node, invocation, refOrOutArguments);
            }
            return invocation;
        }

        private LuaExpressionSyntax BuildMemberAccessTargetExpression(ExpressionSyntax targetExpression) {
            var expression = (LuaExpressionSyntax)targetExpression.Accept(this);
            SyntaxKind kind = targetExpression.Kind();
            if(kind >= SyntaxKind.NumericLiteralExpression && kind <= SyntaxKind.NullLiteralExpression) {
                expression = new LuaParenthesizedExpressionSyntax(expression);
            }
            return expression;
        }

        public override LuaSyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node) {
            ISymbol symbol = semanticModel_.GetSymbolInfo(node).Symbol;
            if(symbol.Kind != SymbolKind.Property && symbol.Kind != SymbolKind.Event) {
                if(symbol.Kind == SymbolKind.Field) {
                    IFieldSymbol fieldSymbol = (IFieldSymbol)symbol;
                    string codeTemplate = XmlMetaProvider.GetFieldCodeTemplate(fieldSymbol);
                    if(codeTemplate != null) {
                        return new LuaIdentifierNameSyntax(codeTemplate);
                    }

                    if(fieldSymbol.HasConstantValue) {
                        return GetConstLiteralExpression(fieldSymbol.ConstantValue);
                    }
                }

                var expression = BuildMemberAccessTargetExpression(node.Expression);
                var identifier = (LuaIdentifierNameSyntax)node.Name.Accept(this);
                if(node.Expression.IsKind(SyntaxKind.ThisExpression)) {
                    return identifier;
                }
                else if(node.Expression.IsKind(SyntaxKind.BaseExpression)) {
                    if(expression != LuaIdentifierNameSyntax.This) {
                        var baseTypeName = ((LuaIdentifierNameSyntax)expression);
                        return new LuaInternalMethodIdentifierNameSyntax(baseTypeName.ValueText + '.' + identifier.ValueText);
                    }
                }
                return new LuaMemberAccessExpressionSyntax(expression, identifier, !symbol.IsStatic && symbol.Kind == SymbolKind.Method);
            }
            else {
                if(symbol.Kind == SymbolKind.Property) {
                    IPropertySymbol propertySymbol = (IPropertySymbol)symbol;
                    bool isGet = !node.Parent.Kind().IsAssignment();
                    string codeTemplate = XmlMetaProvider.GetProertyCodeTemplate(propertySymbol, isGet);
                    if(codeTemplate != null) {
                        return BuildCodeTemplateExpression(codeTemplate, node.Expression);
                    }
                }

                var expression = BuildMemberAccessTargetExpression(node.Expression);
                var propertyIdentifier = (LuaExpressionSyntax)node.Name.Accept(this);
                var propertyAdapter = propertyIdentifier as LuaPropertyAdapterExpressionSyntax;
                if(propertyAdapter != null) {
                    if(!node.Expression.IsKind(SyntaxKind.ThisExpression)) {
                        bool hasObjectColon = !symbol.IsStatic;
                        if(node.Expression.IsKind(SyntaxKind.BaseExpression)) {
                            if(expression != LuaIdentifierNameSyntax.This) {
                                propertyAdapter.InvocationExpression.AddArgument(LuaIdentifierNameSyntax.This);
                                hasObjectColon = false;
                            }
                        }

                        var memberAccessExpression = new LuaMemberAccessExpressionSyntax(expression, propertyAdapter.InvocationExpression.Expression, hasObjectColon);
                        propertyAdapter.Update(memberAccessExpression);
                    }
                    return propertyAdapter;
                }
                else {
                    return new LuaMemberAccessExpressionSyntax(expression, propertyIdentifier);
                }
            }
        }

        private string BuildStaticFieldName(ISymbol symbol, bool isReadOnly, IdentifierNameSyntax node) {
            Contract.Assert(symbol.IsStatic);
            string name;
            if(symbol.DeclaredAccessibility == Accessibility.Private) {
                name = symbol.Name;
            }
            else {
                if(isReadOnly) {
                    name = symbol.Name;
                    if(node.Parent.IsKind(SyntaxKind.SimpleAssignmentExpression)) {
                        AssignmentExpressionSyntax assignmentExpression = (AssignmentExpressionSyntax)node.Parent;
                        if(assignmentExpression.Left == node) {
                            CurType.AddStaticReadOnlyAssignmentName(name);
                        }
                    }
                }
                else {
                    var constructor = CurFunction as LuaConstructorAdapterExpressionSyntax;
                    if(constructor != null && constructor.IsStaticCtor) {
                        name = LuaSyntaxNode.Tokens.This + '.' + symbol.Name;
                    }
                    else {
                        if(IsInternalNode(node)) {
                            name = symbol.ToString();
                        }
                        else {
                            name = symbol.Name;
                        }
                    }
                }
            }
            return name;
        }

        private bool IsInternalNode(NameSyntax node) {
            bool isInternal = false;
            MemberAccessExpressionSyntax parent = null;
            if(node.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression)) {
                parent = (MemberAccessExpressionSyntax)node.Parent;
            }
            if(parent != null) {
                if(parent.Expression.IsKind(SyntaxKind.ThisExpression)) {
                    isInternal = true;
                }
                else if(parent.Expression == node) {
                    isInternal = true;
                }
            }
            else {
                isInternal = true;
            }
            return isInternal;
        }

        private LuaExpressionSyntax VisitFieldOrEventIdentifierName(IdentifierNameSyntax node, ISymbol symbol, bool isProperty) {
            string name;
            bool isField, isReadOnly;
            if(isProperty) {
                var propertySymbol = (IPropertySymbol)symbol;
                isField =  IsPropertyField(propertySymbol);
                isReadOnly = propertySymbol.IsReadOnly;
            }
            else {
                var eventSymbol = (IEventSymbol)symbol;
                isField = eventSymbol.IsEventFiled();
                isReadOnly = false;
            }

            if(symbol.IsStatic) {
                if(isField) {
                    name = BuildStaticFieldName(symbol, isReadOnly, node);
                }
                else {
                    return new LuaPropertyAdapterExpressionSyntax(new LuaPropertyOrEventIdentifierNameSyntax(isProperty, symbol.Name));
                }
            }
            else {
                if(isField) {
                    if(IsInternalNode(node)) {
                        name = LuaSyntaxNode.Tokens.This + '.' + symbol.Name;
                    }
                    else {
                        name = symbol.Name;
                    }
                }
                else {
                    if(IsInternalNode(node)) {
                        if(IsInternalMember(node, symbol)) {
                            var propertyAdapter = new LuaPropertyAdapterExpressionSyntax(new LuaPropertyOrEventIdentifierNameSyntax(isProperty, symbol.Name));
                            propertyAdapter.InvocationExpression.AddArgument(LuaIdentifierNameSyntax.This);
                            return propertyAdapter;
                        }
                        else {
                            LuaPropertyOrEventIdentifierNameSyntax identifierName = new LuaPropertyOrEventIdentifierNameSyntax(isProperty, symbol.Name);
                            LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.This, identifierName, true);
                            return new LuaPropertyAdapterExpressionSyntax(memberAccess, identifierName);
                        }
                    }
                    else {
                        return new LuaPropertyAdapterExpressionSyntax(new LuaPropertyOrEventIdentifierNameSyntax(isProperty, symbol.Name));
                    }
                }
            }
            return new LuaIdentifierNameSyntax(name);
        }

        private LuaLiteralExpressionSyntax GetConstLiteralExpression(object constantValue) {
            if(constantValue != null) {
                var code = Type.GetTypeCode(constantValue.GetType());
                switch(code) {
                    case TypeCode.Char: {
                            return new LuaCharacterLiteralExpression((char)constantValue);
                        }
                    case TypeCode.String: {
                            return new LuaStringLiteralExpressionSyntax((string)constantValue);
                        }
                    default: {
                            return new LuaIdentifierLiteralExpressionSyntax(constantValue.ToString());
                        }
                }
            }
            else {
                return new LuaIdentifierLiteralExpressionSyntax(LuaIdentifierNameSyntax.Nil);
            }
        }

        private LuaExpressionSyntax GetMethodNameExpression(IMethodSymbol symbol, NameSyntax node) {
            string name;
            string methodName = XmlMetaProvider.GetMethodMapName(symbol);
            if(symbol.IsStatic) {
                name = methodName;
            }
            else {
                if(IsInternalNode(node)) {
                    if(IsInternalMember(node, symbol)) {
                        return new LuaInternalMethodIdentifierNameSyntax(methodName);
                    }
                    else {
                        LuaIdentifierNameSyntax identifierName = new LuaIdentifierNameSyntax(methodName);
                        LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.This, identifierName, true);
                        return memberAccess;
                    }
                }
                else {
                    name = methodName;
                }
            }
            return new LuaIdentifierNameSyntax(name);
        }

        public override LuaSyntaxNode VisitIdentifierName(IdentifierNameSyntax node) {
            SymbolInfo symbolInfo = semanticModel_.GetSymbolInfo(node);
            ISymbol symbol = symbolInfo.Symbol;
            string name;
            switch(symbol.Kind) {
                case SymbolKind.Local:
                case SymbolKind.Parameter: {
                        name = symbol.Name;
                        CheckReservedWord(ref name, symbol);
                        break;
                    }
                case SymbolKind.TypeParameter:
                case SymbolKind.Label: {
                        name = symbol.Name;
                        break;
                    }
                case SymbolKind.Namespace:
                case SymbolKind.NamedType: {
                        name = symbol.ToString();
                        break;
                    }
                case SymbolKind.Field: {
                        if(symbol.IsStatic) {
                            var fieldSymbol = (IFieldSymbol)symbol;
                            if(fieldSymbol.HasConstantValue) {
                                return GetConstLiteralExpression(fieldSymbol.ConstantValue);
                            }
                            else {
                                name = BuildStaticFieldName(symbol, fieldSymbol.IsReadOnly, node);
                            }
                        }
                        else {
                            if(IsInternalNode(node)) {
                                name = LuaSyntaxNode.Tokens.This + '.' + symbol.Name;
                            }
                            else {
                                name = symbol.Name;
                            }
                        }
                        break;
                    }
                case SymbolKind.Method: {
                        return GetMethodNameExpression((IMethodSymbol)symbol, node);
                    }
                case SymbolKind.Property: {
                        return VisitFieldOrEventIdentifierName(node, symbol, true);
                    }
                case SymbolKind.Event: {
                        return VisitFieldOrEventIdentifierName(node, symbol, false);
                    }
                default: {
                        throw new NotSupportedException();
                    }
            }
            return new LuaIdentifierNameSyntax(name);
        }

        public override LuaSyntaxNode VisitQualifiedName(QualifiedNameSyntax node) {
            return new LuaIdentifierNameSyntax(node.ToString());
        }

        public override LuaSyntaxNode VisitArgumentList(ArgumentListSyntax node) {
            LuaArgumentListSyntax argumentList = new LuaArgumentListSyntax();
            foreach(var argument in node.Arguments) {
                var newNode = (LuaArgumentSyntax)argument.Accept(this);
                argumentList.Arguments.Add(newNode);
            }
            return argumentList;
        }

        public override LuaSyntaxNode VisitArgument(ArgumentSyntax node) {
            LuaExpressionSyntax expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            LuaArgumentSyntax argument = new LuaArgumentSyntax(expression);
            return argument;
        }

        public override LuaSyntaxNode VisitLiteralExpression(LiteralExpressionSyntax node) {
            switch(node.Kind()) {
                case SyntaxKind.CharacterLiteralExpression: {
                        return new LuaCharacterLiteralExpression((char)node.Token.Value);
                    }
                case SyntaxKind.NullLiteralExpression: {
                        return new LuaIdentifierLiteralExpressionSyntax(LuaIdentifierNameSyntax.Nil);
                    }
                default: {
                        return new LuaIdentifierLiteralExpressionSyntax(node.Token.Text);
                    }
            }
        }

        public override LuaSyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node) {
            var declaration = (LuaVariableDeclarationSyntax)node.Declaration.Accept(this);
            return new LuaLocalDeclarationStatementSyntax(declaration);
        }

        public override LuaSyntaxNode VisitVariableDeclaration(VariableDeclarationSyntax node) {
            LuaVariableListDeclarationSyntax variableListDeclaration = new LuaVariableListDeclarationSyntax();
            foreach(VariableDeclaratorSyntax variable in node.Variables) {
                var variableDeclarator = (LuaVariableDeclaratorSyntax)variable.Accept(this);
                variableListDeclaration.Variables.Add(variableDeclarator);
            }
            bool isMultiNil = variableListDeclaration.Variables.Count > 0 && variableListDeclaration.Variables.All(i => i.Initializer == null);
            if(isMultiNil) {
                LuaLocalVariablesStatementSyntax declarationStatement = new LuaLocalVariablesStatementSyntax();
                foreach(var variable in variableListDeclaration.Variables) {
                    declarationStatement.Variables.Add(variable.Identifier);
                }
                return declarationStatement;
            }
            else {
                return variableListDeclaration;
            }
        }

        public override LuaSyntaxNode VisitVariableDeclarator(VariableDeclaratorSyntax node) {
            LuaIdentifierNameSyntax identifier = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
            CheckVariableDeclaratorName(ref identifier, node);
            LuaVariableDeclaratorSyntax variableDeclarator = new LuaVariableDeclaratorSyntax(identifier);
            if(node.Initializer != null) {
                variableDeclarator.Initializer = (LuaEqualsValueClauseSyntax)node.Initializer.Accept(this);
            }
            return variableDeclarator;
        }

        public override LuaSyntaxNode VisitEqualsValueClause(EqualsValueClauseSyntax node) {
            var expression = (LuaExpressionSyntax)node.Value.Accept(this);
            return new LuaEqualsValueClauseSyntax(expression);
        }

        public override LuaSyntaxNode VisitPredefinedType(PredefinedTypeSyntax node) {
            ISymbol symbol = semanticModel_.GetSymbolInfo(node).Symbol;
            string typeName = XmlMetaProvider.GetTypeMapName(symbol);
            return new LuaIdentifierNameSyntax(typeName);
        }

        private void WriteStatementOrBlock(StatementSyntax statement, LuaBlockSyntax block) {
            if(statement.IsKind(SyntaxKind.Block)) {
                var blockNode = (LuaBlockSyntax)statement.Accept(this);
                block.Statements.AddRange(blockNode.Statements);
            }
            else {
                blocks_.Push(block);
                var statementNode = (LuaStatementSyntax)statement.Accept(this);
                block.Statements.Add(statementNode);
                blocks_.Pop();
            }
        }

        #region if else switch

        public override LuaSyntaxNode VisitIfStatement(IfStatementSyntax node) {
            var condition = (LuaExpressionSyntax)node.Condition.Accept(this);
            LuaIfStatementSyntax ifStatement = new LuaIfStatementSyntax(condition);
            WriteStatementOrBlock(node.Statement, ifStatement.Body);
            ifStatement.Else = (LuaElseClauseSyntax)node.Else?.Accept(this);
            return ifStatement;
        }

        public override LuaSyntaxNode VisitElseClause(ElseClauseSyntax node) {
            LuaStatementSyntax statement;
            if(node.Statement.IsKind(SyntaxKind.IfStatement)) {
                statement = (LuaStatementSyntax)node.Statement.Accept(this);
            }
            else {
                LuaBlockSyntax block = new LuaBlockSyntax();
                WriteStatementOrBlock(node.Statement, block);
                statement = block;
            }
            LuaElseClauseSyntax elseClause = new LuaElseClauseSyntax(statement);
            return elseClause;
        }

        public override LuaSyntaxNode VisitSwitchStatement(SwitchStatementSyntax node) {
            var temp = GetTempIdentifier(node);
            LuaSwitchAdapterStatementSyntax switchStatement = new LuaSwitchAdapterStatementSyntax(temp);
            switchs_.Push(switchStatement);
            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            switchStatement.Fill(expression, node.Sections.Select(i => (LuaStatementSyntax)i.Accept(this)));
            switchs_.Pop();
            return switchStatement;
        }

        public override LuaSyntaxNode VisitSwitchSection(SwitchSectionSyntax node) {
            bool isDefault = node.Labels.Any(i => i.Kind() == SyntaxKind.DefaultSwitchLabel);
            if(isDefault) {
                LuaBlockSyntax block = new LuaBlockSyntax();
                foreach(var statement in node.Statements) {
                    var luaStatement = (LuaStatementSyntax)statement.Accept(this);
                    block.Statements.Add(luaStatement);
                }
                return block;
            }
            else {
                var expressions = node.Labels.Select(i => (LuaExpressionSyntax)i.Accept(this));
                var condition = expressions.Aggregate((x, y) => new LuaBinaryExpressionSyntax(x, LuaSyntaxNode.Tokens.Or, y));
                LuaIfStatementSyntax ifStatement = new LuaIfStatementSyntax(condition);
                foreach(var statement in node.Statements) {
                    var luaStatement = (LuaStatementSyntax)statement.Accept(this);
                    ifStatement.Body.Statements.Add(luaStatement);
                }
                return ifStatement;
            }
        }

        public override LuaSyntaxNode VisitCaseSwitchLabel(CaseSwitchLabelSyntax node) {
            var left = switchs_.Peek().Temp;
            var right = (LuaLiteralExpressionSyntax)node.Value.Accept(this);
            LuaBinaryExpressionSyntax BinaryExpression = new LuaBinaryExpressionSyntax(left, LuaSyntaxNode.Tokens.EqualsEquals, right);
            return BinaryExpression;
        }

        #endregion

        public override LuaSyntaxNode VisitBreakStatement(BreakStatementSyntax node) {
            return LuaBreakStatementSyntax.Statement;
        }

        private LuaExpressionSyntax WrapStringConcatExpression(ExpressionSyntax expression) {
            var typeInfo = semanticModel_.GetTypeInfo(expression).Type;
            var original = (LuaExpressionSyntax)expression.Accept(this);
            if(typeInfo.IsStringType()) {
                if(expression is BinaryExpressionSyntax) {
                    return original;
                }

                var constValue = semanticModel_.GetConstantValue(expression);
                if(constValue.HasValue) {
                    return original;
                }
                else {
                    LuaBinaryExpressionSyntax binaryExpression = new LuaBinaryExpressionSyntax(original, LuaSyntaxNode.Tokens.Or, LuaStringLiteralExpressionSyntax.Empty);
                    return new LuaParenthesizedExpressionSyntax(binaryExpression);
                }
            }
            else if(typeInfo.SpecialType == SpecialType.System_Char) {
                var constValue = semanticModel_.GetConstantValue(expression);
                if(constValue.HasValue) {
                    return new LuaCharacterStringLiteralExpressionSyntax((char)constValue.Value);
                }
                else {
                    LuaInvocationExpressionSyntax invocation = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.StringChar);
                    invocation.AddArgument(original);
                    return invocation;
                }
            }
            else if(typeInfo.SpecialType >= SpecialType.System_Boolean && typeInfo.SpecialType <= SpecialType.System_Double) {
                return original;
            }
            else if(typeInfo.TypeKind == TypeKind.Enum) {
                var symbol = semanticModel_.GetSymbolInfo(expression).Symbol;
                if(original is LuaLiteralExpressionSyntax) {
                    return new LuaStringLiteralExpressionSyntax(symbol.Name);
                }
                else {
                    ITypeSymbol type;
                    if(symbol.Kind == SymbolKind.Parameter) {
                        type = ((IParameterSymbol)symbol).Type;
                    }
                    else {
                        type = ((IFieldSymbol)symbol).Type;
                    }
                    string typeName = XmlMetaProvider.GetTypeMapName(type);
                    LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(new LuaIdentifierNameSyntax(symbol.Name), LuaIdentifierNameSyntax.ToEnumString, true);
                    LuaInvocationExpressionSyntax invocation = new LuaInvocationExpressionSyntax(memberAccess);
                    invocation.AddArgument(new LuaIdentifierNameSyntax(typeName));
                    generator_.AddExportEnum(symbol.ToString());
                    return invocation;
                }
            }
            else if(typeInfo.IsValueType) {
                LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(original, LuaIdentifierNameSyntax.ToStr, true);
                return new LuaInvocationExpressionSyntax(memberAccess);
            }
            else {
                LuaInvocationExpressionSyntax invocation = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.StringConcat);
                invocation.AddArgument(original);
                return invocation;
            }
        }

        private LuaBinaryExpressionSyntax BuildStringConcatExpression(BinaryExpressionSyntax node) {
            return BuildStringConcatExpression(node.Left, node.Right);
        }

        private LuaBinaryExpressionSyntax BuildStringConcatExpression(ExpressionSyntax leftNode, ExpressionSyntax rightNode) {
            var left = WrapStringConcatExpression(leftNode);
            var right = WrapStringConcatExpression(rightNode);
            return new LuaBinaryExpressionSyntax(left, LuaSyntaxNode.Tokens.Concatenation, right);
        }

        private LuaExpressionSyntax BuildBinaryInvokeExpression(LuaExpressionSyntax left, LuaExpressionSyntax right, LuaIdentifierNameSyntax name) {
            LuaInvocationExpressionSyntax invocation = new LuaInvocationExpressionSyntax(name);
            invocation.AddArgument(left);
            invocation.AddArgument(right);
            return invocation;
        }

        private LuaExpressionSyntax BuildBinaryInvokeExpression(BinaryExpressionSyntax node, LuaIdentifierNameSyntax name) {
            var left = (LuaExpressionSyntax)node.Left.Accept(this);
            var right = (LuaExpressionSyntax)node.Right.Accept(this);
            return BuildBinaryInvokeExpression(left, right, name);
        }

        private LuaExpressionSyntax BuildIntegerDivExpression(BinaryExpressionSyntax node) {
            if(IsLuaNewest) {
                return BuildBinaryExpression(node, LuaSyntaxNode.Tokens.Div);
            }
            else {
                return BuildBinaryInvokeExpression(node, LuaIdentifierNameSyntax.IntegerDiv);
            }
        }

        private LuaBinaryExpressionSyntax BuildBinaryExpression(BinaryExpressionSyntax node, string operatorToken) {
            var left = (LuaExpressionSyntax)node.Left.Accept(this);
            var right = (LuaExpressionSyntax)node.Right.Accept(this);
            return new LuaBinaryExpressionSyntax(left, operatorToken, right);
        }

        private LuaExpressionSyntax BuildBitExpression(BinaryExpressionSyntax node, string boolOperatorToken, LuaIdentifierNameSyntax otherName) {
            var leftType = semanticModel_.GetTypeInfo(node.Left).Type;
            if(leftType.SpecialType == SpecialType.System_Boolean) {
                return BuildBinaryExpression(node, boolOperatorToken);
            }
            else if(!IsLuaNewest) {
                return BuildBinaryInvokeExpression(node, otherName);
            }
            else {
                string operatorToken = GetOperatorToken(node.OperatorToken);
                return BuildBinaryExpression(node, operatorToken);
            }
        }

        public override LuaSyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node) {
            switch(node.Kind()) {
                case SyntaxKind.AddExpression: {
                        var methodSymbol = semanticModel_.GetSymbolInfo(node).Symbol as IMethodSymbol;
                        if(methodSymbol != null) {
                            if(methodSymbol.ContainingType.IsStringType()) {
                                return BuildStringConcatExpression(node);
                            }
                            else if(methodSymbol.ContainingType.IsDelegateType()) {
                                return BuildBinaryInvokeExpression(node, LuaIdentifierNameSyntax.DelegateCombine);
                            }
                        }
                        break;
                    }
                case SyntaxKind.SubtractExpression: {
                        var methodSymbol = semanticModel_.GetSymbolInfo(node).Symbol as IMethodSymbol;
                        if(methodSymbol != null && methodSymbol.ContainingType.IsDelegateType()) {
                            return BuildBinaryInvokeExpression(node, LuaIdentifierNameSyntax.DelegateRemove);
                        }
                        break;
                    }
                case SyntaxKind.DivideExpression: {
                        var leftType = semanticModel_.GetTypeInfo(node.Left).Type;
                        var rightType = semanticModel_.GetTypeInfo(node.Right).Type;
                        if(leftType.IsIntegerType() && rightType.IsIntegerType()) {
                            return BuildIntegerDivExpression(node);
                        }
                        break;
                    }
                case SyntaxKind.ModuloExpression: {
                        if(!IsLuaNewest) {
                            var leftType = semanticModel_.GetTypeInfo(node.Left).Type;
                            var rightType = semanticModel_.GetTypeInfo(node.Right).Type;
                            if(leftType.IsIntegerType() && rightType.IsIntegerType()) {
                                return BuildBinaryInvokeExpression(node, LuaIdentifierNameSyntax.IntegerMod);
                            }
                        }
                        break;
                    }
                case SyntaxKind.LeftShiftExpression: {
                        if(!IsLuaNewest) {
                            return BuildBinaryInvokeExpression(node, LuaIdentifierNameSyntax.ShiftLeft);
                        }
                        break;
                    }
                case SyntaxKind.RightShiftExpression: {
                        if(!IsLuaNewest) {
                            return BuildBinaryInvokeExpression(node, LuaIdentifierNameSyntax.ShiftRight);
                        }
                        break;
                    }
                case SyntaxKind.BitwiseOrExpression: {
                        return BuildBitExpression(node, LuaSyntaxNode.Tokens.Or, LuaIdentifierNameSyntax.BitOr);
                    }
                case SyntaxKind.BitwiseAndExpression: {
                        return BuildBitExpression(node, LuaSyntaxNode.Tokens.And, LuaIdentifierNameSyntax.BitAnd);
                    }
                case SyntaxKind.ExclusiveOrExpression: {
                        return BuildBitExpression(node, LuaSyntaxNode.Tokens.NotEquals, LuaIdentifierNameSyntax.BitXor);
                    }
                case SyntaxKind.IsExpression: {
                        return BuildBinaryInvokeExpression(node, LuaIdentifierNameSyntax.Is);
                    }
                case SyntaxKind.AsExpression: {
                        return BuildBinaryInvokeExpression(node, LuaIdentifierNameSyntax.As);
                    }
            }
            string operatorToken = GetOperatorToken(node.OperatorToken);
            return BuildBinaryExpression(node, operatorToken);
        }

        private LuaAssignmentExpressionSyntax GetLuaAssignmentExpressionSyntax(ExpressionSyntax operand, bool isPlus) {
            var expression = (LuaExpressionSyntax)operand.Accept(this);
            string operatorToken = isPlus ? LuaSyntaxNode.Tokens.Plus : LuaSyntaxNode.Tokens.Sub;
            LuaBinaryExpressionSyntax binary = new LuaBinaryExpressionSyntax(expression, operatorToken, LuaIdentifierNameSyntax.One);
            LuaAssignmentExpressionSyntax assignment = new LuaAssignmentExpressionSyntax(expression, binary);
            return assignment;
        }

        public override LuaSyntaxNode VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node) {
            SyntaxKind kind = node.Kind();
            if(kind == SyntaxKind.PreIncrementExpression || kind == SyntaxKind.PreDecrementExpression) {
                LuaAssignmentExpressionSyntax assignment = GetLuaAssignmentExpressionSyntax(node.Operand, kind == SyntaxKind.PreIncrementExpression);
                if(node.Parent.IsKind(SyntaxKind.ExpressionStatement) || node.Parent.IsKind(SyntaxKind.ForStatement)) {
                    return assignment;
                }
                else {
                    CurBlock.Statements.Add(new LuaExpressionStatementSyntax(assignment));
                    return assignment.Left;
                }
            }
            else {
                var operand = (LuaExpressionSyntax)node.Operand.Accept(this);
                string operatorToken = GetOperatorToken(node.OperatorToken);
                LuaPrefixUnaryExpressionSyntax unaryExpression = new LuaPrefixUnaryExpressionSyntax(operand, operatorToken);
                return unaryExpression;
            }
        }

        public override LuaSyntaxNode VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node) {
            SyntaxKind kind = node.Kind();
            if(kind != SyntaxKind.PostIncrementExpression && kind != SyntaxKind.PostDecrementExpression) {
                throw new NotSupportedException();
            }
            LuaAssignmentExpressionSyntax assignment = GetLuaAssignmentExpressionSyntax(node.Operand, kind == SyntaxKind.PostIncrementExpression);
            if(node.Parent.IsKind(SyntaxKind.ExpressionStatement) || node.Parent.IsKind(SyntaxKind.ForStatement)) {
                return assignment;
            }
            else {
                var temp = GetTempIdentifier(node);
                LuaVariableDeclaratorSyntax variableDeclarator = new LuaVariableDeclaratorSyntax(temp);
                variableDeclarator.Initializer = new LuaEqualsValueClauseSyntax(assignment.Left);
                CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(variableDeclarator));
                CurBlock.Statements.Add(new LuaExpressionStatementSyntax(assignment));
                return temp;
            }
        }

        public override LuaSyntaxNode VisitContinueStatement(ContinueStatementSyntax node) {
            return LuaContinueAdapterStatementSyntax.Statement;      
        }

        private void VisitLoopBody(StatementSyntax bodyStatement, LuaBlockSyntax block) {
            bool hasContinue = IsContinueExists(bodyStatement);
            if(hasContinue) {
                // http://lua-users.org/wiki/ContinueProposal
                var continueIdentifier = LuaIdentifierNameSyntax.Continue;
                block.Statements.Add(new LuaLocalVariableDeclaratorSyntax(new LuaVariableDeclaratorSyntax(continueIdentifier)));
                LuaRepeatStatementSyntax repeatStatement = new LuaRepeatStatementSyntax(LuaIdentifierNameSyntax.One);
                WriteStatementOrBlock(bodyStatement, repeatStatement.Body);
                LuaAssignmentExpressionSyntax assignment = new LuaAssignmentExpressionSyntax(continueIdentifier, LuaIdentifierNameSyntax.True);
                repeatStatement.Body.Statements.Add(new LuaExpressionStatementSyntax(assignment));
                block.Statements.Add(repeatStatement);
                LuaIfStatementSyntax IfStatement = new LuaIfStatementSyntax(new LuaPrefixUnaryExpressionSyntax(continueIdentifier, LuaSyntaxNode.Tokens.Not));
                IfStatement.Body.Statements.Add(LuaBreakStatementSyntax.Statement);
                block.Statements.Add(IfStatement);
            }
            else {
                WriteStatementOrBlock(bodyStatement, block);
            }
        }

        public override LuaSyntaxNode VisitForEachStatement(ForEachStatementSyntax node) {
            LuaIdentifierNameSyntax identifier = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            LuaForInStatementSyntax forInStatement = new LuaForInStatementSyntax(identifier, expression);
            VisitLoopBody(node.Statement, forInStatement.Body);
            return forInStatement;
        }

        public override LuaSyntaxNode VisitWhileStatement(WhileStatementSyntax node) {
            var condition = (LuaExpressionSyntax)node.Condition.Accept(this);
            LuaWhileStatementSyntax whileStatement = new LuaWhileStatementSyntax(condition);
            VisitLoopBody(node.Statement, whileStatement.Body);
            return whileStatement;
        }

        public override LuaSyntaxNode VisitForStatement(ForStatementSyntax node) {
            LuaBlockSyntax body = new LuaBlockSyntax();
            blocks_.Push(body);

            if(node.Declaration != null) {
                body.Statements.Add((LuaVariableDeclarationSyntax)node.Declaration.Accept(this));
            }
            var initializers = node.Initializers.Select(i => new LuaExpressionStatementSyntax((LuaExpressionSyntax)i.Accept(this)));
            body.Statements.AddRange(initializers);

            LuaExpressionSyntax condition = node.Condition != null ? (LuaExpressionSyntax)node.Condition.Accept(this) : LuaIdentifierNameSyntax.True;
            LuaWhileStatementSyntax whileStatement = new LuaWhileStatementSyntax(condition);
            blocks_.Push(whileStatement.Body);
            VisitLoopBody(node.Statement, whileStatement.Body);
            var incrementors = node.Incrementors.Select(i => new LuaExpressionStatementSyntax((LuaExpressionSyntax)i.Accept(this)));
            whileStatement.Body.Statements.AddRange(incrementors);
            blocks_.Pop();
            body.Statements.Add(whileStatement);
            blocks_.Pop();

            return new LuaBlockBlockSyntax(body);
        }

        public override LuaSyntaxNode VisitDoStatement(DoStatementSyntax node) {
            var condition = (LuaExpressionSyntax)node.Condition.Accept(this);
            var newCondition = new LuaPrefixUnaryExpressionSyntax(new LuaParenthesizedExpressionSyntax(condition), LuaSyntaxNode.Tokens.Not);
            LuaRepeatStatementSyntax repeatStatement = new LuaRepeatStatementSyntax(newCondition);
            VisitLoopBody(node.Statement, repeatStatement.Body);
            return repeatStatement;
        }

        public override LuaSyntaxNode VisitYieldStatement(YieldStatementSyntax node) {
            CurFunction.HasYield = true;
            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            if(node.IsKind(SyntaxKind.YieldBreakStatement)) {
                LuaReturnStatementSyntax returnStatement = new LuaReturnStatementSyntax(expression);
                return returnStatement;
            }
            else {
                LuaInvocationExpressionSyntax invocationExpression = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.YieldReturn);
                invocationExpression.AddArgument(expression);
                return new LuaExpressionStatementSyntax(invocationExpression);
            }
        }

        public override LuaSyntaxNode VisitParenthesizedExpression(ParenthesizedExpressionSyntax node) {
            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            return new LuaParenthesizedExpressionSyntax(expression);
        }
        
        private bool MayBeNullOrFalse(ExpressionSyntax conditionalWhenTrue) {
            var type = semanticModel_.GetTypeInfo(conditionalWhenTrue).Type;
            bool mayBeNullOrFalse;
            if(type.IsValueType) {
                if(type.SpecialType == SpecialType.System_Boolean) {
                    var constValue = semanticModel_.GetConstantValue(conditionalWhenTrue);
                    if(constValue.HasValue && (bool)constValue.Value) {
                        mayBeNullOrFalse = false;
                    }
                    else {
                        mayBeNullOrFalse = true;
                    }
                }
                else {
                    mayBeNullOrFalse = false;
                }
            }
            else if(type.IsStringType()) {
                var constValue = semanticModel_.GetConstantValue(conditionalWhenTrue);
                if(constValue.HasValue) {
                    mayBeNullOrFalse = false;
                }
                else {
                    mayBeNullOrFalse = true;
                }
            }
            else {
                mayBeNullOrFalse = true;
            }
            return mayBeNullOrFalse;
        }

        /// <summary>
        /// http://lua-users.org/wiki/TernaryOperator
        /// </summary>
        public override LuaSyntaxNode VisitConditionalExpression(ConditionalExpressionSyntax node) {
            bool mayBeNullOrFalse = MayBeNullOrFalse(node.WhenTrue);
            if(mayBeNullOrFalse) {
                var temp = GetTempIdentifier(node);
                var condition = (LuaExpressionSyntax)node.Condition.Accept(this);
                LuaIfStatementSyntax ifStatement = new LuaIfStatementSyntax(condition);
                blocks_.Push(ifStatement.Body);
                var whenTrue = (LuaExpressionSyntax)node.WhenTrue.Accept(this);
                blocks_.Pop();
                ifStatement.Body.Statements.Add(new LuaExpressionStatementSyntax(new LuaAssignmentExpressionSyntax(temp, whenTrue)));

                LuaBlockSyntax block = new LuaBlockSyntax();
                blocks_.Push(block);
                var whenFalse = (LuaExpressionSyntax)node.WhenFalse.Accept(this);
                blocks_.Pop();
                block.Statements.Add(new LuaExpressionStatementSyntax(new LuaAssignmentExpressionSyntax(temp, whenFalse)));

                ifStatement.Else = new LuaElseClauseSyntax(block);
                CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(new LuaVariableDeclaratorSyntax(temp)));
                CurBlock.Statements.Add(ifStatement);
                return temp;
            }
            else {
                var condition = (LuaExpressionSyntax)node.Condition.Accept(this);
                var whenTrue = (LuaExpressionSyntax)node.WhenTrue.Accept(this);
                var whenFalse = (LuaExpressionSyntax)node.WhenFalse.Accept(this);
                return new LuaBinaryExpressionSyntax(new LuaBinaryExpressionSyntax(condition, LuaSyntaxNode.Tokens.And, whenTrue), LuaSyntaxNode.Tokens.Or, whenFalse);
            }
        }

        public override LuaSyntaxNode VisitGotoStatement(GotoStatementSyntax node) {
            if(node.CaseOrDefaultKeyword.IsKind(SyntaxKind.CaseKeyword)) {
                const string kCaseLabel = "caseLabel";
                var switchStatement = switchs_.Peek();
                int caseIndex = GetCaseLabelIndex(node);
                var labelIdentifier = switchStatement.CaseLabels.GetOrDefault(caseIndex);
                if(labelIdentifier == null) {
                    string uniqueName = GetUniqueIdentifier(kCaseLabel + caseIndex, node);
                    labelIdentifier = new LuaIdentifierNameSyntax(uniqueName);
                    switchStatement.CaseLabels.Add(caseIndex, labelIdentifier);
                }
                return new LuaGotoCaseAdapterStatement(labelIdentifier);
            }
            else if(node.CaseOrDefaultKeyword.IsKind(SyntaxKind.DefaultKeyword)) {
                const string kDefaultLabel = "defaultLabel";
                var switchStatement = switchs_.Peek();
                if(switchStatement.DefaultLabel == null) {
                    string identifier = GetUniqueIdentifier(kDefaultLabel, node);
                    switchStatement.DefaultLabel = new LuaIdentifierNameSyntax(identifier);
                }
                return new LuaGotoCaseAdapterStatement(switchStatement.DefaultLabel);
            }
            else {
                var identifier = (LuaIdentifierNameSyntax)node.Expression.Accept(this);
                return new LuaGotoStatement(identifier);
            }
        }

        public override LuaSyntaxNode VisitLabeledStatement(LabeledStatementSyntax node) {
            LuaIdentifierNameSyntax identifier = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
            var statement = (LuaStatementSyntax)node.Statement.Accept(this);
            return new LuaLabeledStatement(identifier, statement);
        }

        public override LuaSyntaxNode VisitEmptyStatement(EmptyStatementSyntax node) {
            return LuaStatementSyntax.Empty;
        }

        public override LuaSyntaxNode VisitCastExpression(CastExpressionSyntax node) {
            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            var typeExpression = (LuaExpressionSyntax)node.Type.Accept(this);
            return BuildBinaryInvokeExpression(expression, typeExpression, LuaIdentifierNameSyntax.Cast);
        }
    }
}