﻿using Bogus;
using Newtonsoft.Json;
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
            .RuleFor(c => c.NationalityIso, (f, c) => "DE")
            .RuleFor(c => c.Gender, (f, c) => GenderTypes.Female)
            .RuleFor(c => c.Town, (f, c) => f.Address.City())
            .RuleFor(c => c.Street, (f, c) => f.Address.StreetName())
            .RuleFor(c => c.StreetNo, (f, c) => f.Random.Number(0, 256).ToString())
            .RuleFor(c => c.PostalCode, (f, c) => f.Random.Number(10000, 19999).ToString());


        private readonly GanymedeClient _client =
            new GanymedeClient("", "", "http://localhost:4447/");

        [TestCase("C55B0489-059D-4739-A1F9-89606C4116DB")]
        [TestCase("1135C3EA-18A8-497B-86B8-12157D98880C")]
        [TestCase("7FD4D028-2A32-4D5D-816D-7A631F86A9DD")]
        [TestCase("42D554E9-329C-4FCF-986B-3DA44724F52B")]
        [TestCase("851AF314-0ADF-4D9D-9500-EB784C86C18B")]
        public async Task CreateRetailWallets(Guid customerId)
        {
            var retailId = await _client.CreateRetailWalletAsync(customerId, new SimpleAccessCredentialsDto
            {
                Passphrase = "Password123"
            }, Blockchains.Algorand);
        }

        [Test]
        public void EmulateFunding()
        {
            var xlmTokenId = Guid.Parse("567bc204-bcef-440a-b964-8c780a49cf84");
            var repeats = Enumerable.Repeat(0, 5);

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


                //Create wallets
                var wallets = _client.CreateRetailWalletsAsync(customerId, new SimpleAccessCredentialsDto { Passphrase = "Nyala1234567" }).ConfigureAwait(false).GetAwaiter().GetResult();
                Assert.NotNull(wallets);


                // Update kyc
                var customerDto = new KycDataDto
                {
                    Title = customerData.Title,
                    Firstname = customerData.Firstname,
                    Lastname = customerData.Lastname,
                    PlaceOfBirth = "Berlin",
                    DateOfBirth = customerData.BirthDate.Value,
                    Email = customerData.Email,
                    NonPepPerson = true,
                    HighCorruptionIndex = false,
                    NonSanctionedCountry = true,
                    NonUsTaxPerson = true,
                    IdentVerified = true,
                    EulaAgreed = true,
                    Address = new KycAddressDto()
                    {
                        CountryCodeIso2 = customerData.CountryIso,
                        PostalCode = customerData.PostalCode,
                        Street = customerData.Street,
                        StreetNo = customerData.StreetNo,
                        Town = customerData.Town
                    },
                    NationalityIso = customerData.NationalityIso,
                    Gender = customerData.Gender
                };

                var response = _client.UpdateCustomerKycDataAsync(customerId, customerDto).GetAwaiter().GetResult();


                Assert.IsTrue(response);

                // Opt-In for token
                var success = _client.CreateTokenizedAssetOptInAsync(customerId, wallets.FirstOrDefault(o => o.Blockchain == Blockchains.Stellar).Id, new RetailWalletOptInDto
                {
                    TokenizedAssetId = xlmTokenId,
                    Credentials = new SimpleAccessCredentialsDto
                    {
                        Passphrase = "Nyala1234567",
                    }
                })
                .ConfigureAwait(false).GetAwaiter().GetResult();
                Assert.IsTrue(success);
            });
        }

        [TestCase("E3D86A22-7E6F-4C74-ADFA-9892178B8F93", "2BDBDE7E-F9AB-4F51-8789-027ACB83A7B4")]
        public void Should_Optin_For_Token(Guid customerId, Guid retailWalletId)
        {
            // Opt-In for token
            var success = _client.CreateTokenizedAssetOptInAsync(customerId, retailWalletId, new RetailWalletOptInDto
            {
                TokenizedAssetId = Guid.Parse("567bc204-bcef-440a-b964-8c780a49cf84"),
                Credentials = new SimpleAccessCredentialsDto
                {
                    Passphrase = "Nyala1234567",
                }
            })
            .ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsTrue(success);
        }

        [TestCase("E3D86A22-7E6F-4C74-ADFA-9892178B8F93")]
        public async Task Should_Update_KYC_Data(Guid customerId)
        {
            var customerDto = new KycDataDto
            {
                Title = "Mr.",
                Firstname = "Deontae",
                Lastname = "Jacob",
                PlaceOfBirth = "Berlin",
                DateOfBirth = DateTime.Now.AddYears(-37),
                Email = "test-ace79e82-9422-4024-af7d-172db7ec79be1@faker.com",
                NonPepPerson = true,
                HighCorruptionIndex = false,
                NonSanctionedCountry = true,
                NonUsTaxPerson = true,
                IdentVerified = true,
                EulaAgreed = true,
                Address = new KycAddressDto()
                {
                    CountryCodeIso2 = "DE",
                    PostalCode = "71345",
                    Street = "Right",
                    StreetNo = "30",
                    Town = "Berlin"
                },
                NationalityIso = "DE",
                Gender = GenderTypes.Male
            };

            var response = await _client.UpdateCustomerKycDataAsync(customerId, customerDto);
            Assert.IsTrue(response);
        }

        [TestCase("E3D86A22-7E6F-4C74-ADFA-9892178B8F93", "Nyala1234567")]
        public async Task Should_Check_Retail_Wallet_Passphrase(Guid customerId, string passphrase)
        {
            var response = await _client.CheckRetailWalletPassphrase(customerId, passphrase);
            Assert.IsTrue(response);
        }
        [TestCase("ADB983B8-7ED3-43EA-8B2F-FFDC70C736ED")]
        public async Task Should_Get_Retail_Wallet(Guid customerId)
        {
            var response = await _client.GetRetailWalletsAsync(customerId);
            Assert.NotNull(response);
        }
    }
}
