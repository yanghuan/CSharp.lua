using System;
using System.Collections.Generic;
using System.Text;

namespace Bridge.Test.NUnit {
  public sealed class Assert {
    public static Action Async() {
      return () => { };
    }

    /// <summary>
    /// A non-strict comparison.
    /// </summary>
    /// <param name="expected">Expected</param>
    /// <param name="actual">Actual</param>
    /// <param name="description">Description</param>
    public static void AreEqual(object expected, object actual, string description = null) {
    }

    /// <summary>
    /// A deep recursive comparison, working on primitive types, arrays, objects, regular expressions, dates and functions.
    /// </summary>
    /// <param name="expected">Expected</param>
    /// <param name="actual">Actual</param>
    /// <param name="description">Description</param>
    public static void AreDeepEqual(object expected, object actual, string description = null) {
    }

    /// <summary>
    /// A deep recursive comparison, working on primitive types, arrays, objects, regular expressions, dates and functions, checking for inequality.
    /// </summary>
    /// <param name="expected">Expected</param>
    /// <param name="actual">Actual</param>
    /// <param name="description">Description</param>
    public static void AreNotDeepEqual(object expected, object actual, string description = null) {
    }

    /// <summary>
    /// A strict type and value comparison.
    /// </summary>
    /// <param name="expected">Expected</param>
    /// <param name="actual">Actual</param>
    /// <param name="description">Description</param>
    public static void AreStrictEqual(object expected, object actual, string description = null) {
    }

    /// <summary>
    /// A non-strict comparison, checking for inequality.
    /// </summary>
    /// <param name="expected">Expected</param>
    /// <param name="actual">Actual</param>
    /// <param name="description">Description</param>
    public static void AreNotEqual(object expected, object actual, string description = null) {
    }

    /// <summary>
    /// A boolean check. Passes if the first argument is truthy.
    /// </summary>
    /// <param name="condition">The value being tested</param>
    /// <param name="description">Description</param>
    public static void True(bool condition, string description = null) {
    }

    /// <summary>
    /// A boolean check. Passes if the first argument is falsy.
    /// </summary>
    /// <param name="condition">The value being tested</param>
    /// <param name="description">Description</param>
    public static void False(bool condition, string description = null) {
    }

    /// <summary>
    /// The <c>Assert.Fail</c> method provides you with the ability to generate a failure based on tests that are not encapsulated by the other methods.
    /// It is also useful in developing your own project-specific assertions.
    /// </summary>
    /// <param name="description">Description</param>
    public static void Fail(string description = null) {
    }

    /// <summary>
    /// The Assert.Throws method is pretty much in a class by itself.
    /// Rather than comparing values, it attempts to invoke a code snippet, represented as a delegate, in order to verify that it throws a particular exception.
    /// </summary>
    /// <param name="block">Delegate which is used to execute the code in question. Under .NET 2.0, this may be an anonymous delegate.</param>
    /// <param name="description">Description</param>
    public static void Throws(Action block, string description = null) {
    }

    /// <summary>
    /// The Assert.Throws method is pretty much in a class by itself.
    /// Rather than comparing values, it attempts to invoke a code snippet, represented as a delegate, in order to verify that it throws a particular exception.
    /// </summary>
    /// <typeparam name="T">Exception to check</typeparam>
    /// <param name="block">Delegate which is used to execute the code in question. Under .NET 2.0, this may be an anonymous delegate.</param>
    /// <param name="description">Description</param>
    public static void Throws<T>(Action block, string description = null) {
    }

    /// <summary>
    /// The Assert.Throws method is pretty much in a class by itself.
    /// Rather than comparing values, it attempts to invoke a code snippet, represented as a delegate, in order to verify that it throws a particular exception.
    /// </summary>
    /// <param name="block">Delegate which is used to execute the code in question. Under .NET 2.0, this may be an anonymous delegate.</param>
    /// <param name="expected">Expected exception</param>
    /// <param name="description">Description</param>
    public static void Throws(Action block, object expected, string description = null) {
    }

    /// <summary>
    /// The Assert.Throws method is pretty much in a class by itself.
    /// Rather than comparing values, it attempts to invoke a code snippet, represented as a delegate, in order to verify that it throws a particular exception.
    /// </summary>
    /// <param name="block">Delegate which is used to execute the code in question. Under .NET 2.0, this may be an anonymous delegate.</param>
    /// <param name="expected"> callback Function that must return true to pass the assertion check.</param>
    /// <param name="description">Description</param>
    public static void Throws(Action block, Func<object, bool> expected, string description = null) {
    }

    /// <summary>
    /// Checks if an object is null.
    /// </summary>
    /// <param name="anObject">An object being tested</param>
    /// <param name="description">Description</param>
    public static void Null(object anObject, string description = null) {
    }

    /// <summary>
    /// Checks if an object is not null.
    /// </summary>
    /// <param name="anObject">An object being tested</param>
    /// <param name="description">Description</param>
    public static void NotNull(object anObject, string description = null) {
    }
  }
}
