using FRED.Core.Data;
using FRED.Core.Events;
using FRED.Core.Fritzbox.Networking;
using FRED.Core.Interfaces;
using FRED.Core.Parser;
using FRED.UI.Content;
using FRED.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FRED.UI {
    public partial class Editor : Window, Changeable {
        private Consumable? consumer                = null;
        private List<ConsumingCallback> consuming   = new List<ConsumingCallback>();

        public Editor() {
            InitializeComponent();
        }

        public new void Hide() {
            base.Hide();
            this.consumer   = null;
            this.consuming  = new List<ConsumingCallback>();
        }

        public new void Show() {
            base.Show();

            this.AddConsumer("Name", (object? result) => {
                if(result == null) {
                    return;
                }

                BoxName.Text = (String)result;
            });

            OpenInfo();
            Focus();
        }

        private void LoadDeviceInfos() {
            /* START: @ToDo Currently for Testing here */
            JASON.Collect(delegate (JASONData? data) {
                System.Diagnostics.Debug.Print(data?.ToString());
            });

            JUIS.Collect(delegate (JUISData? data) {
                System.Diagnostics.Debug.Print(data?.ToString());
            });

            SystemStatus.Collect(delegate (SystemStatusData? data) {
                System.Diagnostics.Debug.Print(data?.ToString());
            });
            /* END: @ToDo Currently for Testing here */
        }

        private void AddConsumer(String name, Action<object?> callback) {
            if(consumer == null) {
                this.consuming.Add(new ConsumingCallback() {
                    Name        = name,
                    Callback    = callback
                });
                return;
            }

            if(!consumer.Get(name, callback)) {
                this.consuming.Add(new ConsumingCallback() {
                    Name        = name,
                    Callback    = callback
                });
            }
        }

        public void OnReady(Action callback) {
            callback();
        }

        public void OnChange(object? sender, EventArgs e) {
            if(e.GetType() == typeof(FileEvent)) {
                FileEvent file          = (FileEvent) e;

                if(file == null) {
                    return;
                }

                FileContainer? container = file.File;
                NavigationItem navi     = new NavigationItem();
                String icon             = "Unknown";

                container?.UpdateSubType();

                switch(container?.GetType()) {
                    case "CFG":
                        icon = "Properties";
                    break;
                    case "B64":
                        if(container.HasSubType()) {
                            switch(container.GetSubType()) {
                                case "CFG":
                                    icon = "Properties";
                                break;
                                case "XML":
                                case "Binary":
                                    icon = container.GetSubType();
                                break;
                            }
                        }
                    break;
                    case "BIN":
                        icon = "Binary";
                    break;
                }

                navi.ArrowVisibility    = Visibility.Hidden;
                navi.PreviewMouseDown   += OnClick;
                navi.Header             = new StackPanel() {
                    Orientation = Orientation.Horizontal,
                    Children    = {
                        new Image() {
                            Width   = 16,
                            Source  = (System.Windows.Media.ImageSource) TryFindResource("Icon" + icon)
                        },
                        new TextBlock() {
                            Text = container?.GetName()
                        }
                    }
                };

                if(container != null && container.IsCrypted()) {
                    ((StackPanel) navi.Header).Children.Add(new Image() {
                        Width   = 16,
                        Source  = (System.Windows.Media.ImageSource) TryFindResource("IconCrypted")
                    });
                }

                Files.Children.Add(navi);
            }

            this.UpdateScrollbar();
        }

        protected void UpdateScrollbar() {
            this.Focus();
            this.Scroll.InvalidateScrollInfo();
        }

        public void SetConsumer(Consumable consumer) {
            this.consumer = consumer;

            this.consumer.OnReady(delegate() {
                foreach(ConsumingCallback callback in this.consuming) {
                    if(callback != null && callback.Name != null && callback.Callback != null) { 
                        consumer.Get(callback.Name, callback.Callback);
                    }
                }
            });
        }

        private void OpenInfo() {
            Info info = new Info();

            this.AddConsumer("Info", (object? result) => {
                if (result == null) {
                    return;
                }

                info.BindData((InfoData)result);
            });

            Panel.Content = info;
        }
        private void OnScroll(object sender, MouseWheelEventArgs e) {
            this.Scroll.ScrollToVerticalOffset(this.Scroll.VerticalOffset);
            this.UpdateScrollbar();
        }

        private void OnClick(object sender, MouseButtonEventArgs e) {
            String? name        = null;
            NavigationItem item = (NavigationItem) sender;

            if(item.Header is StackPanel) {
                StackPanel panel    = (StackPanel) item.Header;
                TextBlock? text     = panel.Children.OfType<TextBlock>().FirstOrDefault();

                if(text != null) {
                    name = text.Text;
                }
            } else {
                name = item.Header.ToString();
            }

            switch (name) {
                case "Files":
                    this.UpdateScrollbar();
                break;
                case "Info":
                    this.OpenInfo();
                break;
                case "Properties":
                    Properties properties = new Properties();

                    this.AddConsumer("Properties", (object? result) => {
                        if(result == null) {
                            return;
                        }
                        properties.BindData((PropertyList) result);
                    });

                    Panel.Content = properties;
                break;
                default:
                    if(name == null) {
                        return;
                    }

                    File file = new File(name);

                    this.AddConsumer(name, (object? result) => {
                        if (result == null) {
                            return;
                        }

                        file?.Update((FileContainer) result);
                    });

                    Panel.Content = file;
                break;
            }
        }
    }
}
