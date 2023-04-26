using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tokenization.SDK
{
    public class TokenizationClient
    {
        private static readonly HttpClient _httpClient = new();
        private readonly Client _client;

        /// <summary>
        /// Creates instance of the Tokenization client
        /// </summary>
        /// <param name="key">API public key</param>
        /// <param name="secret">API private secret</param>
        /// <param name="apiUrl">Defaults to https://api.bloxxon.co</param>
        public TokenizationClient(string key, string secret, string apiUrl = "https://api.bloxxon.co")
        {
            _client = new Client(key, secret, apiUrl, _httpClient);
        }

        #region Customer

        /// <summary>
        /// Get list of all customers
        /// </summary>
        /// <returns>All customers</returns>
        public async Task<IEnumerable<CustomerAccountDto>> GetCustomersAsync()
        {
            var response = await _client.ApiExternalV1CustomersGetAsync();
            return response.Data;
        }

        /// <summary>
        /// Get customer by customer ID
        /// </summary>
        /// <param name="customerId">The GUID ID of the customer</param>
        /// <returns>Customer details</returns>
        public async Task<CustomerAccountDto> GetCustomerAsync(Guid customerId)
        {
            var response = await _client.ApiExternalV1CustomersGetAsync(customerId);
            return response.Data;
        }

        /// <summary>
        /// Searches the customer database and returns a list of customers. 
        /// </summary>
        /// <param name="seachText">Applied as starting with on the columns: Firstname, Lastname or Email</param>
        /// <param name="take">Result set size</param>
        /// <param name="skip">Number of results to skip</param>
        /// <returns></returns>
        public async Task<IEnumerable<CustomerAccountDto>> SearchCustomerAsync(string seachText, int take = 50, int skip = 0)
        {
            var response = await _client.ApiExternalV1SearchAsync(seachText, take, skip);
            return response.Data;
        }

        /// <summary>
        /// Create new customer. Throws exception if email already used or if ID is provided and already used
        /// </summary>
        /// <param name="data">All required data for creating a new customer</param>
        /// <returns>The GUID ID of the newly created customer</returns>
        public async Task<Guid> CreateCustomerAsync(CreateCustomerAccountDto data)
        {
            var response = await _client.ApiExternalV1CustomersPostAsync(data);
            return response.Data;
        }

        /// <summary>
        /// Update customer details. Throws exception if email already used or if ID is provided and already used
        /// </summary>
        /// <param name="customerId">The GUID ID of the customer to update</param>
        /// <param name="data">All updatable data</param>
        /// <returns>The GUID ID of the updated customer</returns>
        public async Task<Guid> UpdateCustomerAsync(Guid customerId, UpdateCustomerAccountDto data)
        {
            var response = await _client.ApiExternalV1CustomersPutAsync(customerId, data);
            return response.Data;
        }

        /// <summary>
        /// Update customer KYC details.
        /// </summary>
        /// <param name="customerId">The GUID ID of the customer to update</param>
        /// <param name="data">All updatable data</param>
        public async Task<bool> UpdateCustomerKycDataAsync(Guid customerId, KycDataDto data)
        {
            var response = await _client.ApiExternalV1CustomersKycAsync(customerId, data);
            return response.Data;
        }

        #endregion

        #region Retail Wallet

        /// <summary>
        /// Gets all retail wallets for the specified customer
        /// </summary>
        /// <param name="customerId">The GUID ID of the customer</param>
        /// <returns>List of wallet details if any</returns>
        public async Task<IEnumerable<RetailWalletDto>> GetRetailWalletsAsync(Guid customerId)
        {
            var response = await _client.ApiExternalV1CustomersRetailWalletsGetAsync(customerId);
            return response.Data;
        }

        /// <summary>
        /// Created wallets for all supported blockchains for the specific customer. If customer already has wallets no new wallets are created. 
        /// </summary>
        /// <param name="customerId">The GUID ID of the customer for whom wallets are to be created</param>
        /// <param name="credentials">The customer Passphrase is required. KeyFileContent is optinal (configurable)</param>
        /// <returns></returns>
        public async Task<IEnumerable<RetailWalletDto>> CreateRetailWalletsAsync(Guid customerId, SimpleAccessCredentialsDto credentials)
        {
            var response = await _client.ApiExternalV1CustomersRetailWalletsPostAsync(customerId, credentials);
            return response.Data;
        }

        /// <summary>
        /// Creates a new wallet for the give customer. If the customer already has a wallet with the given assetType no new wallet will be created.
        /// </summary>
        /// <param name="customerId">The GUID ID of the customer for whom wallets are to be created</param>
        /// <param name="credentials">The customer Passphrase is required. KeyFileContent is optinal (configurable)</param>
        /// <param name="assetType"></param>
        /// <returns></returns>
        public async Task<RetailWalletDto> CreateRetailWalletAsync(Guid customerId, SimpleAccessCredentialsDto credentials, Blockchains blockchain)
        {
            var response = await _client.ApiExternalV1CustomersRetailWalletsPutAsync(customerId, (int)blockchain, credentials);
            return response.Data;
        }

        /// <summary>
        /// Create an Opt In for a specified Tokenized Asset.
        /// </summary>
        /// <param name="customerId">The GUID ID of the customer</param>
        /// <param name="walletId">The reatail wallet GUID ID</param>
        public async Task<bool> CreateTokenizedAssetOptInAsync(Guid customerId, Guid walletId, RetailWalletOptInDto data)
        {
            var response = await _client.ApiExternalV1CustomersRetailWalletsOptInAsync(customerId, walletId, data);
            return response.Data;
        }

        /// <summary>
        /// Create an Opt In for a specified Tokenized Asset.
        /// </summary>
        /// <param name="customerId">The GUID ID of the customer</param>
        public async Task<bool> CreateTokenizedAssetOptInAsync(Guid customerId, RetailWalletOptInDto data)
        {
            var response = await _client.ApiExternalV1CustomersOptInAsync(customerId, data);
            return response.Data;
        }

        /// <summary>
        /// Initiates the retail wallet recovery process and return a one-time usable recovery key
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public async Task<RetailWalletSeedRecoveryDto> InitiateCustomersRetailWalletsRecoveryAsync(Guid customerId)
        {
            var response = await _client.ApiExternalV1CustomersRetailWalletsRecoveryPostAsync(customerId);
            return response.Data;
        }

        /// <summary>
        /// Resets the passpharse for a retail-wallet
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<bool> RecoverRetailWalletSeedAccessAsync(Guid customerId, ResetRetailWalletAccessCredentialsDto data)
        {
            var response = await _client.ApiExternalV1CustomersRetailWalletsRecoveryPutAsync(customerId, data);
            return response.Data;
        }

        /// <summary>
        /// Returns true if the retail wallet passphrase is correct for given customer.
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="passphrase"></param>
        /// <returns></returns>
        public async Task<bool> CheckRetailWalletPassphrase(Guid customerId, string passphrase)
        {
            var response = await _client.ApiExternalV1CustomersRetailWalletsCheckPassphraseAsync(customerId, new SimpleAccessCredentialsDto
            {
                Passphrase = passphrase,
            });
            return response.Data;
        }

        #endregion
    }
}
