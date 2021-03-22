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
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;

using CSharpLua.LuaAst;

namespace CSharpLua {
  internal sealed class PartialTypeDeclaration : IComparable<PartialTypeDeclaration> {
    public INamedTypeSymbol Symbol;
    public TypeDeclarationSyntax Node;
    public LuaTypeDeclarationSyntax TypeDeclaration;
    public LuaCompilationUnitSyntax CompilationUnit;

    public int CompareTo(PartialTypeDeclaration other) {
      string filePath = CompilationUnit.FilePath;
      string otherFilePath = other.CompilationUnit.FilePath;

      if (filePath.Contains(otherFilePath)) {
        return 1;
      } else if (otherFilePath.Contains(filePath)) {
        return -1;
      } else {
        return other.Node.Members.Count.CompareTo(Node.Members.Count);
      }
    }
  }

  public sealed class LuaSyntaxGenerator {
    public sealed class SettingInfo {
      public bool HasSemicolon { get; set; }
      private int indent_;
      public string IndentString { get; private set; }
      public bool IsClassic { get; set; }
      public bool IsExportMetadata { get; set; }
      public string BaseFolder { get; set; } = "";
      public bool IsExportAttributesAll { get; private set; }
      public bool IsExportEnumAll { get; private set; }
      public bool IsModule { get; set; }
      public HashSet<string> ExportAttributes { get; private set; }
      public HashSet<string> ExportEnums { get; private set; }
      public HashSet<string> LuaModuleLibs;
      public bool IsInlineSimpleProperty { get; set; }
      public bool IsPreventDebugObject { get; set; }
      public bool IsNotConstantForEnum { get; set; }
      public bool IsNoConcurrent { get; set; }

      public SettingInfo() {
        Indent = 2;
      }

      public string[] Attributes {
        set {
          if (value != null) {
            if (value.Length == 0) {
              IsExportAttributesAll = true;
            } else {
              ExportAttributes = new HashSet<string>(value);
            }
          }
        }
      }

      public string[] Enums {
        set {
          if (value != null) {
            if (value.Length == 0) {
              IsExportEnumAll = true;
            } else {
              ExportEnums = new HashSet<string>(value);
            }
          }
        }
      }

      public int Indent {
        get {
          return indent_;
        }
        set {
          if (value > 0 && indent_ != value) {
            indent_ = value;
            IndentString = new string(' ', indent_);
          }
        }
      }
    }

    private const string kLuaSuffix = ".lua";
    private static readonly Encoding Encoding = new UTF8Encoding(false);

    private readonly CSharpCompilation compilation_;
    public XmlMetaProvider XmlMetaProvider { get; }
    public CSharpCommandLineArguments CommandLineArguments { get; }
    public SettingInfo Setting { get; set; }
    private bool IsConcurrent => !Setting.IsNoConcurrent;
    private readonly ConcurrentHashSet<string> exportEnums_ = new();
    private readonly ConcurrentHashSet<INamedTypeSymbol> ignoreExportTypes_ = new();
    private readonly ConcurrentHashSet<ISymbol> forcePublicSymbols_ = new();
    private readonly ConcurrentList<LuaEnumDeclarationSyntax> enumDeclarations_ = new();
    private readonly ConcurrentDictionary<INamedTypeSymbol, ConcurrentList<PartialTypeDeclaration>> partialTypes_ = new();
    private readonly ImmutableHashSet<string> monoBehaviourSpecialMethodNames_;
    private ImmutableList<LuaExpressionSyntax> assemblyAttributes_ = ImmutableList<LuaExpressionSyntax>.Empty;
    private readonly ConcurrentDictionary<INamedTypeSymbol, ConcurrentHashSet<INamedTypeSymbol>> genericImportDepends_ = new();
    private IMethodSymbol mainEntryPoint_;
    public INamedTypeSymbol SystemExceptionTypeSymbol { get; }
    private readonly INamedTypeSymbol monoBehaviourTypeSymbol_;

    static LuaSyntaxGenerator() {
      Contract.ContractFailed += (_, e) => {
        e.SetHandled();
        throw new ApplicationException(e.Message, e.OriginalException);
      };
    }

    private CSharpCompilationOptions WithOptions(CSharpCompilationOptions compilationOptions) {
      return compilationOptions
        .WithConcurrentBuild(IsConcurrent)
        .WithOutputKind(OutputKind.DynamicallyLinkedLibrary)
        .WithMetadataImportOptions(MetadataImportOptions.All);
    }

    private static SyntaxTree ParseText((string Text, string Path) code, CSharpParseOptions parseOptions) {
      return CSharpSyntaxTree.ParseText(code.Text, parseOptions, code.Path);
    }

    private static Task<SyntaxTree> BuildSyntaxTreeAsync((string Text, string Path) code, CSharpParseOptions parseOptions) {
      return Task.Factory.StartNew(o => ParseText(code, parseOptions), null);
    }

    private IEnumerable<SyntaxTree> BuildSyntaxTrees(IEnumerable<(string Text, string Path)> codes, CSharpParseOptions parseOptions) {
      if (IsConcurrent) {
        var tasks = codes.Select(i => BuildSyntaxTreeAsync(i, parseOptions));
        return Task.WhenAll(tasks).Result;
      } else {
        return codes.Select(i => ParseText(i, parseOptions));
      }
    }

    private (CSharpCompilation, CSharpCommandLineArguments) BuildCompilation(IEnumerable<(string Text, string Path)> codes, IEnumerable<Stream> libs, IEnumerable<string> cscArguments, SettingInfo setting) {
      var commandLineArguments = CSharpCommandLineParser.Default.Parse((cscArguments ?? Array.Empty<string>()).Concat(new string[] { "-define:__CSharpLua__" }), null, null);
      var parseOptions = commandLineArguments.ParseOptions.WithLanguageVersion(LanguageVersion.Preview).WithDocumentationMode(DocumentationMode.Parse);
      var syntaxTrees = BuildSyntaxTrees(codes, parseOptions);
      var references = libs.Select(i => MetadataReference.CreateFromStream(i)).ToList();
      var compilation = CSharpCompilation.Create("_", syntaxTrees, references, WithOptions(commandLineArguments.CompilationOptions));
      using (MemoryStream ms = new MemoryStream()) {
        EmitResult result = compilation.Emit(ms);
        if (!result.Success) {
          var errors = result.Diagnostics.Where(i => i.Severity == DiagnosticSeverity.Error);
          string message = string.Join("\n", errors);
          throw new CompilationErrorException(message);
        }
      }
      return (compilation, commandLineArguments);
    }

    private static IEnumerable<Stream> ToFileStreams(IEnumerable<string> paths) {
      return paths.Select(i => new FileStream(i, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
    }

    public LuaSyntaxGenerator(IEnumerable<(string Text, string Path)> codes, IEnumerable<string> libs, IEnumerable<string> cscArguments, IEnumerable<string> metas, SettingInfo setting)
      : this(codes, ToFileStreams(libs), cscArguments, ToFileStreams(metas), setting) {
    }

    public LuaSyntaxGenerator(IEnumerable<(string Text, string Path)> codes, IEnumerable<Stream> libs, IEnumerable<string> cscArguments, IEnumerable<Stream> metas, SettingInfo setting) {
      Setting = setting;
      (compilation_, CommandLineArguments) = BuildCompilation(codes, libs, cscArguments, setting);
      XmlMetaProvider = new XmlMetaProvider(metas);
      if (Setting.ExportEnums != null) {
        exportEnums_.UnionWith(Setting.ExportEnums);
      }
      SystemExceptionTypeSymbol = compilation_.GetTypeByMetadataName("System.Exception");
      if (compilation_.ReferencedAssemblyNames.Any(i => i.Name.Contains("UnityEngine"))) {
        monoBehaviourTypeSymbol_ = compilation_.GetTypeByMetadataName("UnityEngine.MonoBehaviour");
        if (monoBehaviourTypeSymbol_ != null) {
          monoBehaviourSpecialMethodNames_ = new string[] { "Awake", "Start", "Update", "FixedUpdate", "LateUpdate" }.ToImmutableHashSet();
        }
      }
      DoPretreatment();
    }

    private LuaCompilationUnitSyntax CreateCompilationUnit(SyntaxTree syntaxTree, bool isSingleFile) {
      var semanticModel = GetSemanticModel(syntaxTree);
      var compilationUnitSyntax = (CompilationUnitSyntax)syntaxTree.GetRoot();
      var transfor = new LuaSyntaxNodeTransform(this, semanticModel);
      return transfor.VisitCompilationUnit(compilationUnitSyntax, isSingleFile);
    }

    private Task<LuaCompilationUnitSyntax> CreateCompilationUnitAsync(SyntaxTree syntaxTree, bool isSingleFile) {
      return Task.Factory.StartNew(o => CreateCompilationUnit(syntaxTree, isSingleFile), null);
    }

    private IEnumerable<LuaCompilationUnitSyntax> Create(bool isSingleFile = false) {
      List<LuaCompilationUnitSyntax> luaCompilationUnits;
      if (IsConcurrent) {
        try {
          var tasks = compilation_.SyntaxTrees.Select(i => CreateCompilationUnitAsync(i, isSingleFile));
          luaCompilationUnits = Task.WhenAll(tasks).Result.ToList();
        } catch (AggregateException e) {
          if (e.InnerExceptions.Count > 0) {
            throw e.InnerExceptions.First();
          } else {
            throw e;
          }
        }
      } else {
        luaCompilationUnits = compilation_.SyntaxTrees.Select(i => CreateCompilationUnit(i, isSingleFile)).ToList();
      }

      CheckExportEnums();
      CheckPartialTypes();
      CheckRefactorNames();
      return luaCompilationUnits.Where(i => !i.IsEmpty);
    }

    private void Write(LuaCompilationUnitSyntax luaCompilationUnit, TextWriter writer) {
      LuaRenderer rener = new LuaRenderer(this, writer);
      luaCompilationUnit.Render(rener);
    }

    private void Write(LuaCompilationUnitSyntax luaCompilationUnit, string outFile) {
      using var writer = new StreamWriter(outFile, false, Encoding);
      Write(luaCompilationUnit, writer);
    }

    public void Generate(string outFolder) {
      List<string> modules = new List<string>();
      foreach (var luaCompilationUnit in Create()) {
        string outFile = GetOutFileAbsolutePath(luaCompilationUnit.FilePath, outFolder, out string module);
        Write(luaCompilationUnit, outFile);
        modules.Add(module);
      }
      ExportManifestFile(modules, outFolder);
    }

    public void GenerateSingleFile(string outFile, string outFolder, IEnumerable<string> luaSystemLibs) {
      outFile = GetOutFileRelativePath(outFile, outFolder, out _);
      using var streamWriter = new StreamWriter(outFile, false, Encoding);
      streamWriter.WriteLine("CSharpLuaSingleFile = true");
      bool isFirst = true;
      foreach (var luaSystemLib in luaSystemLibs) {
        WriteLuaSystemLib(luaSystemLib, streamWriter, isFirst);
        isFirst = false;
      }
      streamWriter.WriteLine();
      streamWriter.WriteLine(LuaSyntaxNode.Tokens.ShortComment + LuaCompilationUnitSyntax.GeneratedMarkString);
      foreach (var luaCompilationUnit in Create(true)) {
        WriteCompilationUnit(luaCompilationUnit, streamWriter);
      }
      WriteSingleFileManifest(streamWriter);
    }

    private static string GetSystemLibName(string path) {
      const string begin = "CoreSystem";
      int index = path.LastIndexOf(begin);
      return path.Substring(index + begin.Length + 1);
    }

    private static void RemoveLicenseComments(ref string code) {
      const string kBegin = "--[[";
      const string kEnd = "--]]";
      int i = code.IndexOf(kBegin);
      if (i != -1) {
        bool isSpace = code.Take(i).All(char.IsWhiteSpace);
        if (isSpace) {
          int j = code.IndexOf(kEnd, i + kBegin.Length);
          Contract.Assert(j != -1);
          code = code.Substring(j + kEnd.Length).Trim();
        }
      }
    }

    private void WriteLuaSystemLib(string filePath, TextWriter writer, bool isFirst) {
      writer.WriteLine();
      writer.WriteLine("-- CoreSystemLib: {0}", GetSystemLibName(filePath));
      writer.WriteLine(LuaSyntaxNode.Keyword.Do);
      string code = File.ReadAllText(filePath);
      if (!isFirst) {
        RemoveLicenseComments(ref code);
      }
      writer.WriteLine(code);
      writer.WriteLine(LuaSyntaxNode.Keyword.End);
    }

    private void WriteCompilationUnit(LuaCompilationUnitSyntax luaCompilationUnit, TextWriter writer) {
      writer.WriteLine(LuaSyntaxNode.Keyword.Do);
      Write(luaCompilationUnit, writer);
      writer.WriteLine();
      writer.WriteLine(LuaSyntaxNode.Keyword.End);
    }

    private void WriteSingleFileManifest(TextWriter writer) {
      var types = GetExportTypes();
      if (types.Count > 0) {
        LuaTableExpression typeTable = new LuaTableExpression();
        typeTable.Add("types", new LuaTableExpression(types.Select(i => new LuaStringLiteralExpressionSyntax(GetTypeShortName(i)))));
        LuaCompilationUnitSyntax luaCompilationUnit = new LuaCompilationUnitSyntax(hasGeneratedMark: false);
        luaCompilationUnit.AddStatement(LuaIdentifierNameSyntax.SystemInit.Invocation(typeTable));
        if (mainEntryPoint_ != null) {
          luaCompilationUnit.AddStatement(LuaBlankLinesStatement.One);
          var methodName = mainEntryPoint_.Name;
          var methodTypeName = GetTypeName(mainEntryPoint_.ContainingType);
          var entryPointInvocation = new LuaInvocationExpressionSyntax(methodTypeName.MemberAccess(methodName));
          luaCompilationUnit.AddStatement(entryPointInvocation);
        }
        Write(luaCompilationUnit, writer);
        writer.WriteLine();
      }
    }

    public string GenerateSingle() {
      foreach (var luaCompilationUnit in Create()) {
        StringBuilder sb = new StringBuilder();
        using (var writer = new StringWriter(sb)) {
          Write(luaCompilationUnit, writer);
        }
        return sb.ToString();
      }
      throw new InvalidProgramException();
    }

    internal string RemoveBaseFolder(string path) {
      if (!string.IsNullOrEmpty(Setting.BaseFolder)) {
        return path.Remove(0, Setting.BaseFolder.Length).TrimStart(Path.DirectorySeparatorChar, '/');
      }
      return Path.GetFileName(path);
    }

    private string GetOutFileAbsolutePath(string inFilePath, string output_, out string module) {
      return GetOutFileRelativePath(RemoveBaseFolder(inFilePath), output_, out module);
    }

    private string GetOutFileRelativePath(string path, string output_, out string module) {
      string extend = Path.GetExtension(path);
      path = path.Remove(path.Length - extend.Length, extend.Length);
      path = path.Replace('.', '_');
      string outPath = Path.Combine(output_, path + kLuaSuffix);
      string dir = Path.GetDirectoryName(outPath);
      if (!Directory.Exists(dir)) {
        Directory.CreateDirectory(dir);
      }
      module = path.Replace(Path.DirectorySeparatorChar, '.');
      return outPath;
    }

    internal bool IsCheckedOverflow {
      get {
        return compilation_.Options.CheckOverflow;
      }
    }

    internal bool IsEnumExport(string enumTypeSymbol) {
      if (Setting.IsExportEnumAll) {
        return true;
      }
      return exportEnums_.Contains(enumTypeSymbol);
    }

    internal void AddExportEnum(ITypeSymbol enumType) {
      Contract.Assert(enumType.TypeKind == TypeKind.Enum);
      if (enumType.IsFromCode()) {
        exportEnums_.Add(enumType.ToString());
      }
    }

    internal bool IsConstantEnum(ITypeSymbol enumType) { 
      if (enumType.TypeKind == TypeKind.Enum) {
        bool isNot = Setting.IsNotConstantForEnum && IsTypeEnableExport(enumType);
        return !isNot;
      }
      return false;
    }

    internal void AddEnumDeclaration(INamedTypeSymbol type, LuaEnumDeclarationSyntax enumDeclaration) {
      if (type.IsProtobufNetDeclaration()) {
        AddExportEnum(type);      // protobuf-net enum is always export
      }
      enumDeclarations_.Add(enumDeclaration);
    }

    internal void AddIgnoreExportType(INamedTypeSymbol type) {
      ignoreExportTypes_.Add(type);
    }

    internal void AddForcePublicSymbol(ISymbol symbol) {
      forcePublicSymbols_.Add(symbol.OriginalDefinition);
    }

    internal bool IsForcePublicSymbol(ISymbol symbol) {
      return forcePublicSymbols_.Contains(symbol.OriginalDefinition);
    }

    private static readonly HashSet<string> ignoreSystemAttributes_ = new HashSet<string>() {
      "System.AttributeUsageAttribute",
      "System.ComponentModel.BrowsableAttribute",
      "System.Diagnostics.ConditionalAttribute",
      "System.Runtime.Serialization.CollectionDataContractAttribute"
    };

    internal bool IsExportAttribute(INamedTypeSymbol symbol) {
      string name = symbol.ToString();
      bool isExport = false;
      if (Setting.IsExportAttributesAll) {
        isExport = true; ;
      } else if (Setting.ExportAttributes != null && Setting.ExportAttributes.Contains(name)) {
        isExport = true;
      }
      if (isExport) {
        if (ignoreSystemAttributes_.Contains(name)) {
          isExport = false;
        }
      } else if (symbol.IsAssemblyAttribute()) {
        isExport = true;
      }
      return isExport;
    }

    internal bool IsFromLuaModule(ISymbol symbol) {
      return symbol.IsFromCode() || IsFromModuleOnly(symbol);
    }

    private bool IsFromModuleOnly(ISymbol symbol) {
      var luaModuleLibs = Setting.LuaModuleLibs;
      return luaModuleLibs != null && luaModuleLibs.Contains(symbol.ContainingAssembly.Name);
    }

    internal bool IsConditionalAttributeIgnore(ISymbol symbol) {
      foreach (var attrbute in symbol.GetAttributes()) {
        var attributeSymbol = attrbute.AttributeClass;
        if (attributeSymbol.IsConditionalAttribute()) {
          string conditionString = (string)attrbute.ConstructorArguments.First().Value;
          return !CommandLineArguments.ParseOptions.PreprocessorSymbolNames.Contains(conditionString);
        }
      }
      return false;
    }

    private void CheckExportEnums() {
      foreach (var enumDeclaration in enumDeclarations_) {
        if (IsEnumExport(enumDeclaration.FullName)) {
          enumDeclaration.IsExport = true;
          enumDeclaration.CompilationUnit.AddTypeDeclarationCount();
        }
      }
    }

    internal void AddPartialTypeDeclaration(INamedTypeSymbol typeSymbol, TypeDeclarationSyntax node, LuaTypeDeclarationSyntax luaNode, LuaCompilationUnitSyntax compilationUnit) {
      var list = partialTypes_.GetOrAdd(typeSymbol, _ => new ConcurrentList<PartialTypeDeclaration>());
      list.Add(new PartialTypeDeclaration() {
        Symbol = typeSymbol,
        Node = node,
        TypeDeclaration = luaNode,
        CompilationUnit = compilationUnit,
      });
    }

    private void CheckPartialTypes() {
      while (partialTypes_.Count > 0) {
        var types = partialTypes_.Values.ToArray();
        partialTypes_.Clear();
        foreach (var typeDeclarations in types) {
          var major = typeDeclarations.Min();
          var transfor = new LuaSyntaxNodeTransform(this, null);
          transfor.AcceptPartialType(major, typeDeclarations);
        }
      }
    }

    internal SemanticModel GetSemanticModel(SyntaxTree syntaxTree) {
      return compilation_.GetSemanticModel(syntaxTree);
    }

    internal bool IsBaseType(BaseTypeSyntax type) {
      var syntaxTree = type.SyntaxTree;
      SemanticModel semanticModel = GetSemanticModel(syntaxTree);
      var symbol = semanticModel.GetTypeInfo(type.Type).Type;
      Contract.Assert(symbol != null);
      return symbol.TypeKind != TypeKind.Interface;
    }

    private bool IsTypeEnableExport(ITypeSymbol type) {
      bool isExport = true;
      if (type.TypeKind == TypeKind.Enum) {
        isExport = IsEnumExport(type.ToString());
      }
      if (ignoreExportTypes_.Contains(type)) {
        isExport = false;
      }
      return isExport;
    }

    private void AddSuperTypeTo(HashSet<INamedTypeSymbol> parentTypes, INamedTypeSymbol rootType, INamedTypeSymbol superType) {
      if (superType.IsGenericType) {
        if (superType.OriginalDefinition.IsFromCode()) {
          parentTypes.Add(superType.OriginalDefinition);
        }
        foreach (var typeArgument in superType.TypeArguments) {
          if (typeArgument.Kind != SymbolKind.TypeParameter) {
            if (typeArgument.OriginalDefinition.IsFromCode() && !typeArgument.OriginalDefinition.Is(rootType)) {
              AddSuperTypeTo(parentTypes, rootType, (INamedTypeSymbol)typeArgument);
            }
          }
        }
      } else if (superType.IsFromCode()) {
        parentTypes.Add(superType);
      }
    }

    private List<INamedTypeSymbol> GetExportTypes() {
      List<INamedTypeSymbol> allTypes = new List<INamedTypeSymbol>();
      if (types_.Count > 0) {
        types_.Sort((x, y) => x.ToString().CompareTo(y.ToString()));

        List<List<INamedTypeSymbol>> typesList = new List<List<INamedTypeSymbol>>() { types_ };
        while (true) {
          HashSet<INamedTypeSymbol> parentTypes = new HashSet<INamedTypeSymbol>();
          var lastTypes = typesList.Last();
          foreach (var type in lastTypes) {
            if (type.ContainingType != null) {
              AddSuperTypeTo(parentTypes, type, type.ContainingType);
            }

            if (type.BaseType != null) {
              AddSuperTypeTo(parentTypes, type, type.BaseType);
            }

            foreach (var interfaceType in type.Interfaces) {
              AddSuperTypeTo(parentTypes, type, interfaceType);
            }

            AddGenericImportDependTo(parentTypes, type);
          }

          if (parentTypes.Count == 0) {
            break;
          }

          typesList.Add(parentTypes.ToList());
        }

        typesList.Reverse();
        var types = typesList.SelectMany(i => i).Distinct().Where(IsTypeEnableExport);
        allTypes.AddRange(types);
      }
      return allTypes;
    }

    public void SetMainEntryPoint(IMethodSymbol symbol, SyntaxNode node) {
      if (mainEntryPoint_ != null) {
        throw new CompilationErrorException(node, "has more than one entry point");
      }
      mainEntryPoint_ = symbol;
    }

    public void WithAssemblyAttributes(List<LuaExpressionSyntax> attributes) {
      assemblyAttributes_ = assemblyAttributes_.AddRange(attributes);
    }

    private void ExportManifestFile(List<string> moduleFiles, string outFolder) {
      if (moduleFiles.Count > 0) {
        moduleFiles.Sort();
        var types = GetExportTypes();
        if (types.Count > 0) {
          var t = new LuaTableExpression();
          t.Add("path", "path");
          t.Add("files", new LuaTableExpression(moduleFiles.Select(i => new LuaStringLiteralExpressionSyntax(i))));
          t.Add("types", new LuaTableExpression(types.Select(i => new LuaStringLiteralExpressionSyntax(GetTypeShortName(i)))));
          FillManifestInitConf(t);
          var functionExpression = new LuaFunctionExpressionSyntax();
          functionExpression.AddParameter("path");
          functionExpression.AddStatement(new LuaReturnStatementSyntax(LuaIdentifierNameSyntax.SystemInit.Invocation(t)));
          var luaCompilationUnit = new LuaCompilationUnitSyntax();
          luaCompilationUnit.AddStatement(new LuaReturnStatementSyntax(functionExpression));
          string outFile = Path.Combine(outFolder, "manifest.lua");
          Write(luaCompilationUnit, outFile);
        }
      }
    }

    private void FillManifestInitConf(LuaTableExpression t) {
      if (mainEntryPoint_ != null) {
        LuaIdentifierNameSyntax name = mainEntryPoint_.Name;
        var typeName = GetTypeName(mainEntryPoint_.ContainingType);
        var identifier = new LuaSymbolNameSyntax(typeName.MemberAccess(name));
        t.Add(name, new LuaStringLiteralExpressionSyntax(identifier));
      }

      LuaTableExpression assemblyTable = new LuaTableExpression();
      string moduleName = compilation_.Options.ModuleName;
      if (!string.IsNullOrEmpty(moduleName)) {
        assemblyTable.Add("name", new LuaStringLiteralExpressionSyntax(moduleName.TrimEnd(".dll")));
      }

      bool hasNormalAttribute = false;
      if (assemblyAttributes_.Count > 0) {
        const string kAssemblyFields = "System.Reflection.Assembly";
        foreach (var attribute in assemblyAttributes_) {
          var invocation = (LuaInvocationExpressionSyntax)attribute;
          if (invocation.Expression is LuaIdentifierNameSyntax identifierName) {
            string type = identifierName.ValueText;
            if (type.StartsWith(kAssemblyFields)) {
              int index = kAssemblyFields.Length;
              int count = type.Length - index - "Attribute".Length;
              string field = type.Substring(index, count);
              if (invocation.ArgumentList.Arguments.Count == 1) {
                assemblyTable.Add(field, invocation.ArgumentList.Arguments.First());
              } else {
                assemblyTable.Add(field, new LuaTableExpression(invocation.ArgumentList.Arguments) { IsSingleLine = true });
              }
              continue;
            }
          }
          assemblyTable.Add(invocation);
          hasNormalAttribute = true;
        }
      }

      if (assemblyTable.Items.Count > 0) {
        const string kAssembly = "assembly";
        if (!hasNormalAttribute) {
          t.Add(kAssembly, assemblyTable);
        } else {
          var function = new LuaFunctionExpressionSyntax();
          function.AddParameter(LuaIdentifierNameSyntax.Global);
          function.AddStatement(new LuaReturnStatementSyntax(assemblyTable));
          t.Add(kAssembly, function);
        }
      }
    }

    private void AddGenericImportDependTo(HashSet<INamedTypeSymbol> parentTypes, INamedTypeSymbol type) {
      var set = genericImportDepends_.GetOrDefault(type);
      if (set != null) {
        parentTypes.UnionWith(set);
      }
    }

    internal void AddGenericImportDepend(INamedTypeSymbol definition, INamedTypeSymbol type) {
      if (type != null && type.IsFromCode() && !definition.IsContainsInternalSymbol(type)) {
        var set = genericImportDepends_.GetOrAdd(definition, i => new ConcurrentHashSet<INamedTypeSymbol>());
        set.Add(type);
      }
    }

    #region     // member name refactor

    private readonly List<INamedTypeSymbol> types_ = new();
    private readonly Dictionary<INamedTypeSymbol, HashSet<INamedTypeSymbol>> extends_ = new();
    private readonly Dictionary<INamedTypeSymbol, HashSet<INamedTypeSymbol>> implicitExtends_ = new();

    private readonly Dictionary<ISymbol, LuaSymbolNameSyntax> memberNames_ = new();
    private readonly Dictionary<INamedTypeSymbol, HashSet<string>> typeNameUseds_ = new();
    private readonly HashSet<ISymbol> refactorNames_ = new();
    private readonly Dictionary<ISymbol, string> memberIllegalNames_ = new();

    internal bool IsNeedRefactorName(ISymbol symbol) => refactorNames_.Contains(symbol);
    private bool IsImplicitExtend(INamedTypeSymbol super, INamedTypeSymbol children) => implicitExtends_.GetOrDefault(super)?.Contains(children) ?? false;

    internal void AddTypeSymbol(INamedTypeSymbol typeSymbol) {
      types_.Add(typeSymbol);
      CheckExtends(typeSymbol);
    }

    private void CheckExtends(INamedTypeSymbol typeSymbol) {
      if (typeSymbol.SpecialType != SpecialType.System_Object) {
        if (typeSymbol.BaseType != null) {
          var super = typeSymbol.BaseType;
          TryAddExtend(super, typeSymbol);
        }
      }

      foreach (INamedTypeSymbol super in typeSymbol.AllInterfaces) {
        TryAddExtend(super, typeSymbol);
      }
    }

    private void TryAddExtend(INamedTypeSymbol super, INamedTypeSymbol children, bool isImplicit = false) {
      if (super.IsFromCode()) {
        if (super.IsGenericType) {
          super = super.OriginalDefinition;
        }
        extends_.TryAdd(super, children);
        if (isImplicit) {
          implicitExtends_.TryAdd(super, children);
        }
      }
    }

    internal LuaIdentifierNameSyntax GetMemberName(ISymbol symbol) {
      Utility.CheckOriginalDefinition(ref symbol);
      var name = memberNames_.GetOrDefault(symbol);
      if (name == null) {
        lock(memberNames_) {
          name = memberNames_.GetOrAdd(symbol, symbol => {
            var identifierName = InternalGetMemberName(symbol);
            CheckMemberBadName(identifierName.ValueText, symbol);
            return new LuaSymbolNameSyntax(identifierName);
          });
        }
      }
      return name;
    }

    private void CheckMemberBadName(string originalString, ISymbol symbol) {
      if (symbol.IsFromCode()) {
        bool isCheckNeedReserved = false;
        bool isCheckIllegalIdentifier = true;
        switch (symbol.Kind) {
          case SymbolKind.Field:
          case SymbolKind.Method:
            isCheckNeedReserved = true;
            break;

          case SymbolKind.Property:
            var propertySymbol = (IPropertySymbol)symbol;
            if (propertySymbol.IsIndexer) {
              isCheckIllegalIdentifier = false;
            } else {
              isCheckNeedReserved = true;
            }
            break;

          case SymbolKind.Event:
            if (IsEventFiled((IEventSymbol)symbol)) {
              isCheckNeedReserved = true;
            }
            break;
        }

        if (isCheckNeedReserved) {
          if (LuaSyntaxNode.IsMethodReservedWord(originalString)) {
            refactorNames_.Add(symbol);
            isCheckIllegalIdentifier = false;
          }
        }

        if (isCheckIllegalIdentifier) {
          if (Utility.IsIdentifierIllegal(ref originalString)) {
            refactorNames_.Add(symbol);
            memberIllegalNames_.Add(symbol, originalString);
          }
        }
      }
    }

    private LuaIdentifierNameSyntax InternalGetMemberName(ISymbol symbol) {
      if (symbol.Kind == SymbolKind.Method) {
        string name = XmlMetaProvider.GetMethodMapName((IMethodSymbol)symbol);
        if (name != null) {
          return name;
        }
      }

      if (!IsFromLuaModule(symbol)) {
        return GetSymbolBaseName(symbol);
      }

      if (symbol.IsStatic) {
        if (symbol.ContainingType.IsStatic) {
          return GetStaticClassMemberName(symbol);
        }
      }

      while (symbol.IsOverride) {
        var overriddenSymbol = symbol.OverriddenSymbol();
        symbol = overriddenSymbol;
      }

      return GetAllTypeSameName(symbol);
    }

    private static bool IsSameNameSymbol(ISymbol member, ISymbol symbol) {
      if (member.EQ(symbol)) {
        return true;
      }

      if (symbol.Kind == SymbolKind.Method) {
        var methodSymbol = (IMethodSymbol)symbol;
        if (methodSymbol.PartialDefinitionPart != null && methodSymbol.PartialDefinitionPart.EQ(member)) {
          return true;
        }
      }

      return false;
    }

    private LuaIdentifierNameSyntax GetAllTypeSameName(ISymbol symbol) {
      List<ISymbol> sameNameMembers = GetSameNameMembers(symbol);
      LuaIdentifierNameSyntax symbolExpression = null;
      int index = 0;
      foreach (ISymbol member in sameNameMembers) {
        if (IsSameNameSymbol(member, symbol)) {
          symbolExpression = GetSymbolBaseName(symbol);
        } else {
          if (!memberNames_.ContainsKey(member)) {
            LuaIdentifierNameSyntax identifierName = GetSymbolBaseName(member);
            memberNames_.Add(member, new LuaSymbolNameSyntax(identifierName));
          }
        }
        if (index > 0) {
          ISymbol refactorSymbol = member;
          Utility.CheckOriginalDefinition(ref refactorSymbol);
          refactorNames_.Add(refactorSymbol);
        }
        ++index;
      }
      if (symbolExpression == null) {
        throw new InvalidOperationException();
      }
      return symbolExpression;
    }

    internal LuaIdentifierNameSyntax AddInnerName(ISymbol symbol) {
      string name = GetSymbolBaseName(symbol);
      LuaSymbolNameSyntax symbolName = new LuaSymbolNameSyntax(name);
      bool success = propertyOrEvnetInnerFieldNames_.TryAdd(symbol, symbolName);
      Contract.Assert(success);
      return symbolName;
    }

    private string GetSymbolBaseName(ISymbol symbol) {
      switch (symbol.Kind) {
        case SymbolKind.Method: {
          IMethodSymbol method = (IMethodSymbol)symbol;
          string name = XmlMetaProvider.GetMethodMapName(method);
          if (name != null) {
            return name;
          }
          var implementation = method.ExplicitInterfaceImplementations.FirstOrDefault();
          if (implementation != null) {
            return implementation.Name;
          }
          break;
        }
        case SymbolKind.Property: {
          IPropertySymbol property = (IPropertySymbol)symbol;
          if (property.IsIndexer) {
            return string.Empty;
          } else {
            var implementation = property.ExplicitInterfaceImplementations.FirstOrDefault();
            if (implementation != null) {
              return implementation.Name;
            }
          }
          break;
        }
        case SymbolKind.Event: {
          IEventSymbol eventSymbol = (IEventSymbol)symbol;
          var implementation = eventSymbol.ExplicitInterfaceImplementations.FirstOrDefault();
          if (implementation != null) {
            return implementation.Name;
          }
          break;
        }
      }
      return symbol.Name;
    }

    private LuaIdentifierNameSyntax GetStaticClassMemberName(ISymbol symbol) {
      var sameNameMembers = GetStaticClassSameNameMembers(symbol);
      LuaIdentifierNameSyntax symbolExpression = null;

      int index = 0;
      foreach (ISymbol member in sameNameMembers) {
        LuaIdentifierNameSyntax identifierName = GetMethodNameFromIndex(symbol, index);
        if (member.EQ(symbol)) {
          symbolExpression = identifierName;
        } else {
          if (!memberNames_.ContainsKey(member)) {
            memberNames_.Add(member, new LuaSymbolNameSyntax(identifierName));
          }
        }
        ++index;
      }

      if (symbolExpression == null) {
        throw new InvalidOperationException();
      }
      return symbolExpression;
    }

    private LuaIdentifierNameSyntax GetMethodNameFromIndex(ISymbol symbol, int index) {
      Contract.Assert(index != -1);
      if (index == 0) {
        return symbol.Name;
      } else {
        while (true) {
          string newName = symbol.Name + index;
          if (IsCurTypeNameEnable(symbol.ContainingType, newName)) {
            TryAddNewUsedName(symbol.ContainingType, newName);
            return newName;
          }
          ++index;
        }
      }
    }

    private bool TryAddNewUsedName(INamedTypeSymbol type, string newName) {
      return typeNameUseds_.TryAdd(type, newName);
    }

    internal string GetUniqueNameInType(INamedTypeSymbol type, string name, Func<string, bool> checker) {
      int index = 0;
      while (true) {
        string newName = index != 0 ? name + index : name;
        if (IsCurTypeNameEnable(type, newName) && checker(newName)) {
          TryAddNewUsedName(type, name);
          return newName;
        }
        ++index;
      }
    }

    private List<ISymbol> GetStaticClassSameNameMembers(ISymbol symbol) {
      List<ISymbol> members = new List<ISymbol>();
      var names = GetSymbolNames(symbol);
      AddSimilarNameMembers(symbol.ContainingType, names, members);
      return members;
    }

    private List<ISymbol> GetSameNameMembers(ISymbol symbol) {
      List<ISymbol> members = new List<ISymbol>();
      var names = GetSymbolNames(symbol);
      var rootType = symbol.ContainingType;
      var curTypeSymbol = rootType;
      while (true) {
        AddSimilarNameMembers(curTypeSymbol, names, members, !rootType.EQ(curTypeSymbol));
        var baseTypeSymbol = curTypeSymbol.BaseType;
        if (baseTypeSymbol != null) {
          curTypeSymbol = baseTypeSymbol;
        } else {
          break;
        }
      }
      members.Sort(MemberSymbolComparison);
      return members;
    }

    private void AddSimilarNameMembers(INamedTypeSymbol typeSymbol, List<string> names, List<ISymbol> outList, bool isWithoutPrivate = false) {
      foreach (var member in typeSymbol.GetMembers()) {
        if (member.IsOverride) {
          continue;
        }

        if (!isWithoutPrivate || !member.IsPrivate()) {
          var memberNames = GetSymbolNames(member);
          if (memberNames.Exists(i => names.Contains(i))) {
            outList.Add(member);
          }
        }
      }
    }

    private List<string> GetSymbolNames(ISymbol symbol) {
      List<string> names = new List<string>();
      if (symbol.Kind == SymbolKind.Property) {
        var propertySymbol = (IPropertySymbol)symbol;
        if (IsPropertyField(propertySymbol)) {
          names.Add(symbol.Name);
        } else {
          string baseName = GetSymbolBaseName(symbol);
          if (propertySymbol.IsReadOnly) {
            names.Add(LuaSyntaxNode.Tokens.Get + baseName);
          } else if (propertySymbol.IsWriteOnly) {
            names.Add(LuaSyntaxNode.Tokens.Set + baseName);
          } else {
            names.Add(LuaSyntaxNode.Tokens.Get + baseName);
            names.Add(LuaSyntaxNode.Tokens.Set + baseName);
          }
        }
      } else if (symbol.Kind == SymbolKind.Event) {
        var eventSymbol = (IEventSymbol)symbol;
        if (IsEventFiled(eventSymbol)) {
          names.Add(symbol.Name);
        } else {
          string baseName = GetSymbolBaseName(symbol);
          names.Add(LuaSyntaxNode.Tokens.Add + baseName);
          names.Add(LuaSyntaxNode.Tokens.Remove + baseName);
        }
      } else {
        names.Add(GetSymbolBaseName(symbol));
      }
      return names;
    }

    private bool MemberSymbolBoolComparison(ISymbol a, ISymbol b, Func<ISymbol, bool> boolFunc, out int v) {
      bool boolOfA = boolFunc(a);
      bool boolOfB = boolFunc(b);

      if (boolOfA) {
        if (boolOfB) {
          v = MemberSymbolCommonComparison(a, b);
        } else {
          v = -1;
        }
        return true;
      }

      if (b.IsAbstract) {
        v = 1;
        return true;
      }

      v = 0;
      return false;
    }

    private int MemberSymbolComparison(ISymbol a, ISymbol b) {
      if (a.EQ(b)) {
        return 0;
      }

      bool isFromCodeOfA = IsFromLuaModule(a.ContainingType);
      bool isFromCodeOfB = IsFromLuaModule(b.ContainingType);

      if (!isFromCodeOfA) {
        if (!isFromCodeOfB) {
          return 0;
        } else {
          return -1;
        }
      }

      if (!isFromCodeOfB) {
        return 1;
      }

      int countOfA = AllInterfaceImplementationsCount(a);
      int countOfB = AllInterfaceImplementationsCount(b);
      if (countOfA > 0 || countOfB > 0) {
        if (countOfA != countOfB) {
          return countOfA > countOfB ? -1 : 1;
        }

        if (countOfA == 1) {
          var implementationOfA = a.InterfaceImplementations().First();
          var implementationOfB = b.InterfaceImplementations().First();
          if (implementationOfA.EQ(implementationOfB)) {
            throw new CompilationErrorException($"{a} is conflict with {b}");
          }

          if (MemberSymbolBoolComparison(implementationOfA, implementationOfB, i => !i.IsExplicitInterfaceImplementation(), out int result)) {
            return result;
          }
        }

        return MemberSymbolCommonComparison(a, b);
      }

      if (MemberSymbolBoolComparison(a, b, i => i.IsAbstract, out var v)) {
        return v;
      }
      if (MemberSymbolBoolComparison(a, b, i => i.IsVirtual, out v)) {
        return v;
      }
      if (MemberSymbolBoolComparison(a, b, i => i.IsOverride, out v)) {
        return v;
      }

      return MemberSymbolCommonComparison(a, b);
    }

    private int MemberSymbolCommonComparison(ISymbol a, ISymbol b) {
      if (a.ContainingType.EQ(b.ContainingType)) {
        var type = a.ContainingType;
        var names = GetSymbolNames(a);
        List<ISymbol> members = new List<ISymbol>();
        AddSimilarNameMembers(type, names, members);
        int indexOfA = members.IndexOf(a);
        Contract.Assert(indexOfA != -1);
        int indexOfB = members.IndexOf(b);
        if (indexOfB == -1) {
          var m = a.ContainingType.GetMembers();
          indexOfA = m.IndexOf(a);
          indexOfB = m.IndexOf(b);
        }
        Contract.Assert(indexOfA != indexOfB);
        return indexOfA.CompareTo(indexOfB);
      } else {
        bool isSubclassOf = a.ContainingType.IsSubclassOf(b.ContainingType);
        return isSubclassOf ? 1 : -1;
      }
    }

    private void CheckRefactorNames() {
      HashSet<ISymbol> alreadyRefactorSymbols = new HashSet<ISymbol>();
      foreach (ISymbol symbol in refactorNames_) {
        if (symbol.ContainingType.TypeKind == TypeKind.Interface) {
          RefactorInterfaceSymbol(symbol, alreadyRefactorSymbols);
        } else {
          bool hasImplementation = false;
          foreach (ISymbol implementation in AllInterfaceImplementations(symbol)) {
            hasImplementation = RefactorInterfaceSymbol(implementation, alreadyRefactorSymbols);
          }

          if (!hasImplementation) {
            RefactorCurTypeSymbol(symbol, alreadyRefactorSymbols);
          }
        }
      }

      CheckRefactorInnerNames();
    }

    private void RefactorCurTypeSymbol(ISymbol symbol, HashSet<ISymbol> alreadyRefactorSymbols) {
      INamedTypeSymbol typeSymbol = symbol.ContainingType;
      var childrens = extends_.GetOrDefault(typeSymbol);
      string newName = GetRefactorName(typeSymbol, childrens, symbol);
      RefactorName(symbol, newName, alreadyRefactorSymbols);
    }

    private bool RefactorInterfaceSymbol(ISymbol symbol, HashSet<ISymbol> alreadyRefactorSymbols) {
      if (symbol.IsFromCode()) {
        INamedTypeSymbol typeSymbol = symbol.ContainingType;
        Contract.Assert(typeSymbol.TypeKind == TypeKind.Interface);
        var childrens = extends_.GetOrDefault(typeSymbol);
        string newName = GetRefactorName(null, childrens, symbol);
        if (childrens != null) {
          foreach (INamedTypeSymbol children in childrens) {
            if (children.TypeKind != TypeKind.Interface) {
              ISymbol implementationSymbol;
              if (!IsImplicitExtend(typeSymbol, children)) {
                implementationSymbol = children.FindImplementationForInterfaceMember(symbol);
                Contract.Assert(implementationSymbol != null);
              } else {
                implementationSymbol = FindImplicitImplementationForInterfaceMember(children, symbol);
              }
              if (implementationSymbol != null) {
                RefactorName(implementationSymbol, newName, alreadyRefactorSymbols);
              }
            }
          }
        }
        if (memberNames_.ContainsKey(symbol)) {
          RefactorName(symbol, newName, alreadyRefactorSymbols);
        }
        return true;
      }
      return false;
    }

    private void RefactorName(ISymbol symbol, string newName, HashSet<ISymbol> alreadyRefactorSymbols) {
      if (!alreadyRefactorSymbols.Contains(symbol)) {
        if (symbol.IsOverridable()) {
          RefactorChildrensOverridden(symbol, symbol.ContainingType, newName, alreadyRefactorSymbols);
        }
        UpdateName(symbol, newName, alreadyRefactorSymbols);
      }
    }

    private void RefactorChildrensOverridden(ISymbol originalSymbol, INamedTypeSymbol curType, string newName, HashSet<ISymbol> alreadyRefactorSymbols) {
      var childrens = extends_.GetOrDefault(curType);
      if (childrens != null) {
        foreach (INamedTypeSymbol children in childrens) {
          var curSymbol = children.GetMembers(originalSymbol.Name).FirstOrDefault(i => i.IsOverridden(originalSymbol));
          if (curSymbol != null) {
            UpdateName(curSymbol, newName, alreadyRefactorSymbols);
          }
          RefactorChildrensOverridden(originalSymbol, children, newName, alreadyRefactorSymbols);
        }
      }
    }

    private void UpdateName(ISymbol symbol, string newName, HashSet<ISymbol> alreadyRefactorSymbols) {
      var memberName = memberNames_.GetOrDefault(symbol);
      if (memberName == null) {
        var s = symbol;
        Utility.CheckOriginalDefinition(ref s);
        memberName = memberNames_.GetOrDefault(s);
      }
      if (memberName != null) {
        memberName.Update(newName);
        GetRefactorCheckName(symbol, newName, out string checkName1, out string checkName2);
        TryAddNewUsedName(symbol.ContainingType, checkName1);
        if (checkName2 != null) {
          TryAddNewUsedName(symbol.ContainingType, checkName2);
        }
        alreadyRefactorSymbols.Add(symbol);
      }
    }

    private void GetRefactorCheckName(ISymbol symbol, string newName, out string checkName1, out string checkName2) {
      checkName1 = newName;
      checkName2 = null;
      if (symbol.Kind == SymbolKind.Property) {
        var property = (IPropertySymbol)symbol;
        bool isField = IsPropertyField(property);
        if (!isField) {
          checkName1 = LuaSyntaxNode.Tokens.Get + newName;
          checkName2 = LuaSyntaxNode.Tokens.Set + newName;
        }
      } else if (symbol.Kind == SymbolKind.Event) {
        var evnetSymbol = (IEventSymbol)symbol;
        bool isField = IsEventFiled(evnetSymbol);
        if (!isField) {
          checkName1 = LuaSyntaxNode.Tokens.Add + newName;
          checkName2 = LuaSyntaxNode.Tokens.Remove + newName;
        }
      }
    }

    private string GetRefactorName(INamedTypeSymbol typeSymbol, IEnumerable<INamedTypeSymbol> childrens, ISymbol symbol) {
      bool isPrivate = symbol.IsPrivate();
      int index = 0;
      string originalName = memberIllegalNames_.GetOrDefault(symbol);
      if (originalName == null) {
        originalName = GetSymbolBaseName(symbol);
        index = 1;
      }

      while (true) {
        string newName = index != 0 ? originalName + index : originalName;
        GetRefactorCheckName(symbol, newName, out string checkName1, out string checkName2);

        bool isEnable = true;
        if (typeSymbol != null) {
          isEnable = IsNewNameEnable(typeSymbol, checkName1, checkName2, isPrivate);
        } else {
          if (!isPrivate && childrens != null) {
            isEnable = childrens.All(i => IsNewNameEnable(i, checkName1, checkName2, isPrivate));
          }
        }
        if (isEnable) {
          return newName;
        }
        ++index;
      }
    }

    private bool IsTypeNameUsed(INamedTypeSymbol typeSymbol, string newName) {
      return typeNameUseds_.Contains(typeSymbol, newName);
    }

    private bool IsNewNameEnable(INamedTypeSymbol typeSymbol, string checkName1, string checkName2, bool isPrivate) {
      bool isEnable = IsNewNameEnable(typeSymbol, checkName1, isPrivate);
      if (isEnable) {
        if (checkName2 != null) {
          isEnable = IsNewNameEnable(typeSymbol, checkName2, isPrivate);
        }
      }
      return isEnable;
    }

    private bool IsNewNameEnable(INamedTypeSymbol typeSymbol, string newName, bool isPrivate) {
      bool isEnable = IsNameEnableOfCurAndChildrens(typeSymbol, newName, isPrivate);
      if (isEnable) {
        if (!isPrivate) {
          var p = typeSymbol.BaseType;
          while (p != null) {
            if (!IsCurTypeNameEnable(p, newName)) {
              return false;
            }
            p = p.BaseType;
          }
        }
        return true;
      }
      return false;
    }

    private bool IsCurTypeNameEnable(INamedTypeSymbol typeSymbol, string newName) {
      return !IsTypeNameUsed(typeSymbol, newName) && typeSymbol.GetMembers(newName).IsEmpty;
    }

    private bool IsNameEnableOfCurAndChildrens(INamedTypeSymbol typeSymbol, string newName, bool isPrivate) {
      if (!IsCurTypeNameEnable(typeSymbol, newName)) {
        return false;
      }

      if (!isPrivate) {
        return IsInnerNameEnableOfChildrens(typeSymbol, newName, isPrivate);
      }

      return true;
    }

    private void CheckRefactorInnerNames() {
      foreach (var innerName in propertyOrEvnetInnerFieldNames_) {
        var symbol = innerName.Key;
        string newName = GetInnerGetRefactorName(symbol);
        innerName.Value.Update(newName);
        TryAddNewUsedName(symbol.ContainingType, newName);
      }
    }

    private string GetInnerGetRefactorName(ISymbol symbol) {
      bool isPrivate = symbol.IsPrivate();
      string originalName = GetSymbolBaseName(symbol);

      int index = 0;
      while (true) {
        string newName = index == 0 ? originalName : originalName + index;
        bool isEnable = IsInnerNameEnable(symbol.ContainingType, newName, isPrivate);
        if (isEnable) {
          return newName;
        }
        ++index;
      }
    }

    private bool IsInnerNameEnable(INamedTypeSymbol typeSymbol, string newName, bool isPrivate) {
      bool isEnable = IsInnerNameEnableOfChildrens(typeSymbol, newName, isPrivate);
      if (isEnable) {
        if (!isPrivate) {
          var p = typeSymbol.BaseType;
          while (p != null) {
            if (!IsCurTypeNameEnable(p, newName)) {
              return false;
            }
            p = p.BaseType;
          }
        }
        return true;
      }
      return false;
    }

    private bool IsInnerNameEnableOfChildrens(INamedTypeSymbol typeSymbol, string newName, bool isPrivate) {
      var childrens = extends_.GetOrDefault(typeSymbol);
      if (childrens != null) {
        foreach (INamedTypeSymbol children in childrens) {
          if (!IsNameEnableOfCurAndChildrens(children, newName, isPrivate)) {
            return false;
          }
        }
      }
      return true;
    }

    public bool IsMonoBehaviourSpecialMethod(IMethodSymbol symbol) {
      if (monoBehaviourSpecialMethodNames_ != null && symbol.ContainingType.Is(monoBehaviourTypeSymbol_)) {
        return monoBehaviourSpecialMethodNames_.Contains(symbol.Name) || symbol.Name.StartsWith("On");
      }
      return false;
    }

    #endregion
    private readonly Dictionary<ISymbol, HashSet<ISymbol>> implicitInterfaceImplementations_ = new Dictionary<ISymbol, HashSet<ISymbol>>();
    private readonly Dictionary<INamedTypeSymbol, Dictionary<ISymbol, ISymbol>> implicitInterfaceTypes_ = new Dictionary<INamedTypeSymbol, Dictionary<ISymbol, ISymbol>>();
    private readonly HashSet<INamedTypeSymbol> typesOfExtendSelf_ = new HashSet<INamedTypeSymbol>();

    private readonly ConcurrentDictionary<IPropertySymbol, bool> isFieldPropertys_ = new ConcurrentDictionary<IPropertySymbol, bool>();
    private readonly ConcurrentDictionary<IEventSymbol, bool> isFieldEvents_ = new ConcurrentDictionary<IEventSymbol, bool>();
    private readonly ConcurrentDictionary<ISymbol, bool> isMoreThanLocalVariables_ = new ConcurrentDictionary<ISymbol, bool>();
    private readonly ConcurrentDictionary<ISymbol, LuaSymbolNameSyntax> propertyOrEvnetInnerFieldNames_ = new ConcurrentDictionary<ISymbol, LuaSymbolNameSyntax>();
    private readonly ConcurrentHashSet<ISymbol> inlineSymbols_ = new ConcurrentHashSet<ISymbol>();

    private sealed class PretreatmentChecker : CSharpSyntaxWalker {
      private readonly LuaSyntaxGenerator generator_;
      private readonly HashSet<INamedTypeSymbol> classTypes_ = new HashSet<INamedTypeSymbol>();

      public PretreatmentChecker(LuaSyntaxGenerator generator) {
        generator_ = generator;
        foreach (SyntaxTree syntaxTree in generator.compilation_.SyntaxTrees) {
          Visit(syntaxTree.GetRoot());
        }
        Check();
      }

      private INamedTypeSymbol GetDeclaredSymbol(BaseTypeDeclarationSyntax node) {
        var semanticModel_ = generator_.compilation_.GetSemanticModel(node.SyntaxTree);
        return semanticModel_.GetDeclaredSymbol(node);
      }

      public override void VisitClassDeclaration(ClassDeclarationSyntax node) {
        var typeSymbol = GetDeclaredSymbol(node);
        classTypes_.Add(typeSymbol);

        var types = node.Members.OfType<BaseTypeDeclarationSyntax>();
        foreach (var type in types) {
          type.Accept(this);
        }
      }

      public override void VisitStructDeclaration(StructDeclarationSyntax node) {
        var typeSymbol = GetDeclaredSymbol(node);
        classTypes_.Add(typeSymbol);
      }

      public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node) {
        var typeSymbol = GetDeclaredSymbol(node);
        classTypes_.Add(typeSymbol);
      }

      public override void VisitEnumDeclaration(EnumDeclarationSyntax node) {
        var typeSymbol = GetDeclaredSymbol(node);
        classTypes_.Add(typeSymbol);
      }

      public override void VisitRecordDeclaration(RecordDeclarationSyntax node) {
        var typeSymbol = GetDeclaredSymbol(node);
        classTypes_.Add(typeSymbol);
      }

      private void Check() {
        foreach (var type in classTypes_) {
          generator_.AddTypeSymbol(type);
          CheckImplicitInterfaceImplementation(type);
          CheckTypeName(type);
        }
        CheckNamespace();
      }

      private void CheckImplicitInterfaceImplementation(INamedTypeSymbol type) {
        if (type.TypeKind == TypeKind.Class && !type.IsStatic) {
          foreach (var baseInterface in type.AllInterfaces) {
            foreach (var interfaceMember in baseInterface.GetMembers().Where(i => !i.IsStatic)) {
              var implementationMember = type.FindImplementationForInterfaceMember(interfaceMember);
              if (implementationMember.Kind == SymbolKind.Method) {
                var methodSymbol = (IMethodSymbol)implementationMember;
                if (methodSymbol.MethodKind != MethodKind.Ordinary) {
                  continue;
                }
              }

              var implementationType = implementationMember.ContainingType;
              if (!implementationType.EQ(type)) {
                if (!implementationType.AllInterfaces.Contains(baseInterface)) {
                  generator_.AddImplicitInterfaceImplementation(implementationMember, interfaceMember);
                  generator_.TryAddExtend(baseInterface, implementationType, true);
                }
              }
            }
          }

          if (IsExtendSelf(type)) {
            generator_.typesOfExtendSelf_.Add(type);
          }
        }
      }

      private bool IsExtendSelf(INamedTypeSymbol typeSymbol) {
        if (typeSymbol.BaseType != null) {
          if (Utility.IsExtendSelf(typeSymbol, typeSymbol.BaseType)) {
            return true;
          }
        }

        foreach (var baseType in typeSymbol.Interfaces) {
          if (Utility.IsExtendSelf(typeSymbol, baseType)) {
            return true;
          }
        }

        return false;
      }

      private void CheckTypeName(INamedTypeSymbol type) {
        string name = type.Name;
        if (type.TypeParameters.IsEmpty) {
          if (LuaSyntaxNode.IsReservedWord(name)) {
            RefactorTypeName(type, name, 1);
            return;
          }
        } else {
          string newName = name + '_' + type.TypeParameters.Length;
          if (CheckTypeNameExists(classTypes_, type, newName)) {
            RefactorTypeName(type, name, 3);
            return;
          }
        }

        if (Utility.IsIdentifierIllegal(ref name)) {
          RefactorTypeName(type, name, 0);
        }
      }

      private void RefactorTypeName(INamedTypeSymbol type, string name, int index) {
        string newName = GetTypeOrNamespaceNewName(classTypes_, type, name, index);
        generator_.typeRefactorNames_.Add(type, newName);
      }

      private string GetTypeOrNamespaceNewName(IEnumerable<ISymbol> allSymbols, ISymbol symbol, string name, int index = 0) {
        while (true) {
          string newName = Utility.GetNewIdentifierName(name, index);
          if (!CheckTypeNameExists(allSymbols, symbol, newName)) {
            return newName;
          }
        }
      }

      private static bool CheckTypeNameExists(IEnumerable<ISymbol> all, ISymbol type, string newName) {
        return all.Where(i => i.ContainingNamespace.EQ(type.ContainingNamespace)).Any(i => i.Name == newName);
      }

      private void CheckNamespace() {
        var all = classTypes_.SelectMany(i => i.ContainingNamespace.GetAllNamespaces()).Distinct().ToArray();
        foreach (var symbol in all) {
          string name = symbol.Name;
          if (LuaSyntaxNode.IsReservedWord(name)) {
            RefactorNamespaceName(all, symbol, symbol.Name, 1);
          } else {
            if (Utility.IsIdentifierIllegal(ref name)) {
              RefactorNamespaceName(all, symbol, name, 0);
            }
          }
        }
      }

      private void RefactorNamespaceName(INamespaceSymbol[] all, INamespaceSymbol symbol, string name, int index) {
        string newName = GetTypeOrNamespaceNewName(all, symbol, name, index);
        generator_.namespaceRefactorNames_.Add(symbol, newName);
      }
    }

    private void DoPretreatment() {
      new PretreatmentChecker(this);
    }

    private void AddImplicitInterfaceImplementation(ISymbol implementationMember, ISymbol interfaceMember) {
      bool success = implicitInterfaceImplementations_.TryAdd(implementationMember, interfaceMember);
      if (success) {
        var containingType = implementationMember.ContainingType;
        var mapps = implicitInterfaceTypes_.GetOrDefault(containingType);
        if (mapps == null) {
          mapps = new Dictionary<ISymbol, ISymbol>();
          implicitInterfaceTypes_.Add(containingType, mapps);
        }
        mapps.Add(interfaceMember, implementationMember);
      }
    }

    private ISymbol FindImplicitImplementationForInterfaceMember(INamedTypeSymbol typeSymbol, ISymbol interfaceMember) {
      var mapps = implicitInterfaceTypes_.GetOrDefault(typeSymbol);
      return mapps?.GetOrDefault(interfaceMember);
    }

    private bool IsImplicitInterfaceImplementation(ISymbol symbol) {
      return implicitInterfaceImplementations_.ContainsKey(symbol);
    }

    private bool IsModuleAutoField(ISymbol symbol) {
      var method = symbol.Kind == SymbolKind.Property ? ((IPropertySymbol)symbol).GetMethod : ((IEventSymbol)symbol).AddMethod;
      return method != null && method.GetAttributes().HasCompilerGeneratedAttribute();
    }

    private bool IsPropertyFieldInternal(IPropertySymbol symbol) {
      if (symbol.IsOverridable() || symbol.IsInterfaceImplementation()) {
        return false;
      }

      if (IsFromModuleOnly(symbol)) {
        return IsModuleAutoField(symbol);
      }

      if (symbol.IsFromAssembly()) {
        return false;
      }

      if (symbol.IsProtobufNetSpecialProperty()) {
        return true;
      }

      var node = symbol.GetDeclaringSyntaxNode();
      if (node != null) {
        switch (node.Kind()) {
          case SyntaxKind.PropertyDeclaration: {
            var property = (PropertyDeclarationSyntax)node;
            bool hasGet = false;
            bool hasSet = false;
            if (property.AccessorList != null) {
              foreach (var accessor in property.AccessorList.Accessors) {
                if (accessor.Body != null || accessor.ExpressionBody != null) {
                  if (accessor.IsKind(SyntaxKind.GetAccessorDeclaration)) {
                    Contract.Assert(!hasGet);
                    hasGet = true;
                  } else {
                    Contract.Assert(!hasSet);
                    hasSet = true;
                  }
                }
              }
            } else {
              Contract.Assert(!hasGet);
              hasGet = true;
            }
            bool isField = !hasGet && !hasSet;
            if (isField) {
              if (property.HasCSharpLuaAttribute(LuaDocumentStatement.AttributeFlags.NoField)) {
                isField = false;
              }
            }
            return isField;
          }
          case SyntaxKind.IndexerDeclaration: {
            return false;
          }
          case SyntaxKind.AnonymousObjectMemberDeclarator: {
            return true;
          }
          case SyntaxKind.Parameter: {
            return true;
          }
          default: {
            throw new InvalidOperationException();
          }
        }
      }
      return false;
    }

    internal bool IsPropertyField(IPropertySymbol symbol) {
      return isFieldPropertys_.GetOrAdd(symbol, symbol => {
        bool isField;
        bool? isMateField = XmlMetaProvider.IsPropertyField(symbol);
        if (isMateField.HasValue) {
          isField = isMateField.Value;
        } else {
          if (IsImplicitInterfaceImplementation(symbol)) {
            isField = false;
          } else {
            isField = IsPropertyFieldInternal(symbol);
          }
        }
        return isField;
      });
    }

    private bool IsEventFiledInternal(IEventSymbol symbol) {
      if (symbol.IsOverridable() || symbol.IsInterfaceImplementation()) {
        return false;
      }

      if (IsFromModuleOnly(symbol)) {
        return IsModuleAutoField(symbol);
      }

      if (symbol.IsFromAssembly()) {
        return false;
      }

      var node = symbol.GetDeclaringSyntaxNode();
      if (node != null) {
        return node.IsKind(SyntaxKind.VariableDeclarator);
      }

      return false;
    }

    internal bool IsEventFiled(IEventSymbol symbol) {
      return isFieldEvents_.GetOrAdd(symbol, symbol => {
        bool isField;
        if (IsImplicitInterfaceImplementation(symbol)) {
          isField = false;
        } else {
          isField = IsEventFiledInternal(symbol);
        }
        return isField;
      });
    }

    internal bool IsPropertyFieldOrEventFiled(ISymbol symbol) {
      if (symbol is IPropertySymbol propertySymbol) {
        return IsPropertyField(propertySymbol);
      } else if (symbol is IEventSymbol eventSymbol) {
        return IsEventFiled(eventSymbol);
      }
      return false;
    }

    public bool IsMoreThanLocalVariables(ISymbol symbol) {
      Contract.Assert(symbol.IsFromCode());
      return isMoreThanLocalVariables_.GetOrAdd(symbol, symbol => {
        const int kMaxLocalVariablesCount = LuaSyntaxNode.kLocalVariablesMaxCount - 5;
        var methods = symbol.ContainingType.GetMembers().Where(i => {
          switch (i.Kind) {
            case SymbolKind.Method: {
              var method = (IMethodSymbol)i;
              switch (method.MethodKind) {
                case MethodKind.Constructor: {
                  return false;
                }
                case MethodKind.PropertyGet:
                case MethodKind.PropertySet: {
                  if (IsPropertyField((IPropertySymbol)method.AssociatedSymbol)) {
                    return false;
                  }
                  break;
                }
                case MethodKind.EventAdd:
                case MethodKind.EventRaise:
                case MethodKind.EventRemove: {
                  if (IsEventFiled((IEventSymbol)method.AssociatedSymbol)) {
                    return false;
                  }
                  break;
                }
              }

              return true;
            }
            case SymbolKind.Field: {
              var field = (IFieldSymbol)i;
              if (field.IsStringConstNotInline()) {
                return true;
              }
              break;
            }
          }
          return false;
        }).ToList();

        int index = 0;
        switch (symbol.Kind) {
          case SymbolKind.Field:
          case SymbolKind.Method: {
            index = methods.FindIndex(i => i.EQ(symbol));
            break;
          }
          case SymbolKind.Property:
          case SymbolKind.Event: {
            index = methods.FindIndex(i => i.Kind == SymbolKind.Method && symbol.EQ(((IMethodSymbol)i).AssociatedSymbol)) + 1;
            break;
          }
        }

        bool isMoreThanLocalVariables = index + symbol.ContainingType.Constructors.Length > kMaxLocalVariablesCount;
        return isMoreThanLocalVariables;
      });
    }

    internal void AddInlineSymbol(IMethodSymbol symbol) {
      if (symbol.MethodKind == MethodKind.PropertyGet) {
        inlineSymbols_.Add(symbol.AssociatedSymbol);
      } else {
        inlineSymbols_.Add(symbol);
      }
    }

    internal bool IsInlineSymbol(ISymbol symbol) {
      return inlineSymbols_.Contains(symbol);
    }

    private IEnumerable<ISymbol> AllInterfaceImplementations(ISymbol symbol) {
      var interfaceImplementations = symbol.InterfaceImplementations();
      var implicitImplementations = implicitInterfaceImplementations_.GetOrDefault(symbol);
      if (implicitImplementations != null) {
        interfaceImplementations = interfaceImplementations.Concat(implicitImplementations);
      }
      return interfaceImplementations;
    }

    private int AllInterfaceImplementationsCount(ISymbol symbol) {
      int count = 0;
      var implicitImplementations = implicitInterfaceImplementations_.GetOrDefault(symbol);
      if (implicitImplementations != null) {
        count = implicitImplementations.Count;
      }
      count += symbol.InterfaceImplementations().Count();
      return count;
    }

    internal bool HasStaticCtor(INamedTypeSymbol typeSymbol) {
      return typesOfExtendSelf_.Contains(typeSymbol) || IsExplicitStaticCtorExists(typeSymbol);
    }

    internal bool IsExtendExists(INamedTypeSymbol typeSymbol) {
      var set = extends_.GetOrDefault(typeSymbol);
      return set != null && set.Count > 0;
    }

    internal bool IsSealed(INamedTypeSymbol typeSymbol) {
      return typeSymbol.IsSealed || (!Setting.IsModule && !IsExtendExists(typeSymbol));
    }

    internal bool IsReadOnlyStruct(ITypeSymbol symbol) {
      if (symbol.IsValueType && symbol.TypeKind != TypeKind.TypeParameter) {
        return symbol.IsReadOnly || XmlMetaProvider.IsTypeReadOnly((INamedTypeSymbol)symbol);
      }
      return false;
    }

    internal bool IsExplicitStaticCtorExists(INamedTypeSymbol typeSymbol) {
      var constructor = typeSymbol.StaticConstructors.SingleOrDefault();
      if (constructor != null) {
        if (!constructor.IsImplicitlyDeclared) {
          return true;
        }

        if (IsInitFieldExists(typeSymbol, true)) {
          return true;
        }
      }

      return false;
    }

    internal bool IsBaseExplicitCtorExists(INamedTypeSymbol baseType) {
      while (baseType != null && !baseType.IsSystemObjectOrValueType()) {
        var constructor = baseType.InstanceConstructors.FirstOrDefault();
        if (constructor != null) {
          if (!constructor.IsImplicitlyDeclared) {
            return true;
          }

          if (IsInitFieldExists(baseType, false)) {
            return true;
          }
        }

        baseType = baseType.BaseType;
      }
      return false;
    }

    private static bool IsInitFieldExists<T>(IEnumerable<T> fieldSymbols, Func<T, ITypeSymbol> fieldTypeFunc, Func<SyntaxNode, ExpressionSyntax> fieldValueFunc) where T : ISymbol {
      foreach (var field in fieldSymbols) {
        var fieldType = fieldTypeFunc(field);
        if (fieldType.IsCustomValueType() && !fieldType.IsNullableType()) {
          return true;
        }

        var node = field.GetDeclaringSyntaxNode();
        if (node != null) {
          var valueExpression = fieldValueFunc(node);
          if (valueExpression != null) {
            var valueKind = valueExpression.Kind();
            if (valueKind != SyntaxKind.NullLiteralExpression) {
              bool isImmutable = fieldType.IsImmutable() && valueKind.IsLiteralExpression();
              if (!isImmutable) {
                return true;
              }
            }
          }
        }
      }

      return false;
    }

    private bool IsInitFieldExists(INamedTypeSymbol symbol, bool isStatic) {
      var members = symbol.GetMembers().Where(i => i.IsStatic == isStatic);
      var fields = members.OfType<IFieldSymbol>();
      if (IsInitFieldExists(fields, i => i.Type, node => ((VariableDeclaratorSyntax)node).Initializer?.Value)) {
        return true;
      }

      var properties = members.OfType<IPropertySymbol>();
      if (IsInitFieldExists(properties, i => i.Type, node => ((PropertyDeclarationSyntax)node).Initializer?.Value)) {
        return true;
      }

      return false;
    }

    #region type and namespace refactor

    private readonly Dictionary<INamespaceSymbol, string> namespaceRefactorNames_ = new Dictionary<INamespaceSymbol, string>();
    private readonly Dictionary<INamedTypeSymbol, string> typeRefactorNames_ = new Dictionary<INamedTypeSymbol, string>();

    private string GetTypeRefactorName(INamedTypeSymbol symbol) {
      return typeRefactorNames_.GetOrDefault(symbol);
    }

    internal LuaIdentifierNameSyntax GetTypeDeclarationName(INamedTypeSymbol typeSymbol) {
      string name = GetTypeRefactorName(typeSymbol);
      if (name == null) {
        name = typeSymbol.Name;
        int typeParametersCount = typeSymbol.TypeParameters.Length;
        if (typeParametersCount > 0) {
          name += "_" + typeParametersCount;
        }
      }
      return name;
    }

    internal LuaExpressionSyntax GetTypeName(ISymbol symbol, LuaSyntaxNodeTransform transfor = null) {
      switch (symbol.Kind) {
        case SymbolKind.TypeParameter: {
          return symbol.Name;
        }
        case SymbolKind.ArrayType: {
          var arrayType = (IArrayTypeSymbol)symbol;
          transfor?.AddGenericTypeCounter();
          var elementType = GetTypeName(arrayType.ElementType, transfor);
          transfor?.SubGenericTypeCounter();
          var invocation = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Array, elementType);
          if (arrayType.Rank > 1) {
            invocation.AddArgument(arrayType.Rank.ToString());
          }
          LuaExpressionSyntax luaExpression = invocation;
          if (transfor != null) {
            transfor.ImportGenericTypeName(ref luaExpression, arrayType);
          }
          return luaExpression;
        }
        case SymbolKind.PointerType: {
          var pointType = (IPointerTypeSymbol)symbol;
          var elementTypeExpression = GetTypeName(pointType.PointedAtType, transfor);
          LuaExpressionSyntax luaExpression = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Array, elementTypeExpression);
          if (transfor != null) {
            transfor.ImportGenericTypeName(ref luaExpression, pointType);
          }
          return luaExpression;
        }
        case SymbolKind.DynamicType: {
          return LuaIdentifierNameSyntax.Object;
        }
      }

      var namedTypeSymbol = (INamedTypeSymbol)symbol;
      if (IsConstantEnum(namedTypeSymbol)) {
        return GetTypeName(namedTypeSymbol.EnumUnderlyingType, transfor);
      }

      if (namedTypeSymbol.IsDelegateType()) {
        if (transfor != null && transfor.IsMetadataTypeName) {
          var delegateMethod = namedTypeSymbol.DelegateInvokeMethod;
          if (!delegateMethod.Parameters.IsEmpty || !delegateMethod.ReturnsVoid) {
            var arguments = delegateMethod.Parameters.Select(i => GetTypeName(i.Type, transfor)).ToList();
            if (delegateMethod.ReturnsVoid) {
              arguments.Add(LuaIdentifierNameSyntax.SystemVoid);
            } else {
              arguments.Add(GetTypeName(delegateMethod.ReturnType, transfor));
            }
            return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Delegate, arguments); ;
          }
        }
        return LuaIdentifierNameSyntax.Delegate;
      }

      if (namedTypeSymbol.IsAnonymousType) {
        return LuaIdentifierNameSyntax.AnonymousType;
      }

      if (namedTypeSymbol.IsTupleType) {
        return LuaIdentifierNameSyntax.ValueTuple;
      }

      if (namedTypeSymbol.IsSystemTuple()) {
        return LuaIdentifierNameSyntax.Tuple;
      }

      if (transfor != null && transfor.IsNoneGenericTypeCounter) {
        var curTypeDeclaration = transfor.CurTypeDeclaration;
        if (curTypeDeclaration != null && curTypeDeclaration.CheckTypeName(namedTypeSymbol, out var classIdentifier)) {
          return classIdentifier;
        }
      }

      var typeName = GetTypeShortName(namedTypeSymbol, transfor);
      var typeArguments = GetTypeArguments(namedTypeSymbol, transfor);
      if (typeArguments.Count == 0) {
        return typeName;
      } else if (XmlMetaProvider.IsTypeIgnoreGeneric(namedTypeSymbol)) {
        string name = typeName.ValueText;
        int genericTokenPos = name.LastIndexOf('_');
        if (genericTokenPos != -1) {
          return name.Substring(0, genericTokenPos);
        } else {
          return typeName;
        }
      } else {
        var invocationExpression = new LuaInvocationExpressionSyntax(typeName);
        invocationExpression.AddArguments(typeArguments);
        LuaExpressionSyntax luaExpression = invocationExpression;
        if (transfor != null) {
          transfor.ImportGenericTypeName(ref luaExpression, namedTypeSymbol);
        }
        return luaExpression;
      }
    }

    private List<LuaExpressionSyntax> GetTypeArguments(INamedTypeSymbol typeSymbol, LuaSyntaxNodeTransform transfor) {
      List<LuaExpressionSyntax> typeArguments = new List<LuaExpressionSyntax>();
      FillExternalTypeArgument(typeArguments, typeSymbol, transfor);
      FillTypeArguments(typeArguments, typeSymbol, transfor);
      return typeArguments;
    }

    private void FillExternalTypeArgument(List<LuaExpressionSyntax> typeArguments, INamedTypeSymbol typeSymbol, LuaSyntaxNodeTransform transfor) {
      var externalType = typeSymbol.ContainingType;
      if (externalType != null) {
        FillExternalTypeArgument(typeArguments, externalType, transfor);
        FillTypeArguments(typeArguments, externalType, transfor);
      }
    }

    private void FillTypeArguments(List<LuaExpressionSyntax> typeArguments, INamedTypeSymbol typeSymbol, LuaSyntaxNodeTransform transfor) {
      if (typeSymbol.TypeArguments.Length > 0) {
        transfor?.AddGenericTypeCounter();
        foreach (var typeArgument in typeSymbol.TypeArguments) {
          if (typeArgument.Kind == SymbolKind.ErrorType) {
            break;
          }
          var typeArgumentExpression = GetTypeName(typeArgument, transfor);
          typeArguments.Add(typeArgumentExpression);
        }
        transfor?.SubGenericTypeCounter();
      }
    }

    private string GetNamespaceNames(IEnumerable<INamespaceSymbol> symbols) {
      var names = symbols.Select(i => namespaceRefactorNames_.GetOrDefault(i, i.Name));
      return string.Join(".", names);
    }

    private string GetNamespaceMapName(INamespaceSymbol symbol, string original) {
      if (symbol.IsFromCode()) {
        return GetNamespaceNames(symbol.GetAllNamespaces());
      } else {
        return XmlMetaProvider.GetNamespaceMapName(symbol, original);
      }
    }

    internal string GetNamespaceDefineName(INamespaceSymbol symbol, NamespaceDeclarationSyntax node) {
      string original = node.Name.ToString();
      if (original == symbol.Name) {
        return namespaceRefactorNames_.GetOrDefault(symbol, original);
      } else {
        var namespaces = new List<INamespaceSymbol>() { symbol };
        do {
          symbol = symbol.ContainingNamespace;
          namespaces.Add(symbol);
          IEnumerable<INamespaceSymbol> symbols = namespaces;
          symbols = symbols.Reverse();
          string symbolsName = string.Join(".", symbols.Select(i => i.Name));
          if (symbolsName == original) {
            return GetNamespaceNames(symbols);
          }
        } while (!symbol.IsGlobalNamespace);
      }
      throw new InvalidOperationException();
    }

    internal LuaIdentifierNameSyntax GetTypeShortName(ISymbol symbol, LuaSyntaxNodeTransform transfor = null) {
      var typeSymbol = (INamedTypeSymbol)symbol.OriginalDefinition;
      string name = typeSymbol.GetTypeShortName(GetNamespaceMapName, GetTypeRefactorName, transfor);
      string newName = XmlMetaProvider.GetTypeMapName(typeSymbol, name);
      if (newName != null) {
        name = newName;
      }
      if (transfor != null) {
        if (transfor.IsNoImportTypeName) {
          if (!name.StartsWith(LuaIdentifierNameSyntax.System.ValueText) && !name.StartsWith(LuaIdentifierNameSyntax.Class.ValueText)) {
            name = LuaIdentifierNameSyntax.Global.ValueText + '.' + name;
          }
        } else {
          transfor.ImportTypeName(ref name, (INamedTypeSymbol)symbol);
        }
      }
      return name;
    }

    #endregion
  }
}
