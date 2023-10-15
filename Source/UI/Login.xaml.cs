using FRED.Core.Data;
using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace FRED.UI {
    public partial class Login : Window {
        public Action<LoginData> OnLoginRequest { get; set; }

        public Login() {
            InitializeComponent();

            this.Closing += delegate(object? sender, System.ComponentModel.CancelEventArgs e) {
                e.Cancel = true;
                this.Hide();
            };
        }

        public new void Show() {
            base.Show();
            this.Release();
            this.Focus();
        }

        public void Release() {
            this.Submit.IsEnabled = true;
        }

        public void SetHostname(String? hostname, bool visible = true) {
            this.HostPanel.Visibility   = visible ? Visibility.Visible : Visibility.Hidden;
            this.Hostname.Text          = hostname != null ? new Uri(hostname).Host : "";
        }

        public void SetError(String? error) {
            if(error == null) {
                this.Error.Visibility = Visibility.Hidden;
                return;
            }

            this.Error.Visibility   = Visibility.Visible;
            this.Error.Text         = error;
        }

        public void SetTimer(int? seconds) {
            if (seconds == null) { 
                Submit.Content = "Anmelden";
                return;
            }

            int remain          = (int) seconds;
            Submit.Content      = "Anmelden (" + --remain + "s)";
            Submit.IsEnabled    = false;
            Timer timer         = new Timer()  {
                Interval = 1000
            };

            timer.Tick += new EventHandler(delegate (object? sender, EventArgs e) {
                Submit.Content      = "Anmelden (" + --remain + "s)";
                Submit.IsEnabled    = false;

                if(remain <= 0) {
                    Submit.IsEnabled    = true;
                    Submit.Content      = "Anmelden";
                    timer.Stop();
                    timer.Dispose();
                }
            });

            timer.Start();
        }

        public void OnPreview(LoginPreview data) {
            if(data.IsBlocked != null && (bool) data.IsBlocked) {
                this.SetTimer(data.BlockedTime);
            }

            this.Username.Items.Clear();

            foreach(String user in data.Users) {
                this.Username.Text = user; // @ToDo Last active!
                this.Username.Items.Add(user);
            }
        }

        public void OnLogin(object sender, MouseButtonEventArgs e) {
           this.Submit.IsEnabled = false;

           this.OnLoginRequest.Invoke(new LoginData() {
               Hostname = this.Hostname.Text,
               Username = this.Username.Text,
               Password = this.Password.Password
           });
        }
    }
}
