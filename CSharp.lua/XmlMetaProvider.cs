using System;
using System.Collections.Generic;
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

            public string GetName(IMethodSymbol symbol) {
                if(models_.Count == 1) {
                    return models_.First().Name;
                }
                throw new NotSupportedException();
            }
        }

        private sealed class TypeMetaInfo {
            private XmlMetaModel.ClassModel model_;
            private Dictionary<string, MethodMetaInfo> methods_ = new Dictionary<string, MethodMetaInfo>();

            public TypeMetaInfo(XmlMetaModel.ClassModel model) {
                model_ = model;
                Method();
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

            public MethodMetaInfo GetMethodMetaInfo(string name) {
                return methods_.GetOrDefault(name);
            }
        }

        private static Dictionary<string, string> namespaceNameMaps_ = new Dictionary<string, string>();
        private static Dictionary<string, string> typeNameMaps_ = new Dictionary<string, string>();
        private static Dictionary<string, TypeMetaInfo> typeMetas_ = new Dictionary<string, TypeMetaInfo>();

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
            string name = symbol.ContainingNamespace.ToString();
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

        public string GetTypeMapName(ISymbol symbol) {
            string name = GetTypeName(symbol);
            return typeNameMaps_.GetOrDefault(name, name);
        }

        internal string GetMethodMapName(IMethodSymbol symbol) {
            string typeName = GetTypeName(symbol.ContainingType);
            var typeInfo = typeMetas_.GetOrDefault(typeName);
            if(typeInfo != null) {
                var methodInfo = typeInfo.GetMethodMetaInfo(symbol.Name);
                if(methodInfo != null) {
                    string name = methodInfo.GetName(symbol);
                    if(name != null) {
                        return name;
                    }
                }
            }
            return symbol.Name;
        }
    }
}
