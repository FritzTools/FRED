using System;

namespace FRED.Core.Interfaces {
    public enum EditorHighlighter {
        Default,
        XML,
        Binary
    }

    public interface EditorInstance {
        public void SetContent(String? source);
        public void SetHighlighter(EditorHighlighter? type);
    }
}
