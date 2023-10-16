namespace FRED.Core.Parser {
    public class ValueToken : IValueToken {
        public ValueToken(GroupToken parent, string key, string? value = null, string? comment = null) {
            Key     = key;
            Value   = value;
            Comment = comment;
            Parent  = parent;
        }

        public GroupToken Parent { get; }

        public string Key { get; }
        public string? Value { get; set; } = null;
        public string? Comment { get; set; } = null;
    }
}
