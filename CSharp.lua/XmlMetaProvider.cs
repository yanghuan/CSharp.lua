using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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

        public XmlMetaProvider(IEnumerable<string> files) {
            foreach(string file in files) {
                XmlSerializer xmlSeliz = new XmlSerializer(typeof(XmlMetaModel));
                try {
                    using(Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                        XmlMetaModel model = (XmlMetaModel)xmlSeliz.Deserialize(stream);
                        if(model.Namespaces != null) {
                            foreach(var namespaceModel in model.Namespaces) {
                               
                            }
                        }
                    }
                }
                catch(Exception e) {
                    throw new Exception($"load xml file wrong at {file}", e);
                }
            }
        }
    }
}
