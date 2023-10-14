using FRED.Core.Data;
using System.Windows;
using System.Windows.Controls;

namespace FRED.UI.Content {
    public partial class Info : UserControl {
        public Info() {
            InitializeComponent();

            this.Description.Content = "Generelle Informationen über der Export-Backupdatei.";
        }

        public void BindData(InfoData data) {
            // File
            Label label = new Label() {
                Name                = "Label_File",
                Content             = "File",
                Width               = 250,
                VerticalAlignment   = VerticalAlignment.Center
            };

            this.Panel.Children.Add(new DockPanel() {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Children            = {
                    label,
                    new TextBlock() {
                        Name            = "Text_File",
                        TextWrapping    = TextWrapping.Wrap,
                        Text            = data.File
                    }
                }
            });

            DockPanel.SetDock(label, Dock.Left);

            // Name
            label = new Label() {
                Name                = "Label_Name",
                Content             = "Name",
                Width               = 250,
                VerticalAlignment   = VerticalAlignment.Center
            };

            this.Panel.Children.Add(new DockPanel() {
                HorizontalAlignment     = HorizontalAlignment.Stretch,
                Children                = {
                    label,
                    new TextBlock() {
                        Name            = "Text_Name",
                        TextWrapping    = TextWrapping.Wrap,
                        Text            = data.Name
                    }
                }
            });

            DockPanel.SetDock(label, Dock.Left);

            // Checksum
            label = new Label() {
                Name                = "Label_Checksum",
                Content             = "Checksum",
                Width               = 250,
                VerticalAlignment   = VerticalAlignment.Center
            };

            this.Panel.Children.Add(new DockPanel() {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Children            = {
                    label,
                    new TextBlock() {
                        Name            = "Text_Checksum",
                        TextWrapping    = TextWrapping.Wrap,
                        Text            = data.Checksum
                    }
                }
            });

            DockPanel.SetDock(label, Dock.Left);

            // Files
            label = new Label() {
                Name                = "Label_Files",
                Content             = "Files",
                Width               = 250,
                VerticalAlignment   = VerticalAlignment.Center
            };

            this.Panel.Children.Add(new DockPanel() {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Children            = {
                    label,
                    new TextBlock() {
                        Name            = "Text_Files",
                        TextWrapping    = TextWrapping.Wrap,
                        Text            = data.Files + ""
                    }
                }
            });

            DockPanel.SetDock(label, Dock.Left);

            // TimeCreated
            label = new Label() {
                Name                = "Label_TimeCreated",
                Content             = "Time Created",
                Width               = 250,
                VerticalAlignment   = VerticalAlignment.Center
            };

            this.Panel.Children.Add(new DockPanel() {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Children            = {
                    label,
                    new TextBlock() {
                        Name            = "Text_TimeCreated",
                        TextWrapping    = TextWrapping.Wrap,
                        Text            = data.TimeCreated.ToString()
                    }
                }
            });

            DockPanel.SetDock(label, Dock.Left);

            // TimeChanged
            label = new Label() {
                Name                = "Label_TimeChanged",
                Content             = "Time Changed",
                Width               = 250,
                VerticalAlignment   = VerticalAlignment.Center
            };

            this.Panel.Children.Add(new DockPanel() {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Children            = {
                    label,
                    new TextBlock() {
                        Name            = "Text_TimeChanged",
                        TextWrapping    = TextWrapping.Wrap,
                        Text            = data.TimeChanged.ToString()
                    }
                }
            });

            DockPanel.SetDock(label, Dock.Left);
        }
    }
}
