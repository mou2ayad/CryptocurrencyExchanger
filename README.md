## The Solution consists of these Projects

 #### 1. App.Components.Contracts:
 > Class Library project (.net standard), it contains the main interface of the Exchange Rate Provider and the ExchangeRateList object 
 #### 2. App.Components.Utilities
 > Class Library project (.net standard), it is a shared component, doesn't cover any business logic, practically it doesn't belong to any specific solution, it can be used in any other solution.
it provides the Error handling, Logging, CustomExceptions, JWT, Swagger and RecurringJobs features to any .net solution

 #### 3. App.Components.CoinmarketcapApiClient
> Class Library project (.net standard), it is the integration with Coinmarketcap Api 
 #### 4. App.Testing.CoinmarketcapAPIClientTest
 > .net Core 3.1 xUnit test, contains the unit test project of App.Components.CoinmarketcapApiClient project (Project 3)
 #### 5. App.Components.ExchangeratesApiClient
 > Class Library project (.net standard), it is the integration with Exchangerates.io Api
 #### 6. App.Testing.ExchangeratesAPIClientTest
 > .net Core 3.1 xUnit test, contains the unit test project of App.Components.ExchangeratesApiClient project (Project 5)
 
#### 7. App.Services.CryptocurrencyExchangerAPI
> .net Core 3.1 RESTful API, it is the Startup project and the final user front API
>
> - Swagger URL : https://localhost:44390/swagger/index.html
The API can be tested using Swagger directly without needing yo use postman or fiddler 
 > - Authentication Endpoint : https://localhost:44390/api/authentication/token
 it is POST request, it is needed to generate the bearer token, We can use this JSON to login and get the required permission  
    {
      "username": "**knab**",
      "password": "**knab2021**"
    }
>
> - Quotes Endpoint : https://localhost:44390/api/v1/cryptocurrency/quotes/{*symbol*} , GET request, we need to use the generated auth token from the previous endpoint, and pass it to the header of this request as following 
   "bearer {*token*}" 
   >
#### 8. App.Testing.CryptocurrencyExchangerAPITest
 > .Net Core 3.1 xUnit test, contains:
 >  - The unit test project of the services of App.Components.CryptocurrencyExchangerAPI project (Project 8)
 >  - Integration Test to the final RESTful API CryptocurrencyExchangerAPI


 
