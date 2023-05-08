# Tokenization.SDK
##### Accessing the Nyala API from .NET applications

## TokenizationClient

```
var client = new TokenizationClient("API_KEY", "API_SECRET", "https://url-for-api.de");
```


## Getting a Customer
```
var customerId = new Guid("28912b27-e10f-4b64-9119-b98dfa20938e");
var customer = await client.GetCustomerAsync(customerId);
```


## Creating a Customer
```
var customer = new CreateCustomerAccountDto
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

var customerId = await client.CreateCustomerAsync(customer);
```


## Searching for a Customer
```
var searchText = "John";
var customers = await client.SearchCustomerAsync(searchText);
```


## Creating Retail Wallets
```
var customerId = Guid.Parse("67d78d46-ff24-4e18-90a1-a5738349b606");
var wallets = await client.CreateRetailWalletsAsync(customerId, new SimpleAccessCredentialsDto
{
    Passphrase = "S3cur3-P4ssphr4s3"
});
```


## Recover Retail Wallet access
```
var customerId = Guid.Parse("67d78d46-ff24-4e18-90a1-a5738349b606");
var seedRecoveryData = await client.InitiateCustomersRetailWalletsRecoveryAsync(customerId);
await _client.RecoverRetailWalletSeedAccessAsync(customerId, new ResetRetailWalletAccessCredentialsDto
{
    Passphrase = "N3w-P4ssphras3",
    RecoveryKey = seedRecoveryData.RecoveryKey,
});
```


## Update kyc data for customer
```
var customerId = Guid.Parse("67d78d46-ff24-4e18-90a1-a5738349b606");
var customerDto = new KycDataDto
{
    Title = "Mr.",
    Firstname = "Deontae",
    Lastname = "Jacob",
    PlaceOfBirth = "Berlin",
    DateOfBirth = DateTime.Now.AddYears(-37),
    Email = "test-ace79e82-9422-4024-af7d-172db7ec79be3@faker.com",
    NonPepPerson = true,
    HighCorruptionIndex = false,
    NonSanctionedCountry = true,
    NonUsTaxPerson = true,
    IdentVerified = true,
    IdentVerifiedType = IdentVerifiedType.Normal,
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
    Gender = GenderTypes.Male,
    Bic = "BIC-New-up"
};
var response = await _client.UpdateCustomerKycDataAsync(customerId, customerDto);
```
