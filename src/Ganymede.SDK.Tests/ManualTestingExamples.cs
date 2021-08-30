using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Ganymede.SDK.Tests
{
    [TestFixture]
    public class ManualTestingExamples
    {
        private readonly GanymedeClient _client = 
            new GanymedeClient("", "", "http://localhost:4447");

        [Test]
        public async Task CreateRetailWallets()
        {
            var customerId = new Guid("2bc4db1d-2da8-42de-a1ae-009d2f368040");
            var retailId = await _client.CreateRetailWalletsAsync(customerId, new SimpleAccessCredentialsDto
            {
                Passphrase = "Bloxxon1234"
            });
        }
    }
}
