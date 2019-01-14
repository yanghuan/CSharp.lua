using CSharpLua.LuaAst;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;

namespace CSharpLua {
  public class LuaReflectionGenerator {
    int depth;
    TextWriter writer;
    LuaSyntaxGenerator luaSyntaxGenerator_;

    private readonly static SymbolDisplayFormat FullSymbolDisplayFormat = new SymbolDisplayFormat(
        typeQualificationStyle:
            SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions:
            SymbolDisplayGenericsOptions.IncludeTypeConstraints
            | SymbolDisplayGenericsOptions.IncludeVariance
            | SymbolDisplayGenericsOptions.IncludeTypeParameters,
        memberOptions:
            SymbolDisplayMemberOptions.IncludeAccessibility
            | SymbolDisplayMemberOptions.IncludeExplicitInterface
            | SymbolDisplayMemberOptions.IncludeModifiers
            | SymbolDisplayMemberOptions.IncludeParameters
            | SymbolDisplayMemberOptions.IncludeType,
        delegateStyle:
            SymbolDisplayDelegateStyle.NameAndSignature,
        extensionMethodStyle:
            SymbolDisplayExtensionMethodStyle.StaticMethod,
        parameterOptions:
            SymbolDisplayParameterOptions.IncludeDefaultValue
            | SymbolDisplayParameterOptions.IncludeExtensionThis
            | SymbolDisplayParameterOptions.IncludeName
            | SymbolDisplayParameterOptions.IncludeParamsRefOut
            | SymbolDisplayParameterOptions.IncludeType,
        propertyStyle:
            SymbolDisplayPropertyStyle.ShowReadWriteDescriptor,
        kindOptions:
            SymbolDisplayKindOptions.IncludeTypeKeyword
            | SymbolDisplayKindOptions.IncludeMemberKeyword
            | SymbolDisplayKindOptions.IncludeNamespaceKeyword
      //, miscellaneousOptions:
      //      SymbolDisplayMiscellaneousOptions.UseSpecialTypes
      );

    private readonly static SymbolDisplayFormat FullTypeDisplayFormat = new SymbolDisplayFormat(
        typeQualificationStyle:
            SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions:
            SymbolDisplayGenericsOptions.IncludeTypeConstraints
            | SymbolDisplayGenericsOptions.IncludeVariance
            | SymbolDisplayGenericsOptions.IncludeTypeParameters,
        memberOptions:
            SymbolDisplayMemberOptions.IncludeAccessibility
            | SymbolDisplayMemberOptions.IncludeExplicitInterface
            | SymbolDisplayMemberOptions.IncludeModifiers
            | SymbolDisplayMemberOptions.IncludeParameters
            | SymbolDisplayMemberOptions.IncludeType,
        delegateStyle:
            SymbolDisplayDelegateStyle.NameAndSignature,
        extensionMethodStyle:
            SymbolDisplayExtensionMethodStyle.StaticMethod,
        parameterOptions:
            SymbolDisplayParameterOptions.IncludeDefaultValue
            | SymbolDisplayParameterOptions.IncludeExtensionThis
            | SymbolDisplayParameterOptions.IncludeName
            | SymbolDisplayParameterOptions.IncludeParamsRefOut
            | SymbolDisplayParameterOptions.IncludeType,
        propertyStyle:
            SymbolDisplayPropertyStyle.ShowReadWriteDescriptor
        //, kindOptions:
        //    SymbolDisplayKindOptions.IncludeTypeKeyword
        //    | SymbolDisplayKindOptions.IncludeMemberKeyword
        //    | SymbolDisplayKindOptions.IncludeNamespaceKeyword
      //, miscellaneousOptions:
      //      SymbolDisplayMiscellaneousOptions.UseSpecialTypes
      );

    string RenderText(LuaExpressionSyntax exp) {
      var memoryStream = new MemoryStream(_textBuffer);
      var textWriter = new StreamWriter(memoryStream);
      var luaRenderer = new LuaRenderer(luaSyntaxGenerator_, textWriter);
      exp.Render(luaRenderer);
      textWriter.Flush();
      var size = memoryStream.Position;
      textWriter.Close();

      memoryStream = new MemoryStream(_textBuffer,0, (int)size);
      var textReader = new StreamReader(memoryStream);
      var ret = textReader.ReadToEnd();
      textReader.Close();
      return ret;
    }
    string GetTypeName(ISymbol symbol) {
      try {
        var typeName = luaSyntaxGenerator_.GetTypeName(symbol);
        return typeName != null ? RenderText(typeName) : string.Empty;
      }
      catch(CompilationErrorException) {
        return symbol.ToDisplayString(FullTypeDisplayFormat);
      }
    }

    string GetTypeShortName(ISymbol symbol) {
      try {
        var typeName = luaSyntaxGenerator_.GetTypeShortName(symbol);
        return typeName != null ? typeName.ValueText : string.Empty;
      } catch (CompilationErrorException) {
        return symbol.ToDisplayString(FullTypeDisplayFormat);
      }
    }
    byte[] _textBuffer = new byte[1024 * 1024];
    public LuaReflectionGenerator(LuaSyntaxGenerator luaSyntaxGenerator) {
      luaSyntaxGenerator_ = luaSyntaxGenerator;
      
    }

    void WriteIntent() {
      writer.Write(new string('\t', depth));
    }

    void WriteLine(string value) {
      WriteIntent();
      writer.WriteLine(value);
    }

    void Write(string value) {
      WriteIntent();
      writer.Write(value);
    }
    void OpenBranch() {
      WriteIntent();
      writer.WriteLine('{');
      depth++;
    }
    void CloseBranch() {
      depth--;
      WriteIntent();
      writer.WriteLine('}');
    }
    void WriteLine() {
      writer.WriteLine();
    }

    Dictionary<string, KeyValuePair<ITypeSymbol,bool>> reflectedTypes = new Dictionary<string, KeyValuePair<ITypeSymbol, bool>>();


    private void WriteTypeDecl(ITypeSymbol type) {
      WriteLine($"[\"{GetTypeName(type)}\"] = ");
      WriteReflectionType(type);
      WriteLine(",");
    }

    public void GenerateReflectionFile(List<ITypeSymbol> types, string outFolder) {
      depth = 0;
      const string kReflectionFile = "reflection.lua";
      string outFile = Path.Combine(outFolder, kReflectionFile);
      reflectedTypes = types.ToDictionary(v => v.ToDisplayString(FullTypeDisplayFormat), v => new KeyValuePair<ITypeSymbol, bool>(v,true));
      using (writer = new StreamWriter(outFile, false, new UTF8Encoding(false))) {
        WriteLine("return ");
        OpenBranch();
        if (types.Count > 0) {
          foreach (var type in types) {
            WriteTypeDecl(type);
          }
          // more types add to tail
          while (true) {
            var moreTypes = reflectedTypes.Where(v => !v.Value.Value).ToList();
            foreach (var type in moreTypes) {
              reflectedTypes[type.Key] = new KeyValuePair<ITypeSymbol, bool>(type.Value.Key, true);
              WriteTypeDecl(type.Value.Key);
            }
            if (moreTypes.Count == 0)
              break;
          }
        }
        CloseBranch();
      }
    }

    void AddRefType(ITypeSymbol type) {
      var key = type.ToDisplayString(FullTypeDisplayFormat);
      if (!reflectedTypes.ContainsKey(key))
        reflectedTypes.Add(key, new KeyValuePair<ITypeSymbol, bool>(type,false));
    }


    private void WriteField(IFieldSymbol symbol) {
      OpenBranch();
      WriteLine($"name = \"{symbol.Name}\",");
      WriteLine($"isStatic = {symbol.IsStatic.ToString().ToLower()},");

      WriteLine($"fieldType = \"{GetTypeName(symbol.Type)}\",");
      AddRefType(symbol.Type);
      CloseBranch();
    }

    private void WriteFields(ITypeSymbol type) {
      var iterator = type.GetMembers().Where(member =>member.IsPublic() && member is IFieldSymbol).Select(v => v as IFieldSymbol).ToList();
      if (iterator.Count == 0)
        return;
      WriteLine("fields = ");
      OpenBranch();
      foreach (var symbol in iterator) {
        WriteLine($"-- {symbol.ToDisplayString(FullSymbolDisplayFormat)}");
        WriteField(symbol);
        WriteLine(",");
      }
      CloseBranch();
      WriteLine(",");
    }



    private void WriteMethod(IMethodSymbol symbol) {
      OpenBranch();
      WriteLine($"name = \"{symbol.Name}\",");
      WriteLine($"isStatic = {symbol.IsStatic.ToString().ToLower()},");

      if (symbol.ReturnType != null) {
        WriteLine($"returnType = \"{GetTypeName(symbol.ReturnType)}\",");
        AddRefType(symbol.ReturnType);
      }
      if (symbol.Parameters.Length > 0) {
        WriteLine($"parameters = ");
        OpenBranch();
        foreach (var arg in symbol.Parameters) {
          OpenBranch();
          WriteLine($"name = \"{arg.Name}\",");
          WriteLine($"type = \"{GetTypeName(arg.Type)}\",");
          AddRefType(arg.Type);
          CloseBranch();
          WriteLine(",");
        }
        CloseBranch();
        WriteLine(",");
      }
      WriteTypeArguments(symbol.TypeArguments);
      WriteTypeParameters(symbol.TypeParameters);

      CloseBranch();
    }

    private void WriteMethods(ITypeSymbol type) {
      var iterator = type.GetMembers().Where(member => member.IsPublic() && member is IMethodSymbol).Select(v => v as IMethodSymbol).ToList();
      if (iterator.Count == 0)
        return;
      WriteLine("methods = ");
      OpenBranch();
      foreach (var symbol in iterator) {
        WriteLine($"-- {symbol.ToDisplayString(FullSymbolDisplayFormat)}");
        WriteMethod(symbol);
        WriteLine(",");
      }
      CloseBranch();
      WriteLine(",");
    }

    private void WriteProperty(IPropertySymbol symbol) {
      OpenBranch();
      WriteLine($"name = \"{symbol.Name}\",");
      WriteLine($"isStatic = {symbol.IsStatic.ToString().ToLower()},");

      WriteLine($"propertyType = \"{GetTypeName(symbol.Type)}\",");
      AddRefType(symbol.Type);
      CloseBranch();
    }

    private void WriteProperties(ITypeSymbol type) {
      var iterator = type.GetMembers().Where(member => member.IsPublic() && member is IPropertySymbol).Select(v => v as IPropertySymbol).ToList();
      if (iterator.Count == 0)
        return;
      WriteLine("properties = ");
      OpenBranch();
      foreach (var symbol in iterator) {
        WriteLine($"-- {symbol.ToDisplayString(FullSymbolDisplayFormat)}");
        WriteProperty(symbol);
        WriteLine(",");
      }
      CloseBranch();
      WriteLine(",");
    }

    private void WriteTypeBase(ITypeSymbol type) {
      WriteLine($"kind = \"{type.Kind}\",");
      WriteLine($"typeKind = \"{type.TypeKind}\",");
      WriteLine($"name = \"{GetTypeName(type)}\",");
      if (type.BaseType != null) {
        WriteLine($"baseType = \"{GetTypeName(type.BaseType)}\",");
        AddRefType(type.BaseType);
      }
    }


    private void WriteTypeArguments(ImmutableArray<ITypeSymbol> TypeArguments) {
      if (TypeArguments.Length > 0) {
        WriteLine($"typeArguments = ");
        OpenBranch();
        foreach (var arg in TypeArguments) {
          WriteLine($"\"{GetTypeName(arg)}\",");
          AddRefType(arg);
        }
        CloseBranch();
        WriteLine(",");
      }
    }

    private void WriteTypeParameters(ImmutableArray<ITypeParameterSymbol> TypeParameters) {
      if (TypeParameters.Length > 0) {
        WriteLine($"typeParameters = ");
        OpenBranch();
        foreach (var arg in TypeParameters) {
          WriteLine($"\"{GetTypeName(arg)}\",");
          AddRefType(arg);
        }
        CloseBranch();
        WriteLine(",");
      }
    }

    private void WriteTypeGeneric(ITypeSymbol type) {
      var namedType = type as INamedTypeSymbol;
      if (namedType == null)
        return;
      if(namedType.ConstructedFrom != namedType) {
        WriteLine($"constructedFrom = \"{GetTypeShortName(namedType.ConstructedFrom)}\",");
        AddRefType(namedType.ConstructedFrom);
      }
      WriteTypeArguments(namedType.TypeArguments);
      WriteTypeParameters(namedType.TypeParameters);
    }

    private void WriteTypeArray(ITypeSymbol type) {
      var arrayType = type as IArrayTypeSymbol;
      if (arrayType == null)
        return;
      var elementType = arrayType.ElementType;
      WriteLine($"elementType = \"{GetTypeName(elementType)}\",");
      AddRefType(elementType);
    }
    private void WriteReflectionType(ITypeSymbol type) {
      OpenBranch();
      // type head
      WriteTypeBase(type);
      // array
      WriteTypeArray(type);
      // generic
      WriteTypeGeneric(type);
      // only gen when code
      if (type.IsFromCode()) {
        // all fields
        WriteFields(type);
        // all fields
        WriteProperties(type);
        // all fields
        WriteMethods(type);
      }

      CloseBranch();
    }
  }

}
