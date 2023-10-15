using System;

namespace FRED.Core.Data {
    public class LoginData {
        public String? Hostname { get; set; } = null;
        public String? Username { get; set; } = null;
        public String? Password { get; set; } = null;

        public new String ToString() {
            return "[LoginData Hostname=" + Hostname + ", Username=" + Username + ", Password=" + Password + "]";
        }
    }
}
