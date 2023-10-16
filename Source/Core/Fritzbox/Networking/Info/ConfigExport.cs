using FRED.Core.Data;
using FRED.Core.Fritzbox.Networking.HTTP;
using System;
using System.Net.Http;

namespace FRED.Core.Fritzbox.Networking.Info {
    public class ConfigExport {
        public static void Collect(AuthUser user, Action<String?> callback) {
            String boundary = "CtAy6cbSuOvDuO01";
            String password = "test";

            Request.Post("http://" + user.Endpoint + "/cgi-bin/firmwarecfg", new MultipartFormDataContent("----WebKitFormBoundary" + boundary) {
                new FormDataContent("sid", user.SID),
                new FormDataContent("ImportExportPassword", password),
                new FormDataContent("ConfigExport", "")
            }, delegate (String response) {
                callback(response);
            });            
        }
    }
}
