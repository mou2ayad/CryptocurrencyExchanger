using App.Components.Utilities.JWT_Auth;
using CryptocurrencyExchanger;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace App.Testing.CryptocurrencyExchangerAPITest.IntegrationTest
{
    public class IntegrationTest
    {
        protected readonly HttpClient httpClient;
        public IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>();
            httpClient = appFactory.CreateClient();
        }
        protected async Task Authentication(string UserName, string Password)
        {
            string token;
            using (var request = new HttpRequestMessage(HttpMethod.Post, "api/authentication/token"))
            {
                AuthenticateRequest authenticateRequest = new AuthenticateRequest() { Password = Password, Username = UserName };
                var json = JsonConvert.SerializeObject(authenticateRequest);
                using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    request.Content = stringContent;
                    var response = await httpClient.SendAsync(request);
                    var responseString = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        var authResponse = JsonConvert.DeserializeObject<AuthenticateResponse>(responseString);
                        token = authResponse.Token;
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
                        return;
                    }
                    throw new Exception("Unauthorized");
                }
            }

        }

    }
}
