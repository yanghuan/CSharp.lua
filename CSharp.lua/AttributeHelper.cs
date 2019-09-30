using System;

using CSharpLua.LuaAst;

using Microsoft.CodeAnalysis;

namespace CSharpLua {
  internal static class AttributeHelper {
    public static bool HasAttribute<T>(this ISymbol symbol, out AttributeData attributeData)
      where T : Attribute {
      foreach (var attrData in symbol.GetAttributes()) {
        // TODO: find better method to compare AttributeData and Attribute?
        if (attrData.AttributeClass.Name == typeof(T).Name) {
          attributeData = attrData;
          return true;
        }
      }
      attributeData = null;
      return false;
    }

    public static LuaInvocationExpressionSyntax GenerateEnumCastExpression(AttributeData attributeData, LuaExpressionSyntax invocationArgumentExpression) {
      return new LuaInvocationExpressionSyntax(new LuaIdentifierLiteralExpressionSyntax(attributeData.ConstructorArguments[0].Value.ToString()), invocationArgumentExpression);
    }
  }
}
