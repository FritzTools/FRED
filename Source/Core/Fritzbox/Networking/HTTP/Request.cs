﻿using System;
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

        private static async Task<String> PostAsync(String url, KeyValuePair<String, String>[] content, KeyValuePair<String, String>[] header) {
            try
            {
                return await PostAsync(url, new FormUrlEncodedContent(content), header);
            } catch (Exception) {
                return null;
            }
        }

        private static async Task<String> PostAsync(String url, KeyValuePair<String, String>[] content) {
            try {
                return await PostAsync(url, new FormUrlEncodedContent(content));
            } catch (Exception) {
                return null;
            }
        }

        private static async Task<String> PostAsync(String url, String content) {
            try {
                return await PostAsync(url, new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded"));
            } catch (Exception) {
                return null;
            }
        }

        private static async Task<String> PostAsync(String url, HttpContent content) {
            try {
                HttpResponseMessage response    = await Client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                //System.Diagnostics.Debug.Print(Convert.ToString(response.RequestMessage));

                return await response.Content.ReadAsStringAsync();
            } catch (Exception e) {
                return e.Message;
            }
        }


        private static async Task<String> PostAsync(String url, HttpContent content, KeyValuePair<String, String>[] header) {
            try {
                Client.DefaultRequestHeaders.Clear();
                foreach (KeyValuePair<String, String> head in header) {
                    Client.DefaultRequestHeaders.Add(head.Key, head.Value);
                }

                HttpResponseMessage response = await Client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                //System.Diagnostics.Debug.Print(Convert.ToString(response.RequestMessage));

                return await response.Content.ReadAsStringAsync();
            } catch (Exception e) {
                return e.Message;
            }
        }

        public static async Task Get(String url, Action<String> callback) {
            callback(await GetAsync(url));
        }

        public static async Task Post(String url, KeyValuePair<String, String>[] data, KeyValuePair<String, String>[] header, Action<String> callback) {
            callback(await PostAsync(url, data, header));
        }

        public static async Task Post(String url, MultipartFormDataContent data, KeyValuePair<String, String>[] header, Action<String> callback) {
            callback(await PostAsync(url, data, header));
        }

        public static async Task Post(String url, HttpContent data, Action<String> callback) {
            callback(await PostAsync(url, data));
        }

        public static async Task Post(String url, KeyValuePair<String, String>[] data, Action<String> callback) {
            callback(await PostAsync(url, data));
        }

        public static async Task Post(String url, String data, Action<String> callback) {
            callback(await PostAsync(url, data));
        }
    }
}
