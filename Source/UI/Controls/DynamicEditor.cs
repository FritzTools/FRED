using FRED.Core.Interfaces;
using System;
using System.Windows.Controls;

namespace FRED.UI.Controls {
    public class DynamicEditor : UserControl {
        EditorInstance? instance = null;

        public DynamicEditor() {}

        public void SetContent(String? source) {
            instance?.SetContent(source);
        }

        public void SetHighlighter(EditorHighlighter? type) {
            instance?.SetHighlighter(type);
        }
        public void CreateEditor(Type? type) {
            if(type == typeof(HEXEditor)) {
                this.instance = new HEXEditor();
            } else if (type == typeof(TextEditor)) {
                this.instance = new TextEditor();
            }

            this.Content = instance;
        }
    }
}
