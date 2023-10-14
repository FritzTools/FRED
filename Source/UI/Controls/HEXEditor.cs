using FRED.Core.Interfaces;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WpfHexaEditor;

namespace FRED.UI.Controls {
    public class HEXEditor : UserControl, EditorInstance {
        HexEditor editor = new HexEditor() {
            BorderThickness = new Thickness(0, 0, 0, 0),
            AllowContextMenu = false
        };

        public HEXEditor() {
            this.Content = editor;
        }

        public void SetContent(String? source) {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);

            if(source == null) {
                writer.Write(source);
                writer.Flush();
            }

            stream.Position     = 0;
            this.editor.Stream  = stream;
        }

        public void SetHighlighter(EditorHighlighter? type) {
            switch(type) {
                case EditorHighlighter.Binary:
                    /* Do Nothing */
                break;
            }
        }
    }
}
