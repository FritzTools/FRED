using System;
using FRED.Core.Fritzbox.Networking.HTTP;
using FRED.Core.Fritzbox.Networking.Discover;
using System.Xml;
using System.Collections.Generic;
using System.Text;

namespace FRED.Core.Fritzbox.Networking {
    public class JUISData {
        private static Dictionary<String, object?> _Data = new Dictionary<String, object?>();

        public object? this[String key] {
            get {
                if(_Data.ContainsKey(key)) {
                    return _Data[key];
                } else {
                    return null;
                }
            }

            set {
                if(value == null) {
                    return;
                }

                _Data[key] = value;
            }
        }

        public JUISData() {
            this["Flag"] = new List<String>();
        }

        public new String ToString() {
            StringBuilder output = new StringBuilder();
            output.Append("[JUIS");

            foreach(KeyValuePair<String, object?> entry in _Data) {
                output.Append(" ");
                output.Append(entry.Key);
                output.Append("=");

                if(entry.Value == null) {
                    output.Append("");
                } else if(entry.Value.GetType() == typeof(List<String>)) {
                    output.Append(String.Join(";", (List<String>) entry.Value));
                } else {
                    output.Append(entry.Value.ToString());
                }
            }

            output.Append("]");

            return output.ToString();
        }
    }

    public class JUIS {
        public static void Collect(Action<JUISData?> callback) {
            Discovery.WhenReady(delegate (String url) {
                Request.Get(url + "juis_boxinfo.xml", delegate (String response) {
                    JUISData data       = new JUISData();
                    XmlDocument doc     = new XmlDocument();

                    if(response == null) {
                        callback(null);
                        return;
                    }

                    doc.LoadXml(response);

                    if(doc != null && doc.DocumentElement != null) {
                        foreach(XmlNode node in doc.DocumentElement.ChildNodes) {
                            if(node == null) {
                                return;
                            }

                            String name = node.Name.Replace("q:", "");

                            switch(name) {
                                case "Flag":
                                    ((List<String>?) data["Flag"])?.Add(node.InnerText);
                                break;
                                default:
                                    data[name] = node.InnerText;
                                break;
                            }
                        }
                    }

                    callback(data);
                }).Wait();
            });
        }
    }
}
