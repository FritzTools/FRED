namespace FRED.Core.Parser {
    public class CommentToken : IToken {
        public string? Content { get; set; } = null;

        public CommentToken(string text) {
            Content = text;
        }
    }
}
