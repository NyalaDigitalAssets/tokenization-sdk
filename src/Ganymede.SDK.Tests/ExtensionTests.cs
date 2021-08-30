using Ganymede.SDK.Extensions;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Text;

namespace Ganymede.SDK.Tests
{
    [TestFixture]
    public class ExtensionTests
    {
        private readonly string _apiSecret = "SuperS3cr3tKey";

        [TestCase("https://some-url.de/api/external/v1/customers", "YUNGA0gLdx5bJbm5Ym+7Q/N6ZaEUI+zyuFV0Lfob0GY=")]
        [TestCase("https://some-url.de/api/external/v1/customers/2e62ff27-83dc-41e6-924b-2018279f47ab/retail-wallets", "gmvnqf4240txCwP6DL+D9vqD+uAzw7/wiy9k3LhV+Rw=")]
        public void Should_Sign_GET_Requests(string url, string expectedSignature)
        {
            var uri = new Uri(url);
            var req = new HttpRequestMessage(HttpMethod.Get, uri);

            var signature = req.Sign(_apiSecret);

            Assert.AreEqual(expectedSignature, signature);
        }

        [Test]
        public void Should_Sign_POST_Requests()
        {
            var uri = new Uri("https://some-url.de/api/external/v1/customers");
            var req = new HttpRequestMessage(HttpMethod.Post, uri);

            var json = JsonConvert.SerializeObject(new CreateCustomerAccountDto
            {
                Id = Guid.NewGuid(),
                BirthDate = DateTime.Now.AddYears(-30),
                Salutation = "Herr",
                Firstname = "Firstname",
                Lastname = "Lastname",
                Email = "test@test.de",
                PhoneNumber = "+123 123 1231 123",
                Type = AccountTypes.Person,
                CountryIso = "DE",
                Town = "Town",
                Street = "Street",
                StreetNo = "42",
                PostalCode = "1337",
            });
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var signature = req.Sign(_apiSecret);

            Assert.AreEqual("pEVuTrNY1xMm/3qt4m+K8EiZ8KRjxQ12Xs6s32Ml3U0=", signature);
        }
    }
}
