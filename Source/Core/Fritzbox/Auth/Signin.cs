using FRED.Core.Data;
using FRED.Core.Fritzbox.Networking.HTTP;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace FRED.Core.Fritzbox.Auth {
    public class Signin {
        public Action<AuthUser?>? Success { get; set; } = null;
        public Action<String?>? Error { get; set; } = null;
        public Action<int?>? WaitTimer { get; set; } = null;

        /*
         * Auth-Documentation: https://avm.de/fileadmin/user_upload/Global/Service/Schnittstellen/AVM_Technical_Note_-_Session_ID_deutsch_2021-05-03.pdf
         */
        private void CreateChallenge(String challenge, LoginData data) {
            // PBKDF2 (ab FRITZ!OS 7.24)
            if(challenge.Contains("$")) {
                String[] parts  = challenge.Split('$');
                String type     = parts[0];
                
                switch(type) {
                    // Version 2
                    case "2":
                        int iter1 = int.Parse(parts[1]);                  // 600000
                        byte[] salt1 = Convert.FromHexString(parts[2]);     // HEX
                        int iter2 = int.Parse(parts[3]);                  // 600000
                        byte[] salt2 = Convert.FromHexString(parts[4]);     // HEX

                        // Calculate
                        byte[] hash1 = HMAC(Encoding.UTF8.GetBytes(data.Password), salt1, iter1);
                        byte[] hash2 = HMAC(hash1, salt2, iter2);

                        this.CallSignin(parts[4] + "$" + ConvertBytes(hash2), data);
                    break;

                    default:
                        /* Not Supported! */
                    break;
                }
                
            // MD5-Fallback
            } else {
                this.CallSignin(challenge + "-" + MD5(challenge + "-" + data.Password), data);
            }
        }

        private static byte[] HMAC(byte[] input, byte[] salt, int iterations) {
            using(Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(input, salt, iterations, HashAlgorithmName.SHA256)) {
                 return pbkdf2.GetBytes(32);
            }
        }

        private String MD5(String input) {
            return ConvertBytes(System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input)));
        }

        private String ConvertBytes(byte[] bytes) {
            StringBuilder output = new StringBuilder();

            for(int index = 0; index < bytes.Length; index++) {
                output.Append(bytes[index].ToString("x2"));
            }

            return output.ToString().ToLower();
        }

        private async void CallSignin(String hash, LoginData data) {
            await Request.Post("http://" + data.Hostname + "/login_sid.lua?version=2&user=" + data.Username + "&response=" + hash, new[] {
                new KeyValuePair<String, String>("response", hash),
                new KeyValuePair<String, String>("username", data.Username)
            }, delegate(String response) {
                this.HandleResponse(response, data.Username, delegate(String sid) {
                    Success?.Invoke(new AuthUser() {
                        Username    = data.Username,
                        SID         = sid
                    });
                });
            });
        }

        public async void Call(LoginData data) {
            await Request.Get("http://" + data.Hostname + "/login_sid.lua?version=2", delegate (String response) {
                this.HandleResponse(response, null, delegate(String challenge) {
                    this.CreateChallenge(challenge, data);
                });
            });
        }
        
        public async void CallPreview(LoginData data, Action<LoginPreview> callback) {
            if(data.Hostname == null) {
                return;
            }

            await Request.Get("http://" + data.Hostname + "/login_sid.lua?version=2&user=" + (data.Username == null ? "" : data.Username), delegate (String response) {
                this.HandleResponse(response, data.Username, delegate(String challenge) {
                    try {
                        XmlDocument doc     = new XmlDocument();
                        doc.LoadXml(response);
                        XmlNode? session    = doc.SelectSingleNode("SessionInfo");

                        if(session == null) {
                            return;
                        }

                        XmlNode? SID        = session.SelectSingleNode("SID");
                        XmlNode? block      = session.SelectSingleNode("BlockTime");
                        List<String> users  = new List<String>();

                        if(SID == null) {
                            return;
                        }

                        if(block == null) {
                            return;
                        }

                        if(session.SelectSingleNode("Users") != null) {
                            foreach(XmlNode user in session.SelectSingleNode("Users").ChildNodes) {
                                users.Add(user.InnerText);
                            }
                        }

                        int blocked = int.Parse(block.InnerText);

                        if (blocked >= 1) {
                            WaitTimer?.Invoke(blocked);
                        }

                        callback.Invoke(new LoginPreview() {
                            SID         = SID.InnerText,
                            BlockedTime = blocked,
                            IsBlocked   = blocked >= 1,
                            IsLoggedIn  = !SID.InnerText.Equals("0000000000000000"),
                            Users       = users
                        });
                    }
                    catch (Exception) { }
                });
            });
        }

        private void HandleResponse(String response, String? username, Action<String> callback) {
            if(response == null || response.Trim().Equals("")) {
                Error?.Invoke("Error 0x001: Login derzeit leider nicht möglich.");
                return;
            }

            try {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response);
                XmlNode? session = doc.SelectSingleNode("SessionInfo");

                if(session == null) {
                    Error?.Invoke("Error 0x004: Login derzeit leider nicht möglich.");
                    return;
                }

                XmlNode? SID        = session.SelectSingleNode("SID");
                XmlNode? block      = session.SelectSingleNode("BlockTime");
                XmlNode? challenge  = session.SelectSingleNode("Challenge");

                if(SID == null) {
                    Error?.Invoke("Error 0x007: Login derzeit leider nicht möglich.");
                    return;
                }

                if(block == null) {
                    Error?.Invoke("Error 0x005: Login derzeit leider nicht möglich.");
                    return;
                }

                if(challenge == null) {
                    Error?.Invoke("Error 0x006: Login derzeit leider nicht möglich.");
                    return;
                }

                int blocked = int.Parse(block.InnerText);

                if(blocked >= 1) {
                    Error?.Invoke("Error 0x008: Falsche Zugangsdaten!");
                    WaitTimer?.Invoke(blocked);
                    return;
                }

                Dictionary<String, AccessRights> rights = new Dictionary<String, AccessRights>();
                if (session.SelectSingleNode("Rights") != null) {
                    XmlNodeList container = session.SelectSingleNode("Rights").ChildNodes;

                    for (int index = 0; index < container.Count;) {
                        XmlNode name        = container[index++];
                        XmlNode value       = container[index++];
                        AccessRights right  = AccessRights.None;

                        switch (int.Parse(value.InnerText)) {
                            case 1:
                                right = AccessRights.Read;
                            break;
                            case 2:
                                right = AccessRights.ReadWrite;
                            break;
                        }

                        rights.Add(name.InnerText, right);
                    }
                }

                if (!SID.InnerText.Equals("0000000000000000")) {;
                    
                    Success?.Invoke(new AuthUser() {
                        Username    = username,
                        SID         = SID.InnerText,
                        Rights      = rights
                    });
                } else {
                    callback(challenge.InnerText);
                }
            } catch (Exception e) {
                Error?.Invoke("Error 0x002: Login derzeit leider nicht möglich: " + e.Message);
                return;
            }
        }
    }
}
