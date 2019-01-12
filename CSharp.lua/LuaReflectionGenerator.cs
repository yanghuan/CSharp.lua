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
            | SymbolDisplayKindOptions.IncludeNamespaceKeyword,
        miscellaneousOptions:
            SymbolDisplayMiscellaneousOptions.UseSpecialTypes);


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
      WriteLine($"[\"{type.ToDisplayString()}\"] = ");
      WriteReflectionType(type);
      WriteLine(",");
    }

    public void GenerateReflectionFile(List<ITypeSymbol> types, string outFolder) {
      depth = 0;
      const string kReflectionFile = "reflection.lua";
      var generator = new LuaReflectionGenerator();
      string outFile = Path.Combine(outFolder, kReflectionFile);
      reflectedTypes = types.ToDictionary(v => v.ToDisplayString(), v => new KeyValuePair<ITypeSymbol, bool>(v,true));
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
      if (!reflectedTypes.ContainsKey(type.ToDisplayString()))
        reflectedTypes.Add(type.ToDisplayString(), new KeyValuePair<ITypeSymbol, bool>(type,false));
    }


    private void WriteField(IFieldSymbol symbol) {
      OpenBranch();
      WriteLine($"name = \"{symbol.Name}\",");
      WriteLine($"isStatic = {symbol.IsStatic.ToString().ToLower()},");

      WriteLine($"fieldType = \"{symbol.Type.ToDisplayString()}\",");
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
        WriteLine($"returnType = \"{symbol.ReturnType.ToDisplayString()}\",");
        AddRefType(symbol.ReturnType);
      }
      if (symbol.Parameters.Length > 0) {
        WriteLine($"parameters = ");
        OpenBranch();
        foreach (var arg in symbol.Parameters) {
          OpenBranch();
          WriteLine($"name = \"{arg.Name}\",");
          WriteLine($"type = \"{arg.Type.ToDisplayString()}\",");
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

      WriteLine($"propertyType = \"{symbol.Type.ToDisplayString()}\",");
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
      WriteLine($"name = \"{type.ToDisplayString()}\",");
      if (type.BaseType != null) {
        WriteLine($"baseType = \"{type.BaseType.ToDisplayString()}\",");
        AddRefType(type.BaseType);
      }
    }


    private void WriteTypeArguments(ImmutableArray<ITypeSymbol> TypeArguments) {
      if (TypeArguments.Length > 0) {
        WriteLine($"typeArguments = ");
        OpenBranch();
        foreach (var arg in TypeArguments) {
          WriteLine($"\"{arg.ToDisplayString()}\",");
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
          WriteLine($"\"{arg.ToDisplayString()}\",");
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
        WriteLine($"constructedFrom = \"{namedType.ConstructedFrom.ToDisplayString()}\",");
        AddRefType(namedType.ConstructedFrom);
      }
      WriteTypeArguments(namedType.TypeArguments);
      WriteTypeParameters(namedType.TypeParameters);
    }

    private void WriteReflectionType(ITypeSymbol type) {
      OpenBranch();
      // type head
      WriteTypeBase(type);
      // generic
      WriteTypeGeneric(type);
      // all fields
      WriteFields(type);
      // all fields
      WriteProperties(type);
      // all fields
      WriteMethods(type);

      CloseBranch();
    }
  }

}
