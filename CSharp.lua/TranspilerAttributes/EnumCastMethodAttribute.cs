using System;

using CSharpLua.LuaAst;

using Microsoft.CodeAnalysis;

namespace CSharpLua {
  /// <summary>
  /// Indicates that this enum type cannot be cast to or from an integer, neither implicitly nor explicitly.
  /// Instead, there is a one-way conversion from integer to this enum type, defined by <see cref="CastFunctionName"/>.
  /// </summary>
  [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
  public class EnumCastMethodAttribute : Attribute {
    private readonly string _func;

    public EnumCastMethodAttribute(string castFunction) {
      _func = castFunction;
    }

    public string CastFunctionName => _func;

    public static LuaInvocationExpressionSyntax GenerateExpression(AttributeData attributeData, LuaExpressionSyntax invocationArgumentExpression) {
      return new LuaInvocationExpressionSyntax(new LuaIdentifierLiteralExpressionSyntax(attributeData.ConstructorArguments[0].Value.ToString()), invocationArgumentExpression);
    }
  }
}
