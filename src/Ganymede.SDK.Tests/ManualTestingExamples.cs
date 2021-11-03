using Bogus;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ganymede.SDK.Tests
{
    [TestFixture]
    public class ManualTestingExamples
    {
        private static readonly Faker<CreateCustomerAccountDto> _customerFaker = new Faker<CreateCustomerAccountDto>()
            .RuleFor(c => c.Id, (f, c) => Guid.NewGuid())
            .RuleFor(c => c.BirthDate, (f, c) => f.Date.Between(new DateTime(1950, 1, 1), DateTime.UtcNow.AddYears(-21)))
            .RuleFor(c => c.Salutation, (f, c) => f.Random.Bool() ? "Herr" : "Frau")
            .RuleFor(c => c.Firstname, (f, c) => f.Name.FirstName())
            .RuleFor(c => c.Lastname, (f, c) => f.Name.LastName())
            .RuleFor(c => c.Email, (f, c) => $"test-{Guid.NewGuid()}@faker.com")
            .RuleFor(c => c.PhoneNumber, (f, c) => "+123 123 1231 123")
            .RuleFor(c => c.Type, (f, c) => AccountTypes.Person)
            .RuleFor(c => c.CountryIso, (f, c) => "DE")
            .RuleFor(c => c.Town, (f, c) => f.Address.City())
            .RuleFor(c => c.Street, (f, c) => f.Address.StreetName())
            .RuleFor(c => c.StreetNo, (f, c) => f.Random.Number(0,256).ToString())
            .RuleFor(c => c.PostalCode, (f, c) => f.Random.Number(10000, 19999).ToString());


        private readonly GanymedeClient _client =
            new GanymedeClient("", "", "https://localhost:4447");

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
            var xlmTokenId = Guid.Parse("0ac46a00-562f-4c62-8361-422791f75204");
            var repeats = Enumerable.Repeat(0, 99);

            Parallel.ForEach(repeats, new ParallelOptions { MaxDegreeOfParallelism = 1 }, (_) =>
            {
                var customerData = _customerFaker.Generate();
                var customerId = customerData.Id.Value;

                try
                {
                    var customer = _client.GetCustomerAsync(customerId).ConfigureAwait(false).GetAwaiter().GetResult();
                }
                catch (ApiException e)
                {
                    if (e.StatusCode == 404)
                    {
                        var cId = _client.CreateCustomerAsync(customerData)
                            .ConfigureAwait(false)
                            .GetAwaiter()
                            .GetResult();

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
