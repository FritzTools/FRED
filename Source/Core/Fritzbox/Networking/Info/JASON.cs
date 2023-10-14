using FRED.Core.Fritzbox.Networking.Discover;
using FRED.Core.Fritzbox.Networking.HTTP;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace FRED.Core.Fritzbox.Networking {
    public class JASONData {
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
                if (value == null) {
                    return;
                }

                _Data[key] = value;
            }
        }

        public JASONData() {
            this["Flag"] = new List<String>();
        }

        public new String ToString() {
            StringBuilder output = new StringBuilder();
            output.Append("[JASON");

            if (_Data != null) {
                foreach (KeyValuePair<String, object?> entry in _Data) {
                    output.Append(" ");
                    output.Append(entry.Key);
                    output.Append("=");

                    if(entry.Value == null) {
                        output.Append("");
                    } else if (entry.Value.GetType() == typeof(List<String>)) {
                        output.Append(String.Join(";", (List<String>)entry.Value));
                    } else {
                        output.Append(entry.Value.ToString());
                    }
                }
            }

            output.Append("]");

            return output.ToString();
        }
    }

    public class JASON {
        public static void Collect(Action<JASONData?> callback) {
            Discovery.WhenReady(delegate (String url) {
                Request.Get(url + "jason_boxinfo.xml", delegate (String response) {
                    JASONData data      = new JASONData();
                    XmlDocument doc     = new XmlDocument();

                    if(response == null) {
                        callback(null);
                        return;
                    }

                    doc.LoadXml(response);

                    if(doc != null && doc.DocumentElement != null) {
                        foreach(XmlNode node in doc.DocumentElement.ChildNodes) {
                            String name = node.Name.Replace("j:", "");

                            switch (name) {
                                case "Flag":
                                    ((List<String>?)data["Flag"])?.Add(node.InnerText);
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
