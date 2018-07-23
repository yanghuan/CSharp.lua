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

      public sealed class PropertyModel {
        [XmlAttribute]
        public string name;
        [XmlAttribute]
        public string Name;
        [XmlElement]
        public TemplateModel set;
        [XmlElement]
        public TemplateModel get;
        [XmlAttribute]
        public string IsField;
        [XmlAttribute]
        public bool Baned;

        public bool? CheckIsField {
          get {
            if (IsField != null) {
              if (IsField.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase)) {
                return true;
              }
              if (IsField.Equals(bool.FalseString, StringComparison.OrdinalIgnoreCase)) {
                return false;
              }
            }
            return null;
          }
        }
      }

      public sealed class FieldModel {
        [XmlAttribute]
        public string name;
        [XmlAttribute]
        public string Template;
        [XmlAttribute]
        public bool Baned;
      }

      public sealed class ArgumentModel {
        [XmlAttribute]
        public string type;
        [XmlElement("arg")]
        public ArgumentModel[] GenericArgs;
      }

      public sealed class MethodModel {
        [XmlAttribute]
        public string name;
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
        [XmlAttribute]
        public bool Baned;
      }

      public sealed class ClassModel {
        [XmlAttribute]
        public string name;
        [XmlAttribute]
        public string Name;
        [XmlElement("property")]
        public PropertyModel[] Propertys;
        [XmlElement("field")]
        public FieldModel[] Fields;
        [XmlElement("method")]
        public MethodModel[] Methods;
        [XmlAttribute]
        public bool Baned;
      }

      public sealed class NamespaceModel {
        [XmlAttribute]
        public string name;
        [XmlAttribute]
        public string Name;
        [XmlElement("class")]
        public ClassModel[] Classes;
        [XmlAttribute]
        public bool Baned;
      }

      public sealed class AssemblyModel {
        [XmlElement("namespace")]
        public NamespaceModel[] Namespaces;
        [XmlElement("class")]
        public ClassModel[] Classes;
      }

      public sealed class ExportModel {
        public sealed class AttributeModel {
          [XmlAttribute("name")]
          public string Name;
        }
        [XmlElement("attribute")]
        public AttributeModel[] Attributes;
      }

      [XmlElement("assembly")]
      public AssemblyModel Assembly;

      [XmlElement("export")]
      public ExportModel Export;
    }

    private enum MethodMetaType {
      Name,
      CodeTemplate,
      IgnoreGeneric,
    }

    private sealed class MethodMetaInfo {
      private List<XmlMetaModel.MethodModel> models_ = new List<XmlMetaModel.MethodModel>();
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
        StringBuilder sb = new StringBuilder();
        INamedTypeSymbol typeSymbol = (INamedTypeSymbol)symbol.OriginalDefinition;
        var namespaceSymbol = typeSymbol.ContainingNamespace;
        if (!namespaceSymbol.IsGlobalNamespace) {
          sb.Append(namespaceSymbol.ToString());
          sb.Append('.');
        }
        sb.Append(symbol.Name);
        if (typeSymbol.TypeArguments.Length > 0) {
          sb.Append('^');
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

      private string GetName(IMethodSymbol symbol) {
        XmlMetaModel.MethodModel methodModel;
        if (isSingleModel_) {
          methodModel = models_.First();
        } else {
          methodModel = models_.Find(i => IsMethodMatch(i, symbol));
        }
        if (methodModel != null && methodModel.Baned) {
          throw new CompilationErrorException($"{symbol.ContainingType.Name}.{symbol.Name} is baned");
        }
        return methodModel?.Name;
      }

      private string GetCodeTemplate(IMethodSymbol symbol) {
        if (isSingleModel_) {
          return models_.First().Template;
        }

        var methodModel = models_.Find(i => IsMethodMatch(i, symbol));
        if (methodModel != null && methodModel.Baned) {
          throw new CompilationErrorException($"{symbol.ContainingType.Name}.{symbol.Name} is baned");
        }
        return methodModel?.Template;
      }

      private string GetIgnoreGeneric(IMethodSymbol symbol) {
        bool isIgnoreGeneric = false;
        XmlMetaModel.MethodModel methodModel;
        if (isSingleModel_) {
          methodModel = models_.First();
          isIgnoreGeneric = methodModel.IgnoreGeneric;
        } else {
          methodModel = models_.Find(i => IsMethodMatch(i, symbol));
          if (methodModel != null) {
            isIgnoreGeneric = methodModel.IgnoreGeneric;
          }
        }
        if (methodModel != null && methodModel.Baned) {
          throw new CompilationErrorException($"{symbol.ContainingType.Name}.{symbol.Name} is baned");
        }
        return isIgnoreGeneric ? bool.TrueString : bool.FalseString;
      }

      public string GetMetaInfo(IMethodSymbol symbol, MethodMetaType type) {
        switch (type) {
          case MethodMetaType.Name: {
              return GetName(symbol);
            }
          case MethodMetaType.CodeTemplate: {
              return GetCodeTemplate(symbol);
            }
          case MethodMetaType.IgnoreGeneric: {
              return GetIgnoreGeneric(symbol);
            }
          default: {
              throw new InvalidOperationException();
            }
        }
      }
    }

    private sealed class TypeMetaInfo {
      private XmlMetaModel.ClassModel model_;
      private Dictionary<string, XmlMetaModel.FieldModel> fields_ = new Dictionary<string, XmlMetaModel.FieldModel>();
      private Dictionary<string, XmlMetaModel.PropertyModel> propertys_ = new Dictionary<string, XmlMetaModel.PropertyModel>();
      private Dictionary<string, MethodMetaInfo> methods_ = new Dictionary<string, MethodMetaInfo>();

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

    private Dictionary<string, XmlMetaModel.NamespaceModel> namespaceNameMaps_ = new Dictionary<string, XmlMetaModel.NamespaceModel>();
    private Dictionary<string, TypeMetaInfo> typeMetas_ = new Dictionary<string, TypeMetaInfo>();
    private HashSet<string> exportAttributes_ = new HashSet<string>();

    public XmlMetaProvider(IEnumerable<string> files) {
      foreach (string file in files) {
        XmlSerializer xmlSeliz = new XmlSerializer(typeof(XmlMetaModel));
        try {
          using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read)) {
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
            var export = model.Export;
            if (export != null) {
              if (export.Attributes != null) {
                foreach (var attribute in export.Attributes) {
                  if (string.IsNullOrEmpty(attribute.Name)) {
                    throw new ArgumentException("attribute's name is empty");
                  }
                  exportAttributes_.Add(attribute.Name);
                }
              }
            }
          }
        } catch (Exception e) {
          throw new Exception($"load xml file wrong at {file}", e);
        }
      }
    }

    private void LoadNamespace(XmlMetaModel.NamespaceModel model) {
      string namespaceName = model.name;
      if (namespaceName == null) {
        throw new ArgumentException("namespace's name is null");
      }

      if (namespaceName.Length > 0 && !string.IsNullOrEmpty(model.Name)) {
        if (namespaceNameMaps_.ContainsKey(namespaceName)) {
          throw new ArgumentException($"namespace [{namespaceName}] is already has");
        }
        namespaceNameMaps_.Add(namespaceName, model);
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
        classesfullName = classesfullName.Replace('^', '_');
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
        if (info.Baned) {
          throw new CompilationErrorException($"{symbol.ToString()} is baned");
        }
        return info.Name;
      }
      return null;
    }

    internal bool MayHaveCodeMeta(ISymbol symbol) {
      return symbol.DeclaredAccessibility == Accessibility.Public && !symbol.IsFromCode();
    }

    private string GetTypeShortString(ISymbol symbol) {
      INamedTypeSymbol typeSymbol = (INamedTypeSymbol)symbol.OriginalDefinition;
      return typeSymbol.GetTypeShortName(GetNamespaceMapName);
    }

    internal string GetTypeMapName(ISymbol symbol, string shortName) {
      if (MayHaveCodeMeta(symbol)) {
        TypeMetaInfo info = typeMetas_.GetOrDefault(shortName);
        if (info != null && info.Model.Baned) {
          throw new CompilationErrorException($"{symbol.ContainingType?.Name}.{symbol.Name} is baned");
        }
        return info?.Model.Name;
      }
      return null;
    }

    private TypeMetaInfo GetTypeMetaInfo(ISymbol memberSymbol) {
      string typeName = GetTypeShortString(memberSymbol.ContainingType);
      return typeMetas_.GetOrDefault(typeName);
    }

    public bool? IsPropertyField(IPropertySymbol symbol) {
      if (MayHaveCodeMeta(symbol)) {
        var info = GetTypeMetaInfo(symbol)?.GetPropertyModel(symbol.Name);
        return info?.CheckIsField;
      }
      return null;
    }

    public string GetFieldCodeTemplate(IFieldSymbol symbol) {
      if (MayHaveCodeMeta(symbol)) {
        var info = GetTypeMetaInfo(symbol)?.GetFieldModel(symbol.Name);
        if (info != null && info.Baned) {
          throw new CompilationErrorException($"{symbol.ContainingType.Name}.{symbol.Name} is baned");
        }
        return info?.Template;
      }
      return null;
    }

    public string GetProertyCodeTemplate(IPropertySymbol symbol, bool isGet) {
      if (MayHaveCodeMeta(symbol)) {
        var info = GetTypeMetaInfo(symbol)?.GetPropertyModel(symbol.Name);
        if (info != null) {
          if (info.Baned) {
            throw new CompilationErrorException($"{symbol.ContainingType.Name}.{symbol.Name} is baned");
          }
          return isGet ? info.get?.Template : info.set?.Template;
        }
      }
      return null;
    }

    private string GetInternalMethodMetaInfo(IMethodSymbol symbol, MethodMetaType metaType) {
      Contract.Assert(symbol != null);
      if (symbol.DeclaredAccessibility != Accessibility.Public) {
        return null;
      }

      string codeTemplate = null;
      if (!symbol.IsFromCode()) {
        codeTemplate = GetTypeMetaInfo(symbol)?.GetMethodMetaInfo(symbol.Name)?.GetMetaInfo(symbol, metaType);
      }

      if (codeTemplate == null) {
        if (symbol.IsOverride) {
          if (symbol.OverriddenMethod != null) {
            codeTemplate = GetInternalMethodMetaInfo(symbol.OverriddenMethod, metaType);
          }
        } else {
          var interfaceImplementations = symbol.InterfaceImplementations();
          if (interfaceImplementations != null) {
            foreach (IMethodSymbol interfaceMethod in interfaceImplementations) {
              codeTemplate = GetInternalMethodMetaInfo(interfaceMethod, metaType);
              if (codeTemplate != null) {
                break;
              }
            }
          }
        }
      }
      return codeTemplate;
    }

    private string GetMethodMetaInfo(IMethodSymbol symbol, MethodMetaType metaType) {
      Utility.CheckMethodDefinition(ref symbol);
      return GetInternalMethodMetaInfo(symbol, metaType);
    }

    public string GetMethodMapName(IMethodSymbol symbol) {
      return GetMethodMetaInfo(symbol, MethodMetaType.Name);
    }

    public string GetMethodCodeTemplate(IMethodSymbol symbol) {
      return GetMethodMetaInfo(symbol, MethodMetaType.CodeTemplate);
    }

    public bool IsMethodIgnoreGeneric(IMethodSymbol symbol) {
      return GetMethodMetaInfo(symbol, MethodMetaType.IgnoreGeneric) == bool.TrueString;
    }

    public bool IsExportAttribute(INamedTypeSymbol attributeTypeSymbol) {
      return exportAttributes_.Count > 0 && exportAttributes_.Contains(attributeTypeSymbol.ToString());
    }
  }
}
