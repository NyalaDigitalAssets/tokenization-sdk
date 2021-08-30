# Ganymede.SDK
##### Accessing the Bloxxon API from .NET applications

## GanymedeClient

```
var client = new GanymedeClient("API_KEY", "API_SECRET", "https://url-for-api.de");
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
