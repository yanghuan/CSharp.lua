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

        private static Dictionary<string, string> namespaceNameMaps_ = new Dictionary<string, string>();
        private static Dictionary<string, string> typeNameMaps_ = new Dictionary<string, string>();

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
                    throw new ArgumentException($"namespace[{namespaceName}] has a class's name is empty");
                }

                string classesfullName = namespaceName + '.' + className;
                FixName(ref classesfullName);
                if(!string.IsNullOrEmpty(classModel.Name)) {
                    if(typeNameMaps_.ContainsKey(classesfullName)) {
                        throw new ArgumentException($"class [{classesfullName}] is already has");
                    }
                    typeNameMaps_.Add(classesfullName, classModel.Name);
                }
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

        public string GetTypeMapName(ISymbol symbol) {
            INamedTypeSymbol typeSymbol = (INamedTypeSymbol)symbol.OriginalDefinition;
            string namespaceName = GetNamespaceMapName(typeSymbol.ContainingNamespace);
            string name;
            if(typeSymbol.TypeArguments.Length == 0) {
                name = $"{namespaceName}.{symbol.Name}";
            } 
            else {
                name = $"{namespaceName}.{symbol.Name}_{typeSymbol.TypeArguments.Length}";
            }
            return typeNameMaps_.GetOrDefault(name, name);
        }
    }
}
