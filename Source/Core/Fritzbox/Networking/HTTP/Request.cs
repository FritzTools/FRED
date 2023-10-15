using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FRED.Core.Fritzbox.Networking.HTTP {
    class Request {
        private static readonly HttpClient Client = new HttpClient();

        private static async Task<String> GetAsync(String url) {
            try {
                HttpResponseMessage response = await Client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            } catch(Exception) {
                return null;
            }
        }

        private static async Task<String> PostAsync(String url, KeyValuePair<String, String>[] content) {
            return await PostAsync(url, new FormUrlEncodedContent(content));
        }

        private static async Task<String> PostAsync(String url, String content) {
            return await PostAsync(url, new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded"));
        }

        private static async Task<String> PostAsync(String url, HttpContent content) {
            HttpResponseMessage response    = await Client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            //System.Diagnostics.Debug.Print(Convert.ToString(response.RequestMessage));

            return await response.Content.ReadAsStringAsync();
        }

        public static async Task Get(String url, Action<String> callback) {
            String getResponse = await GetAsync(url);
            callback(getResponse);
        }

        public static async Task Post(String url, KeyValuePair<String, String>[] data, Action<String> callback) {
            String getResponse = await PostAsync(url, data);
            callback(getResponse);
        }

        public static async Task Post(String url, String data, Action<String> callback) {
            String getResponse      = await PostAsync(url, data);
            callback(getResponse);
        }
    }
}
