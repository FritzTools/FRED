using System;
using System.Collections.Generic;
using System.Text;

namespace FRED.Core.Data {
    public class AuthUser {
        public String? Endpoint { get; set; } = null;
        public String? Username { get; set; } = null;
        public String? SID { get; set; } = null;
        public Dictionary<String, AccessRights>? Rights { get; set; } = null;

        public new String ToString() {
            StringBuilder rights = new StringBuilder(); 

            foreach(KeyValuePair<String, AccessRights> right in Rights) {
                rights.Append(right.Key);

                switch(right.Value) {
                    case AccessRights.Read:
                        rights.Append(":R");
                    break;
                    case AccessRights.ReadWrite:
                        rights.Append(":RW");
                    break;
                }

                rights.Append(", ");
            }

            return "[AuthUser Endpoint=" + Endpoint + ", Username=" + Username + ", SID=" + SID + ", Rights=" + rights.ToString().TrimEnd(' ').TrimEnd(',') + "]";
        }
    }
}
