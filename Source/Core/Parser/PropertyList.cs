using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FRED.Core.Parser {
    public class PropertyList {
        Dictionary<String, String> properties = new Dictionary<String, String>();

        public PropertyList(String input) {
            Regex content   = new Regex(@"(?<key>.*)?=(?<value>.*)?", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            foreach(Match match in content.Matches(input)) {
                properties.Add(match.Groups["key"].Value, match.Groups["value"].Value);
            }
        }

        public Dictionary<String, String> GetList() {
            return this.properties;
        }

        public String Get(String key) {
            return this.properties[key];
        }

        public void Set(String key, String value) {
            this.properties[key] = value;
        }

        public new String ToString {
            get {
                List<String> list = new List<String>();

                foreach (var pair in this.properties) {
                    list.Add(String.Format("[Property Name=\"{0}\", Value=\"{1}\"]", pair.Key, pair.Value));
                }

                return "{PropertyList " + string.Join(", ", list) + " }";
            }
        }
    }
}
