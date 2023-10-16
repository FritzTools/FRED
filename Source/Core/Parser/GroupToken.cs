using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;

namespace FRED.Core.Parser {
    public class GroupToken : IValueToken {
        public GroupToken(GroupToken parent, string key, string? value = null, string? comment = null) {
            Key     = key;
            Value   = value?.Trim();
            Comment = comment;
            Tokens  = new List<IToken>();
            Parent  = parent;
        }

        public string Key { get; }
        public string? Value { get; set; }
        public string? Comment { get; set; }

        public IList<IToken> Tokens { get; }

        public GroupToken Parent { get; }

        public void Add(IToken token) {
            Tokens.Add(token);
        }

        public void Add(string key, string? value = null, string? comment = null) {
            Add(new ValueToken(this, key, value, comment));
        }

        public new String ToString() {
            return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings {
                ReferenceLoopHandling   = ReferenceLoopHandling.Ignore,
                NullValueHandling       = NullValueHandling.Ignore
            });
        }
    }
}
