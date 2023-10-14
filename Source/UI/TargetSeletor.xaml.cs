using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FRED.Core.Interfaces;
using Rssdp;

namespace FRED.UI {
    public partial class TargetSeletor : Window, Changeable {
        private Action<String>? callback            = null;
        System.Windows.Forms.OpenFileDialog dialog  = new System.Windows.Forms.OpenFileDialog() {
            Filter              = "Export files (*.export)|*.export",
            InitialDirectory    = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            RestoreDirectory    = true
        };

        public TargetSeletor() {
            InitializeComponent();

            this.dialog.FileOk += delegate (object? sender, CancelEventArgs e) {
                System.Diagnostics.Debug.Print(dialog.FileName);

                if (this.callback != null) {
                    this.callback(dialog.FileName);
                }

                this.Hide();
            };
        }

        public async void OnChange(object? sender, EventArgs e) {
            DeviceAvailableEventArgs b  = (DeviceAvailableEventArgs) ((object) e);
            SsdpDevice? device          = (SsdpDevice?) sender;
            SsdpDevice fullDevice       = await b.DiscoveredDevice.GetDeviceInfo();

            //System.Diagnostics.Debug.Print("FriendlyName: " + fullDevice.FriendlyName);
            //System.Diagnostics.Debug.Print("SerialNumber: " + fullDevice.SerialNumber);
            //System.Diagnostics.Debug.Print("Found " + b.DiscoveredDevice.Usn + " at " + b.DiscoveredDevice.DescriptionLocation.ToString() +  ": " + device.PresentationUrl.ToString());

            Dispatcher.BeginInvoke(new Action(() => {
                Grid entry = new Grid() {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Name            = "DeviceEntry",
                    RowDefinitions  = {
                        new RowDefinition() {
                            Height = new GridLength(20,GridUnitType.Pixel)
                        },
                        new RowDefinition() {
                            Height = new GridLength(0,GridUnitType.Auto)
                        }
                    },
                    ColumnDefinitions   = {
                        new ColumnDefinition() {
                            Width = new GridLength(50,GridUnitType.Pixel)
                        },
                        new ColumnDefinition() {
                            Width = new GridLength(0,GridUnitType.Auto)
                        }
                    }
                };

                /* Hostname */
                TextBlock hostname = new TextBlock() {
                    Text        = device?.PresentationUrl.ToString(),
                    FontSize    = 12,
                    Foreground  = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#444444"))
                };
                entry.Children.Add(hostname);
                Grid.SetRow(hostname, 0);
                Grid.SetColumn(hostname, 1);

                /* Name */
                TextBlock name = new TextBlock() {
                    Text        = device?.FriendlyName,
                    FontSize    = 16,
                    Foreground  = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FFFFFF"))
                };
                entry.Children.Add(name);
                Grid.SetRow(name, 1);
                Grid.SetColumn(name, 1);

                /* Icon */
                Image icon = new Image() {
                    Width               = 40,
                    VerticalAlignment   = VerticalAlignment.Center,
                    Source              = (ImageSource) this.TryFindResource("IconFritzBox")
                };
                entry.Children.Add(icon);
                Grid.SetRow(icon, 0);
                Grid.SetColumn(icon, 0);
                Grid.SetRowSpan(icon, 2);

                Button button = new Button() {
                    Name                        = "Device",
                    HorizontalContentAlignment  = HorizontalAlignment.Stretch,
                    Padding                     = new Thickness(0),
                    BorderThickness             = new Thickness(0),
                    Style                       = (Style) this.TryFindResource("DeviceBox"),
                    Content                     = entry
                };

                button.PreviewMouseLeftButtonDown += delegate(object sender, MouseButtonEventArgs e) {
                    this.OnDeviceSelect(device?.PresentationUrl.ToString());
                };

                this.Devices.Children.Add(button);
            })).Wait();
        }

        private void OnDeviceSelect(String? hostname) {
            if(hostname == null) {
                // @ToDo Error window: Unknown input
                return;
            }

            if(this.callback == null) {
                return;
            }

            this.callback(hostname);
        }

        public void Show(Action<String> callback) {
            base.Show();
            this.Focus();
            this.callback = callback;
        }

        public void OnReady(Action callback) {}

        private void OpenFile(object sender, RoutedEventArgs e) {
            dialog.ShowDialog();
        }
    }
}
