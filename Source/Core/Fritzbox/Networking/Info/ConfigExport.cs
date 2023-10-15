using FRED.Core.Data;
using FRED.Core.Fritzbox.Networking.HTTP;
using System;

namespace FRED.Core.Fritzbox.Networking.Info {
    public class ConfigExport {
        public static void Collect(AuthUser user, Action<String?> callback) {
            /*MultipartFormDataContent multipart = new MultipartFormDataContent("WebKitFormBoundaryCtAy6cbSuOvDuO01");
            multipart.Add(new StringContent("", Encoding.UTF8, "form-data"), "ConfigExport");
            multipart.Add(new StringContent(password, Encoding.UTF8, "form-data"), "ImportExportPassword");
            multipart.Add(new StringContent(user.SID, Encoding.UTF8, "form-data"), "sid");

            multipart.Headers.ContentType.MediaType = "multipart/form-data";


            System.Diagnostics.Debug.Print(Convert.ToString(multipart));*/

            String boundary = "CtAy6cbSuOvDuO01";
            String password = "test";
            String request  = "------WebKitFormBoundary" + boundary  + "\r\nContent-Disposition: form-data; name=\"sid\"\r\n\r\n" + user.SID + "\r\n------WebKitFormBoundary" + boundary + "\r\nContent-Disposition: form-data; name=\"ImportExportPassword\"\r\n\r\n" + password + "\r\n------WebKitFormBoundary" + boundary + "\r\nContent-Disposition: form-data; name=\"ConfigExport\"\r\n\r\n\r\n------WebKitFormBoundary" + boundary + "--";

            Request.Post("http://" + user.Endpoint + "/cgi-bin/firmwarecfg", request, delegate (String response) {
                callback(response);
            });
        }
    }
}
