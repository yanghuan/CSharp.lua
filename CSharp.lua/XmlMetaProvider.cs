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
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.CodeAnalysis;

namespace CSharpLua {
  public sealed class XmlMetaProvider {
    [XmlRoot("meta")]
    public sealed class XmlMetaModel {
      public sealed class TemplateModel {
        [XmlAttribute]
        public string Template;
      }

      public class MemberModel {
        [XmlAttribute]
        public string name;

        [XmlAttribute]
        public string Baned;

        protected static bool TryTryParseBool(string v, out bool b) {
          b = false;
          if (v != null) {
            if (v.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase)) {
              b = true;
              return true;
            }
            if (v.Equals(bool.FalseString, StringComparison.OrdinalIgnoreCase)) {
              b = false;
              return false;
            }
          }
          return false;
        }

        internal bool IsBaned {
          get {
            if (!string.IsNullOrEmpty(Baned)) {
              if (TryTryParseBool(Baned, out bool b)) {
                return b;
              }
              return true;
            }
            return false;
          }
        }

        private string BanedMessage {
          get {
            if (!string.IsNullOrEmpty(Baned)) {
              if (TryTryParseBool(Baned, out bool b)) {
                return b ? "cannot use" : null;
              }
              return Baned;
            }
            return null;
          }
        }

        public void CheckBaned(ISymbol symbol) {
          if (IsBaned) {
            throw new CompilationErrorException($"{symbol} is baned, {BanedMessage}");
          }
        }
      }

      public sealed class PropertyModel : MemberModel {
        [XmlAttribute]
        public string Name;
        [XmlElement]
        public TemplateModel set;
        [XmlElement]
        public TemplateModel get;
        [XmlAttribute]
        public string IsField;

        public bool? CheckIsField {
          get {
            if (TryTryParseBool(IsField, out bool b)) {
              return b;
            }
            return null;
          }
        }
      }

      public sealed class FieldModel : MemberModel {
        [XmlAttribute]
        public string Template;
        [XmlAttribute]
        public bool IsProperty;
      }

      public sealed class ArgumentModel {
        [XmlAttribute]
        public string type;
        [XmlElement("arg")]
        public ArgumentModel[] GenericArgs;
      }

      public sealed class MethodModel : MemberModel {
        [XmlAttribute]
        public string Name;
        [XmlAttribute]
        public string Template;
        [XmlAttribute]
        public int ArgCount = -1;
        [XmlElement("arg")]
        public ArgumentModel[] Args;
        [XmlAttribute]
        public string RetType;
        [XmlAttribute]
        public int GenericArgCount = -1;
        [XmlAttribute]
        public bool IgnoreGeneric;

        internal string GetMetaInfo(MethodMetaType type) {
          switch (type) {
            case MethodMetaType.Name: {
              return Name;
            }
            case MethodMetaType.CodeTemplate: {
              return Template;
            }
            case MethodMetaType.IgnoreGeneric: {
              return IgnoreGeneric ? bool.TrueString : bool.FalseString;
            }
            default: {
              throw new InvalidOperationException();
            }
          }
        }
      }

      public sealed class ClassModel : MemberModel {
        [XmlAttribute]
        public string Name;
        [XmlElement("property")]
        public PropertyModel[] Propertys;
        [XmlElement("field")]
        public FieldModel[] Fields;
        [XmlElement("method")]
        public MethodModel[] Methods;
        [XmlAttribute]
        public bool IgnoreGeneric;
        [XmlAttribute]
        public bool Readonly;
      }

      public sealed class NamespaceModel : MemberModel {
        [XmlAttribute]
        public string Name;
        [XmlElement("class")]
        public ClassModel[] Classes;
      }

      public sealed class AssemblyModel {
        [XmlElement("namespace")]
        public NamespaceModel[] Namespaces;
        [XmlElement("class")]
        public ClassModel[] Classes;
      }

      [XmlElement("assembly")]
      public AssemblyModel Assembly;
    }

    internal enum MethodMetaType {
      Name,
      CodeTemplate,
      IgnoreGeneric,
    }

    private sealed class MethodMetaInfo {
      private readonly List<XmlMetaModel.MethodModel> models_ = new List<XmlMetaModel.MethodModel>();
      private bool isSingleModel_;

      public void Add(XmlMetaModel.MethodModel model) {
        models_.Add(model);
        CheckIsSingleModel();
      }

      private void CheckIsSingleModel() {
        bool isSingle = false;
        if (models_.Count == 1) {
          var model = models_.First();
          if (model.ArgCount == -1 && model.Args == null && model.RetType == null && model.GenericArgCount == -1) {
            isSingle = true;
          }
        }
        isSingleModel_ = isSingle;
      }

      private static string GetTypeString(ITypeSymbol symbol) {
        if (symbol.Kind == SymbolKind.TypeParameter) {
          return symbol.Name;
        }

        StringBuilder sb = new StringBuilder();
        INamedTypeSymbol typeSymbol = (INamedTypeSymbol)symbol.OriginalDefinition;
        var namespaceSymbol = typeSymbol.ContainingNamespace;
        
        if(symbol.ContainingType != null) {
          sb.Append(GetTypeString(symbol.ContainingType));
          sb.Append('.');
        }
        else if (!namespaceSymbol.IsGlobalNamespace) {
          sb.Append(namespaceSymbol.ToString());
          sb.Append('.');
        }
        sb.Append(symbol.Name);
        if (typeSymbol.TypeArguments.Length > 0) {
          sb.Append('`');
          sb.Append(typeSymbol.TypeArguments.Length);
        }
        return sb.ToString();
      }

      private static bool IsTypeMatch(ITypeSymbol symbol, string typeString) {
        if (symbol.Kind == SymbolKind.ArrayType) {
          var typeSymbol = (IArrayTypeSymbol)symbol;
          string elementTypeName = GetTypeString(typeSymbol.ElementType);
          return elementTypeName + "[]" == typeString;
        } else {
          string name = GetTypeString(symbol);
          return name == typeString;
        }
      }

      private static bool IsArgMatch(ITypeSymbol symbol, XmlMetaModel.ArgumentModel parameterModel) {
        if (!IsTypeMatch(symbol, parameterModel.type)) {
          return false;
        }

        if (parameterModel.GenericArgs != null) {
          var typeSymbol = (INamedTypeSymbol)symbol;
          if (typeSymbol.TypeArguments.Length != parameterModel.GenericArgs.Length) {
            return false;
          }

          int index = 0;
          foreach (var typeArgument in typeSymbol.TypeArguments) {
            var genericArgModel = parameterModel.GenericArgs[index];
            if (!IsArgMatch(typeArgument, genericArgModel)) {
              return false;
            }
            ++index;
          }
        }

        return true;
      }

      private bool IsMethodMatch(XmlMetaModel.MethodModel model, IMethodSymbol symbol) {
        if (model.name != symbol.Name) {
          return false;
        }

        if (model.ArgCount != -1) {
          if (symbol.Parameters.Length != model.ArgCount) {
            return false;
          }
        }

        if (model.GenericArgCount != -1) {
          if (symbol.TypeArguments.Length != model.GenericArgCount) {
            return false;
          }
        }

        if (!string.IsNullOrEmpty(model.RetType)) {
          if (!IsTypeMatch(symbol.ReturnType, model.RetType)) {
            return false;
          }
        }

        if (model.Args != null) {
          if (symbol.Parameters.Length != model.Args.Length) {
            return false;
          }

          int index = 0;
          foreach (var parameter in symbol.Parameters) {
            var parameterModel = model.Args[index];
            if (!IsArgMatch(parameter.Type, parameterModel)) {
              return false;
            }
            ++index;
          }
        }

        return true;
      }

      private XmlMetaModel.MethodModel GetMethodModel(IMethodSymbol symbol, bool isCheckBaned) {
        XmlMetaModel.MethodModel methodModel;
        if (isSingleModel_) {
          methodModel = models_.First();
        } else {
          methodModel = models_.Find(i => IsMethodMatch(i, symbol));
        }
        if (methodModel != null && isCheckBaned) {
          methodModel.CheckBaned(symbol);
        }
        return methodModel;
      }

      public string GetMetaInfo(IMethodSymbol symbol, MethodMetaType type) {
        return GetMethodModel(symbol, type == MethodMetaType.CodeTemplate)?.GetMetaInfo(type);
      }
    }

    private sealed class TypeMetaInfo {
      private readonly XmlMetaModel.ClassModel model_;
      private readonly Dictionary<string, XmlMetaModel.FieldModel> fields_ = new Dictionary<string, XmlMetaModel.FieldModel>();
      private readonly Dictionary<string, XmlMetaModel.PropertyModel> propertys_ = new Dictionary<string, XmlMetaModel.PropertyModel>();
      private readonly Dictionary<string, MethodMetaInfo> methods_ = new Dictionary<string, MethodMetaInfo>();

      public TypeMetaInfo(XmlMetaModel.ClassModel model) {
        model_ = model;
        Field();
        Property();
        Method();
      }

      public XmlMetaModel.ClassModel Model {
        get {
          return model_;
        }
      }

      private void Field() {
        if (model_.Fields != null) {
          foreach (var fieldModel in model_.Fields) {
            if (string.IsNullOrEmpty(fieldModel.name)) {
              throw new ArgumentException($"type [{model_.name}] has a field name is empty");
            }

            if (fields_.ContainsKey(fieldModel.name)) {
              throw new ArgumentException($"type [{model_.name}]'s field [{fieldModel.name}] is already exists");
            }
            fields_.Add(fieldModel.name, fieldModel);
          }
        }
      }

      private void Property() {
        if (model_.Propertys != null) {
          foreach (var propertyModel in model_.Propertys) {
            if (string.IsNullOrEmpty(propertyModel.name)) {
              throw new ArgumentException($"type [{model_.name}] has a property name is empty");
            }

            if (fields_.ContainsKey(propertyModel.name)) {
              throw new ArgumentException($"type [{model_.name}]'s property [{propertyModel.name}] is already exists");
            }
            propertys_.Add(propertyModel.name, propertyModel);
          }
        }
      }

      private void Method() {
        if (model_.Methods != null) {
          foreach (var methodModel in model_.Methods) {
            if (string.IsNullOrEmpty(methodModel.name)) {
              throw new ArgumentException($"type [{model_.name}] has a method name is empty");
            }

            var info = methods_.GetOrDefault(methodModel.name);
            if (info == null) {
              info = new MethodMetaInfo();
              methods_.Add(methodModel.name, info);
            }
            info.Add(methodModel);
          }
        }
      }

      public XmlMetaModel.FieldModel GetFieldModel(string name) {
        return fields_.GetOrDefault(name);
      }

      public XmlMetaModel.PropertyModel GetPropertyModel(string name) {
        return propertys_.GetOrDefault(name);
      }

      public MethodMetaInfo GetMethodMetaInfo(string name) {
        return methods_.GetOrDefault(name);
      }
    }

    private readonly Dictionary<string, XmlMetaModel.NamespaceModel> namespaceNameMaps_ = new Dictionary<string, XmlMetaModel.NamespaceModel>();
    private readonly Dictionary<string, TypeMetaInfo> typeMetas_ = new Dictionary<string, TypeMetaInfo>();

    public XmlMetaProvider(IEnumerable<Stream> streams) {
      foreach (Stream stream in streams) {
        XmlSerializer xmlSeliz = new XmlSerializer(typeof(XmlMetaModel));
        try {
          XmlMetaModel model = (XmlMetaModel)xmlSeliz.Deserialize(stream);
          var assembly = model.Assembly;
          if (assembly != null) {
            if (assembly.Namespaces != null) {
              foreach (var namespaceModel in assembly.Namespaces) {
                LoadNamespace(namespaceModel);
              }
            }
            if (assembly.Classes != null) {
              LoadType(string.Empty, assembly.Classes);
            }
          }
        } catch (Exception e) {
          throw new Exception($"load xml file wrong {0}", e);
        }
      }
    }

    private void LoadNamespace(XmlMetaModel.NamespaceModel model) {
      string namespaceName = model.name;
      if (namespaceName == null) {
        throw new ArgumentException("namespace's name is null");
      }

      if (namespaceName.Length > 0) {
        if (namespaceNameMaps_.ContainsKey(namespaceName)) {
          throw new ArgumentException($"namespace [{namespaceName}] is already has");
        }
        if (!string.IsNullOrEmpty(model.Name) || model.IsBaned) {
          namespaceNameMaps_.Add(namespaceName, model);
        }
      }

      if (model.Classes != null) {
        string name = !string.IsNullOrEmpty(model.Name) ? model.Name : namespaceName;
        LoadType(name, model.Classes);
      }
    }

    private void LoadType(string namespaceName, XmlMetaModel.ClassModel[] classes) {
      foreach (var classModel in classes) {
        string className = classModel.name;
        if (string.IsNullOrEmpty(className)) {
          throw new ArgumentException($"namespace [{namespaceName}] has a class's name is empty");
        }

        string classesfullName = namespaceName.Length > 0 ? namespaceName + '.' + className : className;
        classesfullName = classesfullName.Replace('`', '_');
        if (typeMetas_.ContainsKey(classesfullName)) {
          throw new ArgumentException($"type [{classesfullName}] is already has");
        }
        TypeMetaInfo info = new TypeMetaInfo(classModel);
        typeMetas_.Add(classesfullName, info);
      }
    }

    public string GetNamespaceMapName(INamespaceSymbol symbol, string original) {
      var info = namespaceNameMaps_.GetOrDefault(original);
      if (info != null) {
        info.CheckBaned(symbol);
        return info.Name;
      }
      return null;
    }

    internal bool MayHaveCodeMeta(ISymbol symbol) {
      return symbol.DeclaredAccessibility == Accessibility.Public && symbol.IsFromAssembly();
    }

    private string GetTypeShortString(ISymbol symbol) {
      INamedTypeSymbol typeSymbol = (INamedTypeSymbol)symbol.OriginalDefinition;
      return typeSymbol.GetTypeShortName(GetNamespaceMapName);
    }

    internal string GetTypeMapName(ISymbol symbol, string shortName) {
      if (MayHaveCodeMeta(symbol)) {
        var info = GetTypeMetaInfo(symbol, shortName);
        return info?.Model.Name;
      }
      return null;
    }

    internal bool IsTypeIgnoreGeneric(INamedTypeSymbol typeSymbol) {
      if (MayHaveCodeMeta(typeSymbol)) {
        var info = GetTypeMetaInfo(typeSymbol);
        return info != null && info.Model.IgnoreGeneric;
      }
      return false;
    }

    internal bool IsTypeReadOnly(INamedTypeSymbol typeSymbol) {
      if (MayHaveCodeMeta(typeSymbol)) {
        var info = GetTypeMetaInfo(typeSymbol);
        return info != null && info.Model.Readonly;
      }
      return false;
    }

    private TypeMetaInfo GetTypeMetaInfo(ISymbol symbol, string shortName) {
      var info = typeMetas_.GetOrDefault(shortName);
      if (info != null) {
        info.Model.CheckBaned(symbol);
      }
      return info;
    }

    private TypeMetaInfo GetTypeMetaInfo(INamedTypeSymbol typeSymbol) {
      string shortName = GetTypeShortString(typeSymbol);
      return GetTypeMetaInfo(typeSymbol, shortName);
    }

    private TypeMetaInfo GetTypeMetaInfo(ISymbol memberSymbol) {
      return GetTypeMetaInfo(memberSymbol.ContainingType);
    }

    private XmlMetaModel.FieldModel GetFieldMetaInfo(IFieldSymbol symbol) {
      if (MayHaveCodeMeta(symbol)) {
        return GetTypeMetaInfo(symbol)?.GetFieldModel(symbol.Name);
      }
      return null;
    }

    public string GetFieldCodeTemplate(IFieldSymbol symbol) {
      return GetFieldMetaInfo(symbol)?.Template ?? symbol.GetCodeTemplateFromAttribute();
    }

    public bool IsFieldForceProperty(IFieldSymbol symbol) {
      return GetFieldMetaInfo(symbol)?.IsProperty ?? false;
    }

    private XmlMetaModel.PropertyModel GetPropertyMetaInfo(IPropertySymbol symbol) {
      if (MayHaveCodeMeta(symbol)) {
        return GetTypeMetaInfo(symbol)?.GetPropertyModel(symbol.Name);
      }
      return null;
    }

    public bool? IsPropertyField(IPropertySymbol symbol) {
      return GetPropertyMetaInfo(symbol)?.CheckIsField;
    }

    public string GetProertyCodeTemplate(IPropertySymbol symbol, bool isGet) {
      var info = GetPropertyMetaInfo(symbol);
      if (info != null) {
        info.CheckBaned(symbol);
        return isGet ? info.get?.Template : info.set?.Template;
      }
      return null;
    }

    private string GetInternalMethodMetaInfo(IMethodSymbol symbol, MethodMetaType metaType) {
      Contract.Assert(symbol != null);
      if (!symbol.IsPublic()) {
        return null;
      }

      string metaInfo = null;
      if (symbol.IsFromAssembly()) {
        metaInfo = GetTypeMetaInfo(symbol)?.GetMethodMetaInfo(symbol.Name)?.GetMetaInfo(symbol, metaType);
      }

      if (metaInfo == null) {
        if (symbol.IsOverride) {
          if (symbol.OverriddenMethod != null) {
            metaInfo = GetInternalMethodMetaInfo(symbol.OverriddenMethod, metaType);
          }
        } else {
          var interfaceImplementations = symbol.InterfaceImplementations();
          if (interfaceImplementations != null) {
            foreach (IMethodSymbol interfaceMethod in interfaceImplementations) {
              metaInfo = GetInternalMethodMetaInfo(interfaceMethod, metaType);
              if (metaInfo != null) {
                break;
              }
            }
          }
        }
      }
      return metaInfo;
    }

    private string GetMethodMetaInfo(IMethodSymbol symbol, MethodMetaType metaType) {
      Utility.CheckMethodDefinition(ref symbol);
      return GetInternalMethodMetaInfo(symbol, metaType);
    }

    public string GetMethodMapName(IMethodSymbol symbol) {
      return GetMethodMetaInfo(symbol, MethodMetaType.Name);
    }

    public string GetMethodCodeTemplate(IMethodSymbol symbol) {
      return GetMethodMetaInfo(symbol, MethodMetaType.CodeTemplate) ?? symbol.GetCodeTemplateFromAttribute();
    }

    public bool IsMethodIgnoreGeneric(IMethodSymbol symbol) {
      return GetMethodMetaInfo(symbol, MethodMetaType.IgnoreGeneric) == bool.TrueString;
    }
  }
}
