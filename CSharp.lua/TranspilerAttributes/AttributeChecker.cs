using System;

using Microsoft.CodeAnalysis;

namespace CSharpLua {
  internal static class AttributeChecker {
    public static bool HasAttribute<T>(this ISymbol symbol, out AttributeData attributeData)
      where T : Attribute {
      foreach (var attrData in symbol.GetAttributes()) {
        // TODO: find better method to compare AttributeData and Attribute?
        if (attrData.AttributeClass.Name == nameof(T)) {
          attributeData = attrData;
          return true;
        }
      }
      attributeData = null;
      return false;
    }
  }
}
