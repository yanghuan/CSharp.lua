using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;

public static class Extension {
  private static string ToIntegerFormat(int format) {
    if (format == 10) {
      return "D";
    }
    if (format == 16) {
      return "x";
    }
    return "";
  }

  public static string ToString(this sbyte b, int format) {
    return b.ToString(ToIntegerFormat(format));
  }

  public static string ToString(this byte b, int format) {
    return b.ToString(ToIntegerFormat(format));
  }

  public static string ToString(this ushort v, int format) {
    return v.ToString(ToIntegerFormat(format));
  }

  public static string ToString(this short v, int format) {
    return v.ToString(ToIntegerFormat(format));
  }

  public static string ToString(this int v, int format) {
    return v.ToString(ToIntegerFormat(format));
  }

  public static string ToString(this uint v, int format) {
    return v.ToString(ToIntegerFormat(format));
  }

  public static string ToString(this long v, int format) {
    return v.ToString(ToIntegerFormat(format));
  }

  public static string ToString(this ulong v, int format) {
    return v.ToString(ToIntegerFormat(format));
  }

  public static string ToPrecision(this float v, int decimals = 0) {
    return v.ToString(ToIntegerFormat(decimals));
  }

  private static string ToSingleFormat(int format) {
    return "f" + format;
  }

  public static string ToFixed(this float v, int decimals = 0) {
    return v.ToString(ToSingleFormat(decimals));
  }

  private static string ToExponentialFormat(int format) {
    if (format == 0) {
      return "e";
    }
    return "e" + format;
  }

  public static string ToExponential(this float v, int decimals = 0) {
    return v.ToString(ToExponentialFormat(decimals));
  }

  public static string ToFixed(this decimal v, int decimals, MidpointRounding midpointRounding) {
    return v.ToString();
  }

  public static int ValueOf(this ValueType v) {
    return (int)v;
  }

  public static T As<T>(this object t) {
    return (T)t;
  }

  public static T[] Filter<T>(this IEnumerable<T> source, Func<T, bool> action) {
    return source.Where(action).ToArray();
  }

  public static R[] Map<T, R>(this IEnumerable<T> source, Func<T, R> selector) {
    return source.Select(selector).ToArray();
  }

  public static bool Contains<T>(this IEnumerable<T> source, T value) {
    return Enumerable.Contains(source, value);
  }

  public static void ForEach<T>(this T[] array, Action<T> action) {
    Array.ForEach(array, action);
  }
  
  public static int IndexOf<T>(this T[] array, T v) {
    return Array.IndexOf(array, v);
  }

  public static string Join(this IEnumerable<object> values, string separator) {
   return string.Join(separator, values);
  }

  public static void Sort<T>(this T[] array) {
    Array.Sort(array);
  }

  public static T[] Concat<T>(this T[] array, params T[] v) {
    if (v.Length == 1 && v[0] == null) {
      return array;
    }

    T[] newArray = new T[array.Length + v.Length];
    Array.Copy(array, newArray, array.Length);
    Array.Copy(v, 0, newArray, array.Length, v.Length);
    return newArray;
  }

  public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
    foreach (var i in source) {
      action(i);
    }
  }

  public static bool Some<T>(this IEnumerable<T> source, Func<T, bool> predicate) {
    return source.Any(predicate);
  }

  public static int LastIndexOfAny(this string s, params char[] c) {
    return s.LastIndexOfAny(c);
  }

  public static MethodInfo GetMethod(this Type t, string name, BindingFlags bindingAttr, Type[] types) {
    return t.GetMethod(name, bindingAttr, null, types, null);
  }

  public static PropertyInfo GetProperty(this Type t, string name, BindingFlags bindingAttr, Type[] types) {
    return t.GetProperty(name, bindingAttr, null, typeof(object), types, null);
  }
}

public static class Script {
  public static object ToPlainObject(object o) {
    return o;
  }

  public static void Write(string message) {
    Console.WriteLine(message);
  }

  public static T CreateInstance<T>(params object[] args) {
    return (T)Activator.CreateInstance(typeof(T), args);
  }
}

namespace Bridge {
  public static class Browser {
    public static bool IsChrome = false;
    public static int ChromeVersion = 0;
  }
}

