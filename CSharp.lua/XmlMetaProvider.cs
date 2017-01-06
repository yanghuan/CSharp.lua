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
        [XmlRoot("assembly")]
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
                [XmlAttribute]
                public bool IsAutoField;
                [XmlElement]
                public TemplateModel set;
                [XmlElement]
                public TemplateModel get;
            }

            public sealed class FieldModel {
                [XmlAttribute]
                public string name;
                [XmlAttribute]
                public string Template;
            }

            public sealed class ArgumentModel {
                [XmlAttribute]
                public string type;
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
                public string Import;
            }

            public sealed class NamespaceModel {
                [XmlAttribute]
                public string name;
                [XmlAttribute]
                public string Name;
                [XmlElement("class")]
                public ClassModel[] Classes;
            }

            [XmlElement("namespace")]
            public NamespaceModel[] Namespaces;
        }

        private sealed class MethodMetaInfo {
            private List<XmlMetaModel.MethodModel> models_ = new List<XmlMetaModel.MethodModel>();

            public void Add(XmlMetaModel.MethodModel model) {
                models_.Add(model);
            }

            private bool IsTypeMatch(ITypeSymbol symbol, string typeString) {
                INamedTypeSymbol typeSymbol = (INamedTypeSymbol)symbol.OriginalDefinition;
                string namespaceName = typeSymbol.ContainingNamespace.ToString();
                string name;
                if(typeSymbol.TypeArguments.Length == 0) {
                    name = $"{namespaceName}.{symbol.Name}";
                }
                else {
                    name = $"{namespaceName}.{symbol.Name}^{typeSymbol.TypeArguments.Length}";
                }
                return name == typeString;
            }

            private bool IsMethodMatch(XmlMetaModel.MethodModel model, IMethodSymbol symbol) {
                if(model.name != symbol.Name) {
                    return false;
                }

                if(model.ArgCount != -1) {
                    if(symbol.Parameters.Length != model.ArgCount) {
                        return false;
                    }
                }

                if(model.GenericArgCount != -1) {
                    if(symbol.TypeArguments.Length != model.GenericArgCount) {
                        return false;
                    }
                }

                if(!string.IsNullOrEmpty(model.RetType)) {
                    if(!IsTypeMatch(symbol.ReturnType, model.RetType)) {
                        return false;
                    }
                }

                if(model.Args != null) {
                    if(symbol.Parameters.Length != model.Args.Length) {
                        return false;
                    }

                    int index = 0;
                    foreach(var parameter in symbol.Parameters) {
                        var parameterModel = model.Args[index];
                        if(!IsTypeMatch(parameter.Type, parameterModel.type)) {
                            return false;
                        }
                        ++index;
                    }
                }

                return true;
            }

            public string GetName(IMethodSymbol symbol) {
                if(models_.Count == 1) {
                    return models_.First().Name;
                }

                var methodModel = models_.Find(i => IsMethodMatch(i, symbol));
                return methodModel?.Name;
            }

            internal string GetCodeTemplate(IMethodSymbol symbol) {
                if(models_.Count == 1) {
                    return models_.First().Template;
                }

                var methodModel = models_.Find(i => IsMethodMatch(i, symbol));
                return methodModel?.Template;
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
                if(model_.Fields != null) {
                    foreach(var fieldModel in model_.Fields) {
                        if(string.IsNullOrEmpty(fieldModel.name)) {
                            throw new ArgumentException($"type [{model_.name}] has a field name is empty");
                        }

                        if(fields_.ContainsKey(fieldModel.name)) {
                            throw new ArgumentException($"type [{model_.name}]'s field [{fieldModel.name}] is already exists");
                        }
                        fields_.Add(fieldModel.name, fieldModel);
                    }
                }
            }

            private void Property() {
                if(model_.Propertys != null) {
                    foreach(var propertyModel in model_.Propertys) {
                        if(string.IsNullOrEmpty(propertyModel.name)) {
                            throw new ArgumentException($"type [{model_.name}] has a property name is empty");
                        }

                        if(fields_.ContainsKey(propertyModel.name)) {
                            throw new ArgumentException($"type [{model_.name}]'s property [{propertyModel.name}] is already exists");
                        }
                        propertys_.Add(propertyModel.name, propertyModel);
                    }
                }
            }

            private void Method() {
                if(model_.Methods != null) {
                    foreach(var methodModel in model_.Methods) {
                        if(string.IsNullOrEmpty(methodModel.name)) {
                            throw new ArgumentException($"type [{model_.name}] has a method name is empty");
                        }

                        var info = methods_.GetOrDefault(methodModel.name);
                        if(info == null) {
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

        private Dictionary<string, string> namespaceNameMaps_ = new Dictionary<string, string>();
        private Dictionary<string, string> typeNameMaps_ = new Dictionary<string, string>();
        private Dictionary<string, TypeMetaInfo> typeMetas_ = new Dictionary<string, TypeMetaInfo>();

        public XmlMetaProvider(IEnumerable<string> files) {
            foreach(string file in files) {
                XmlSerializer xmlSeliz = new XmlSerializer(typeof(XmlMetaModel));
                try {
                    using(Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                        XmlMetaModel model = (XmlMetaModel)xmlSeliz.Deserialize(stream);
                        if(model.Namespaces != null) {
                            foreach(var namespaceModel in model.Namespaces) {
                                LoadNamespace(namespaceModel);
                            }
                        }
                    }
                }
                catch(Exception e) {
                    throw new Exception($"load xml file wrong at {file}", e);
                }
            }
        }

        private static void FixName(ref string name) {
            name = name.Replace('^', '_');
        }

        private void LoadNamespace(XmlMetaModel.NamespaceModel model) {
            string namespaceName = model.name;
            if(string.IsNullOrEmpty(namespaceName)) {
                throw new ArgumentException("namespace's name is empty");
            }

            if(!string.IsNullOrEmpty(model.Name)) {
                if(namespaceNameMaps_.ContainsKey(namespaceName)) {
                    throw new ArgumentException($"namespace [{namespaceName}] is already has");
                }
                namespaceNameMaps_.Add(namespaceName, model.Name);
            }

            if(model.Classes != null) {
                LoadType(string.IsNullOrEmpty(model.Name) ? namespaceName : model.Name, model.Classes);
            }
        }

        private void LoadType(string namespaceName, XmlMetaModel.ClassModel[] classes) {
            foreach(var classModel in classes) {
                string className = classModel.name;
                if(string.IsNullOrEmpty(className)) {
                    throw new ArgumentException($"namespace [{namespaceName}] has a class's name is empty");
                }

                string classesfullName = namespaceName + '.' + className;
                FixName(ref classesfullName);
                if(!string.IsNullOrEmpty(classModel.Name)) {
                    if(typeNameMaps_.ContainsKey(classesfullName)) {
                        throw new ArgumentException($"class [{classesfullName}] is already has");
                    }
                    typeNameMaps_.Add(classesfullName, classModel.Name);
                }

                if(typeMetas_.ContainsKey(classesfullName)) {
                    throw new ArgumentException($"type [{classesfullName}] is already has");
                }
                TypeMetaInfo info = new TypeMetaInfo(classModel);
                typeMetas_.Add(classesfullName, info);
            }
        }

        public string GetNamespaceMapName(INamespaceSymbol symbol) {
            string name = symbol.ToString();
            if(name[0] == '<') {
                return symbol.Name;
            }
            else {
                return namespaceNameMaps_.GetOrDefault(name, name);
            }
        }

        private string GetTypeName(ISymbol symbol) {
            INamedTypeSymbol typeSymbol = (INamedTypeSymbol)symbol.OriginalDefinition;
            string namespaceName = GetNamespaceMapName(typeSymbol.ContainingNamespace);
            string name;
            if(typeSymbol.TypeArguments.Length == 0) {
                name = $"{namespaceName}.{symbol.Name}";
            }
            else {
                name = $"{namespaceName}.{symbol.Name}_{typeSymbol.TypeArguments.Length}";
            }
            return name;
        }

        private bool IsCodeTemplateEnable(ISymbol symbol) {
            return symbol.DeclaredAccessibility == Accessibility.Public && !symbol.IsFromCode();
        }

        public string GetTypeMapName(ISymbol symbol) {
            string name = GetTypeName(symbol);
            if(IsCodeTemplateEnable(symbol)) {
                return typeNameMaps_.GetOrDefault(name, name);
            }
            return name;        
        }

        private TypeMetaInfo GetTypeMetaInfo(ISymbol memberSymbol) {
            string typeName = GetTypeName(memberSymbol.ContainingType);
            return typeMetas_.GetOrDefault(typeName);
        }

        private string GetMethodName(IMethodSymbol symbol) {
            if(symbol.DeclaredAccessibility != Accessibility.Public) {
                return null;
            }

            string name = null;
            if(!symbol.IsFromCode()) {
                name = GetTypeMetaInfo(symbol)?.GetMethodMetaInfo(symbol.Name)?.GetName(symbol);
            }

            if(name == null) {
                if(symbol.IsOverride) {
                    if(symbol.OverriddenMethod != null) {
                        name = GetMethodName(symbol.OverriddenMethod);
                    }
                }
                else {
                    var interfaceImplementations = symbol.InterfaceImplementations();
                    if(interfaceImplementations != null) {
                        foreach(IMethodSymbol interfaceMethod in interfaceImplementations) {
                            name = GetMethodName(interfaceMethod);
                            if(name != null) {
                                break;
                            }
                        }
                    }
                }
            }
            return name;
        }

        public string GetMethodMapName(IMethodSymbol symbol) {
            string name = GetMethodName(symbol);
            return name ?? symbol.Name;
        }

        public bool IsPropertyField(IPropertySymbol symbol) {
            if(IsCodeTemplateEnable(symbol)) {
                var info = GetTypeMetaInfo(symbol)?.GetPropertyModel(symbol.Name);
                return info != null && info.IsAutoField;
            }
            return false;
        }

        public string GetFieldCodeTemplate(IFieldSymbol symbol) {
            if(IsCodeTemplateEnable(symbol)) {
                return GetTypeMetaInfo(symbol)?.GetFieldModel(symbol.Name)?.Template;
            }
            return null;
        }

        public string GetProertyCodeTemplate(IPropertySymbol symbol, bool isGet) {
            if(IsCodeTemplateEnable(symbol)) {
                var info = GetTypeMetaInfo(symbol)?.GetPropertyModel(symbol.Name);
                if(info != null) {
                    return isGet ? info.get?.Template : info.set?.Template;
                }
            }
            return null;
        }

        public string GetMethodCodeTemplate(IMethodSymbol symbol) {
            Contract.Assert(symbol != null);

            if(symbol.DeclaredAccessibility != Accessibility.Public) {
                return null;
            }

            string codeTemplate = null;
            if(!symbol.IsFromCode()) {
                codeTemplate = GetTypeMetaInfo(symbol)?.GetMethodMetaInfo(symbol.Name)?.GetCodeTemplate(symbol);
            }

            if(codeTemplate == null) {
                if(symbol.IsOverride) {
                    if(symbol.OverriddenMethod != null) {
                        codeTemplate = GetMethodCodeTemplate(symbol.OverriddenMethod);
                    }
                }
                else {
                    var interfaceImplementations = symbol.InterfaceImplementations();
                    if(interfaceImplementations != null) {
                        foreach(IMethodSymbol interfaceMethod in interfaceImplementations) {
                            codeTemplate = GetMethodCodeTemplate(interfaceMethod);
                            if(codeTemplate != null) {
                                break;
                            }
                        }
                    }
                }
            }
            return codeTemplate;
        }
    }
}
