using Ganymede.SDK.Extensions;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;

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

        partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
        {
            settings.NullValueHandling = NullValueHandling.Ignore;
        }

        private HttpRequestMessage Sign(HttpRequestMessage request)
        {
            var signature = request.Sign(_apiSecret);
            request.Headers.Authorization = new AuthenticationHeaderValue("basic", $"{_apiKey}:{signature}");
            return request;
        }
    }
}
