using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ganymede.SDK.Tests
{
    [TestFixture]
    public class ManualTestingExamples
    {
        private readonly GanymedeClient _client =
            new GanymedeClient("", "", "http://localhost:4447");

        //[Test]
        public async Task CreateRetailWallets()
        {
            var customerId = new Guid("2bc4db1d-2da8-42de-a1ae-009d2f368040");
            var retailId = await _client.CreateRetailWalletsAsync(customerId, new SimpleAccessCredentialsDto
            {
                Passphrase = "Bloxxon1234"
            });
        }

        [Test]
        public void EmulateFunding()
        {
            var xlmTokenId = Guid.Parse("67d78d46-ff24-4e18-90a1-a5738349b606");
            var repeats = Enumerable.Repeat(0, 5);

            Parallel.ForEach(repeats, new ParallelOptions { MaxDegreeOfParallelism = 1 }, (_) =>
            {
                var customerId = Guid.NewGuid();

                try
                {
                    var customer = _client.GetCustomerAsync(customerId).ConfigureAwait(false).GetAwaiter().GetResult();
                }
                catch (ApiException e)
                {
                    if (e.StatusCode == 404)
                    {
                        var cId = _client.CreateCustomerAsync(new CreateCustomerAccountDto
                        {
                            Id = customerId,
                            BirthDate = DateTime.Now.AddYears(-30),
                            Salutation = "Herr",
                            Firstname = "Firstname",
                            Lastname = "Lastname",
                            Email = $"{customerId}@test.de",
                            PhoneNumber = "+123 123 1231 123",
                            Type = AccountTypes.Person,
                            CountryIso = "DE",
                            Town = "Town",
                            Street = "Street",
                            StreetNo = "42",
                            PostalCode = "1337",
                        }).ConfigureAwait(false).GetAwaiter().GetResult();

                        Assert.AreEqual(cId, customerId);
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception)
                {
                    throw;
                }

                // Create wallets
                var wallets = _client.CreateRetailWalletsAsync(customerId, new SimpleAccessCredentialsDto { Passphrase = "Bloxxon1234" }).ConfigureAwait(false).GetAwaiter().GetResult();
                var xlmWallet = wallets.FirstOrDefault(w => w.AssetType == AssetTypes.XLM);
                Assert.NotNull(xlmWallet);

                // Opt-In for token
                var success = _client.CreateTokenizedAssetOptInAsync(customerId, xlmWallet.Id, new RetailWalletOptInDto
                {
                    TokenizedAssetId = xlmTokenId,
                    Credentials = new SimpleAccessCredentialsDto
                    {
                        Passphrase = "Bloxxon1234",
                    }
                })
                .ConfigureAwait(false).GetAwaiter().GetResult();
                Assert.IsTrue(success);
            });
        }
    }
}
