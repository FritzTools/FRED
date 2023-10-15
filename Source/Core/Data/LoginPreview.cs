using System;
using System.Collections.Generic;

namespace FRED.Core.Data {
    public class LoginPreview {
        public String? SID { get; set; } = null;
        public int? BlockedTime { get; set; } = null;
        public bool? IsBlocked { get; set; } = null;
        public bool? IsLoggedIn { get; set; } = null;
        public List<String> Users { get; set; } = null;

        public new String ToString() {
            return "[LoginPreview SID=" + SID + ", IsLoggedIn=" + IsLoggedIn  + " IsBlocked=" + IsBlocked  + ", BlockedTime=" + BlockedTime  + ", Users=" + String.Join(", ", Users) + "]";
        }
    }
}
