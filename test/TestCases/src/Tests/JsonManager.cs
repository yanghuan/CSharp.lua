/*
 * @create:  李锦俊 
 * @email: mybios@qq.com
 * 
 * */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Core {
  // 特定类型的json序列化器
  public interface IJsonSerializer {
    /// <summary>
    /// 创建一个新的脚本对象
    /// </summary>
    /// <returns>返回new出来的脚本对象实例</returns>
    object NewObject();

    bool VisitObject(object value, Func<IJsonSerializer, object, bool> visitor);
  }


  public struct TypeNamePair {
    public TypeNamePair(string d, string n) {
      displayName = d;
      name = n;
    }
    public string displayName;
    public string name;
  }

  public interface IScriptSerializableManager {
    bool TryGetBind(string typeName, out IJsonSerializer cls);

    IJsonSerializer GetJsonSerializer(Type type);
    IJsonSerializer GetJsonSerializer(string typeName);
    IJsonSerializer TryGetJsonSerializer(Type type);
    IJsonSerializer TryGetJsonSerializer(string typeName);

  }


  /// <summary>
  /// 脚本序列化的实现，由脚本层实现具体的接口，并注册到CoreGlobal中
  /// </summary>
  public interface IScriptSerializable {
    // 从脚本层C#里获取一个可序列化的类型实例
    object ResolveScript(string className);

    void PopScriptable();
    List<object> ParserJsonArray(string jsonContent, string classFullName);
    List<T> ParserJsonArray<T>(string jsonContent) where T : class;
    object ParserJsonObject(object jsonData);
    object ParserJson(string jsonContent);
  }

  public class ScriptSerializableManager : IScriptSerializableManager {
    Dictionary<string, IJsonSerializer> _scripts = new Dictionary<string, IJsonSerializer>();

    Dictionary<Type, IJsonSerializer> _serializers = new Dictionary<Type, IJsonSerializer>();

    // 由脚本层实现的跟脚本实例化相关的接口
    // 为了效率，这里直接用静态变量，而没有注册到_coreContainer里
    public IScriptSerializable ScriptSerializable { get; private set; }
    void Awake() {
      RegisterAllJsonSerializers();
    }

    static ScriptSerializableManager _instance;
    public static ScriptSerializableManager Instance {
      get {
        if (_instance == null) {
          _instance = new ScriptSerializableManager();
          _instance.Awake();
        }
        return _instance;
      }
    }

    void RegisterAllJsonSerializers() {
      _serializers.Clear();

    }
    public IJsonSerializer GetJsonSerializer(string typeName) {
      IJsonSerializer ser = TryGetJsonSerializer(typeName);
      if (ser == null) {
        throw new Exception("无法识别的序列化类型：" + typeName);
      }
      return ser;
    }
    public IJsonSerializer GetJsonSerializer(Type type) {
      IJsonSerializer ser = TryGetJsonSerializer(type);
      if (ser == null) {
        throw new Exception("无法识别的序列化类型：" + type.FullName);
      }
      return ser;
    }

    public IJsonSerializer TryGetJsonSerializer(string typeName) {
      IJsonSerializer cls;
      if (_scripts.TryGetValue(typeName, out cls)) {
        return cls;
      } else {

        return ScriptSerializableManager.Instance.AddBind(null);
      }
    }
    public IJsonSerializer TryGetJsonSerializer(Type type) {
      IJsonSerializer ser;
      if (_serializers.TryGetValue(type, out ser)) {
        return ser;
      }


        return ScriptSerializableManager.Instance.AddBind(type);
    }
    public IJsonSerializer AddBind<T>() {
      return AddBind(typeof(T));
    }

    public IJsonSerializer AddBind(Type type) {
      return null;
    }
    public IJsonSerializer AddBind(Type type, IJsonSerializer cls, IJsonSerializer customSerializer) {
      if (_scripts.ContainsKey(type.FullName)) {
        throw new Exception(string.Format("不允许重复添加：{0}", type.FullName));
      }
      _scripts.Add(type.FullName, cls);
      _serializers.Add(type, customSerializer);
      return cls;
    }
    public IJsonSerializer GetBind(string typeName) {
      IJsonSerializer t;
      if (TryGetBind(typeName, out t)) {
        return t;
      } else {
        throw new Exception("找不到类型：" + typeName);
      }
    }
    public bool TryGetBind(string typeName, out IJsonSerializer cls) {
      IJsonSerializer t;
      if (_scripts.TryGetValue(typeName, out t)) {
        cls = t;
        return true;
      } else {
        Type type = typeof(Type);
        if (type == null) {
          cls = null;
          return false;
        }
        cls = AddBind(type);
        return true;
      }
    }
    public bool TryGetBind(Type type, out IJsonSerializer cls) {
      return _serializers.TryGetValue(type, out cls);
    }
  }

}
