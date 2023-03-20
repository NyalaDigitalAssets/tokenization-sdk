using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace Tokenization.SDK.Extensions
{
    public static class HttpRequestMessageExtensions
    {
        public static string Sign(this HttpRequestMessage req, string secret)
        {
            var msg = $"{req.Content?.Headers?.ContentLength ?? 0}" +
                $"{req.Method}" +
                $"{req.RequestUri.AbsolutePath}" +
                $"{req.RequestUri.Query}"
                .ToLower();

            using var hmac = new HMACSHA256
            {
                Key = Encoding.UTF8.GetBytes(secret)
            };

            return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(msg)));
        }
    }
}
