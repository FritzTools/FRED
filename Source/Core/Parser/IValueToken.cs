namespace FRED.Core.Parser {
    public interface IValueToken : IToken {
        string Key { get; }
        string? Value { get; set; }
        string? Comment { get; set; }
    }
}
