using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace Ganymede.SDK
{
    internal partial class Client
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;

        public Client(string apiKey, string apiSecret, string baseUrl, HttpClient httpClient)
        {
            _apiKey = apiKey;
            _apiSecret = apiSecret;
            BaseUrl = baseUrl;
            _httpClient = httpClient;
            _settings = new System.Lazy<Newtonsoft.Json.JsonSerializerSettings>(CreateSerializerSettings);
        }

        /// <summary>
        /// Additional preparation to add security signature to header
        /// </summary>
        partial void PrepareRequest(HttpClient client, HttpRequestMessage request, string url)
        {
            Sign(request);
        }

        private HttpRequestMessage Sign(HttpRequestMessage request)
        {
            var signature = GenerateSignature(request);
            request.Headers.Authorization = new AuthenticationHeaderValue("basic", $"{_apiKey}:{signature}");
            return request;
        }

        private string GenerateSignature(HttpRequestMessage req)
        {
            var msg = $"{req.Content?.Headers?.ContentLength ?? 0}" +
                $"{req.Method}" +
                $"{req.RequestUri.AbsolutePath}" +
                $"{req.RequestUri.Query}"
                .ToLower();

            using var hmac = new HMACSHA256
            {
                Key = Encoding.UTF8.GetBytes(_apiSecret)
            };

            return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(msg)));
        }
    }
}
