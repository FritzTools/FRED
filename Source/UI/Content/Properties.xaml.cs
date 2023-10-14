using FRED.Core.Parser;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace FRED.UI.Content {
    public partial class Properties : UserControl {
        public Properties() {
            InitializeComponent();

            this.Description.Content = "Diese Werte sind die Export-Parameter, die am Anfang der Datei stehen.";
        }

        public void BindData(PropertyList properties) {
            foreach(KeyValuePair<string, string> entry in properties.GetList()) {
                Label label = new Label() {
                    Name                = "Label_" + entry.Key,
                    Content             = entry.Key,
                    Width               = 250,
                    VerticalAlignment   = VerticalAlignment.Center
                };

                this.Panel.Children.Add(new DockPanel() {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Children = {
                        label,
                        new TextBox() {
                            Name = "Input_" + entry.Key,
                            Text = entry.Value,
                            HorizontalAlignment = HorizontalAlignment.Stretch
                        }
                    }
                });

                DockPanel.SetDock(label, Dock.Left);
            }
        }
    }
}
