# General description

On this page you will find the brief description on the general tokenization process. The detailed explanation of the steps are as follows:

### Create Customer

The very first step is to create the customer using available SDK. Upn creation of customer, you will receive an unique customer id which will be used for further steps.

### Update KYC Data

After creation of the customer, you can update the customer details and submit the kyc data with all information as required. This can be done anytime.

### Create wallets

The second step is to create wallet for the customer. Upon wallet creation, you will receive proposed reratil wallet id back. The wallet opening request will go to custody for the final review. Upon approval from custodian wallets are created on blockchain.

### Get tokenized asset Id

SmartRegistry will approve the token creation from Nyala and upon token creation on blokcchain unique tokenizedasset id will be provided to you which needs to be used in further steps.

### Opt In for asset

The next step is to optin for the asset with customer id and tokenized asset id. This will create a request to hold an assets for customer.

### Authorize Optins

SmartRegistry will authorize the optins for the final token transfer.

### Transfer Token

Tokens will be then transferred to customer retail wallets.

### Additional stuffs

- Check PIN before requesting optin to make sure that the correct pin is supplied
- Initiate a recovery process if PIN is forgetten
