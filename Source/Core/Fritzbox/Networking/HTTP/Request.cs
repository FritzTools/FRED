using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FRED.Core.Fritzbox.Networking.HTTP {
    class Request {
        private static readonly HttpClient Client = new HttpClient();
        private static async Task<String> GetAsync(String url) {
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

            HttpResponseMessage response = await Client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private static async Task<String> PostAsync(String url, String data){
            HttpContent content             = new StringContent(data);
            HttpResponseMessage response    = await Client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task Get(String url, Action<String> callback) {
            String getResponse = await GetAsync(url);
            callback(getResponse);
        }

        public static async Task Post(String url, String data, Action<String> callback) {
            String getResponse      = await PostAsync(url, data);
            callback(getResponse);
        }
    }
}
