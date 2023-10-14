using FRED.Core.Data;
using FRED.UI.Controls;
using System;
using System.Windows.Controls;

namespace FRED.UI.Content {
    public partial class File : UserControl {
        public File(String? name) {
            InitializeComponent();
        }

        public void Update(FileContainer file) {
            this.Filename.Content       = file.GetName();
            this.Description.Content    = file.GetDescription();

            if(this.Source == null) {
                return;
            }

            switch(file.GetSubType()) {
                case "Binary":
                    this.Source.CreateEditor(typeof(HEXEditor));
                    this.Source.SetHighlighter(Core.Interfaces.EditorHighlighter.Binary);
                break;
                case "XML":
                    this.Source.CreateEditor(typeof(TextEditor));
                    this.Source.SetHighlighter(Core.Interfaces.EditorHighlighter.XML);
                break;
                default:
                    this.Source.CreateEditor(typeof(TextEditor));
                    this.Source.SetHighlighter(Core.Interfaces.EditorHighlighter.Default);
                break;
            }

            this.Source.SetContent(file.GetSource());
        }
    }
}
