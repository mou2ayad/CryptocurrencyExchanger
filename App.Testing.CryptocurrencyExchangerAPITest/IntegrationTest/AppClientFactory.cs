using App.Components.Utilities.JWT_Auth;
using App.Services.CryptocurrencyExchangerAPI;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace App.Testing.CryptocurrencyExchangerAPITest.IntegrationTest
{
    public static class AppClientFactory
    {

        private static WebApplicationFactory<Startup>  appFactory = new WebApplicationFactory<Startup>();
        public static HttpClient GetClient() => appFactory.CreateClient();
       

    }
    public static class HttpClientAuthenticationExtension
    {
        public static async Task AuthenticationAsync(this HttpClient httpClient, string UserName, string Password)
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
                    }                   
                }
            }

        }
    }
}
