using FRED.Core.Fritzbox.Networking.Discover;
using FRED.Core.Fritzbox.Networking.HTTP;
using System;
using System.Text.RegularExpressions;

namespace FRED.Core.Fritzbox.Networking {
    public class SystemStatusData {
        public String? Model { get; set; } = null;
        public String? Annex { get; set; } = null;
        public String? Running { get; set; } = null;
        public String? Reboots { get; set; } = null;
        public String? Hash { get; set; } = null;
        public String? State { get; set; } = null;
        public String? Version { get; set; } = null;
        public String? SubVersion { get; set; } = null;
        public String? Branding { get; set; } = null;

        public new String ToString() {
            return "[SystemStatus Model=" + Model + ", Annex=" + Annex + ", Running=" + Running + ", Reboots=" + Reboots + ", Hash=" + Hash + ", State=" + State + ", FirmwareVersion=" + Version + ", Subversion=" + SubVersion + ", Branding=" + Branding + "]";
        }
    }

    public class SystemStatus {
        public static void Collect(Action<SystemStatusData?> callback) {
            Discovery.WhenReady(delegate (String url) {
                Request.Get(url+ "cgi-bin/system_status", delegate (String response) {
                    if(response == null) {
                        callback(null);
                        return;
                    }

                    String data     = Regex.Replace(response, "(<.*?>|\r|\n)", String.Empty);
                    String[] parts  = data.Split("-");

                    callback(new SystemStatusData() {
                        Model       = parts[0],
                        Annex       = parts[1],
                        Running     = parts[2],
                        Reboots     = parts[3],
                        Hash        = parts[4] + "-" + parts[5],
                        State       = parts[6],
                        Version     = parts[7],
                        SubVersion  = parts[8],
                        Branding    = parts[9]
                    });
                }).Wait();
            });
        }
    }
}
