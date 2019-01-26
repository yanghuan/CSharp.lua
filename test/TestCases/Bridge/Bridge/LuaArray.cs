/*
 * @create:  李锦俊 
 * @email: mybios@qq.com
 * 在lua版本里，用的是表实现，在C#里用的是List的实现
 * */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class LuaArray<T> {
  List<T> _list = new List<T>();
  public LuaArray() {

  }
  public T this[int index] {
    get {
      return GetValue(index);
    }
    set {
      SetValue(index, value);
    }
  }

  public int Count { get { return _list.Count; } }

  public void Add(T value) { _list.Add(value); }
  public void SetValue(int key, T value) { _list[key] = value; }
  public T GetValue(int key) { return _list[key]; }
  public AsValue GetValue<AsValue>(int key) { return (AsValue)(object)_list[key]; }
  public void ForEach(Action<T> each) {
    for (int i = 0; i < _list.Count; i++) {
      each(_list[i]);
    }
  }
  public T Find(Func<T, bool> each) {
    for (int i = 0; i < _list.Count; i++) {
      if (each(_list[i])) return _list[i];
    }
    return default(T);
  }
  public List<T> ToList() {
    return _list;
  }
  public void Remove(int i) { _list.RemoveAt(i); }
  public void Clear() { _list.Clear(); }
}

public class LuaObject {

  public static implicit operator int(LuaObject self) { return self.GetHashCode(); }
}

public static class LuaArrayExternions {
  public static void DOLocalMove(this LuaObject target, float duration, bool snapping = false) {

  }

  public static LuaObject CreateLuaObjectWithMetaXml() {
    return new LuaObject();
  }

  public static LuaObject CreateLuaObject() {
    return new LuaObject();
  }

}

