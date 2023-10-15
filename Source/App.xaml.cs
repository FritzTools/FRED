using FRED.Core.Data;
using FRED.Core.Fritzbox.Networking.Discover;
using FRED.Core.Fritzbox.Networking.Info;
using FRED.Core.Parser;
using FRED.UI;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;

namespace FRED {
    public partial class App : Application {

        public App() {
            TargetSeletor selector  = new TargetSeletor();
            Editor editor           = new Editor();

            selector.Closing += delegate(object? sender, System.ComponentModel.CancelEventArgs e) {
                Environment.Exit(0);
            };

            editor.Closing += delegate(object? sender, System.ComponentModel.CancelEventArgs e) {
                e.Cancel = true;
                editor.Hide();

                if(!(bool) typeof(Window).GetProperty("IsDisposed", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(selector)) {
                    selector.Show();
                }
            };

            // Start Discovering-Service to find the Fritz!Box
            Discovery.AddObserver(selector);
            Discovery.SearchForDevices();

            selector.Show(delegate(String file) {
                if(!new Uri(file).IsFile) {
                    selector.OpenLogin(file, delegate(AuthUser user) {
                        System.Diagnostics.Debug.Print("[OpenLogin] Selector: " + user.ToString());
                        // @ToDo Bind to Editor, and request Export-File!

                        Loading loading = new Loading();
                        loading.Show();

                        ConfigExport.Collect(user, delegate(String? data) {
                            try {
                                String directory    = Path.Combine("./", "Workspace");
                                String file         = Path.Combine(directory, "Config_" + DateTimeOffset.Now.ToUnixTimeMilliseconds() + ".export");
                                
                                if (!Directory.Exists(directory)) {
                                    Directory.CreateDirectory(directory);
                                }

                                using (FileStream writer = File.Create(file)) {
                                    byte[] bytes = new UTF8Encoding(true).GetBytes(data);
                                    writer.Write(bytes, 0, bytes.Length);
                                    writer.Close();
                                }

                                ExportFile parser = new ExportFile(file);
                                editor.SetConsumer(parser);
                                parser.SetObserver(editor);

                                editor.Show();
                                parser.Parse();
                                loading.Hide();
                                selector.Hide();
                            } catch(Exception e) {
                                System.Diagnostics.Debug.Print(e.Message);
                            }
                        });
                    });
                    return;
                }

                ExportFile parser   = new ExportFile(file);
                editor.SetConsumer(parser);
                parser.SetObserver(editor);

                editor.Show();
                parser.Parse();
            });
        }
    }
}
