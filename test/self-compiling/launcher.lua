package.path = package.path .. ";../../CSharp.lua/Coresystem.lua/?.lua"

require("All")()          -- coresystem.lua/All.lua


local codeAnalysisTypes = { 
  "System.Collections.Concurrent.ConcurrentBag_1",
  "System.Collections.Concurrent.ConcurrentDictionary_2",
  "System.Collections.Immutable.ImmutableArray_1",
  "System.Collections.Immutable.ImmutableList_1",
  "System.IO.Stream",
  "System.Xml.Serialization",

  "Microsoft.CodeAnalysis.ISymbol", 
  "Microsoft.CodeAnalysis.IMethodSymbol", 
  "Microsoft.CodeAnalysis.IParameterSymbol", 
  "Microsoft.CodeAnalysis.ITypeSymbol", 
  "Microsoft.CodeAnalysis.INamedTypeSymbol", 
  "Microsoft.CodeAnalysis.INamespaceSymbol", 
  "Microsoft.CodeAnalysis.IEventSymbol", 
  "Microsoft.CodeAnalysis.IPropertySymbol", 
  "Microsoft.CodeAnalysis.Optional_1",
  "Microsoft.CodeAnalysis.SyntaxList_1",
  "Microsoft.CodeAnalysis.DocumentationProvider",
  
  "Microsoft.CodeAnalysis.CSharp.CSharpSyntaxWalker",
  "Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeSyntax",
  "Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax",
  "Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax",
}

for _, name in ipairs(codeAnalysisTypes) do
  System.define(name)
end

require("out.manifest")("out")