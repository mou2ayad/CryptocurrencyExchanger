using System.Net.Http;
using Newtonsoft.Json;

namespace App.Components.Utilities.APIClient
{
    public class RestAPICaller : IRestAPICaller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RestAPICaller(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async System.Threading.Tasks.Task<T> CallAPIAsync<T>(string path, string ServiceName)
        {
            var _httpClient = _httpClientFactory.CreateClient();
            var requestTask = _httpClient.GetAsync(path);
            var response = await requestTask;
            string responseString = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new RestAPIException(ServiceName, response.StatusCode, responseString);             
            return JsonConvert.DeserializeObject<T>(responseString);
        }
        public async System.Threading.Tasks.Task<string> CallAPIAsStringAsync(string path, string ServiceName)
        {
            var _httpClient = _httpClientFactory.CreateClient();
            var requestTask = _httpClient.GetAsync(path);
            var response = await requestTask;
            string responseString = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new RestAPIException(ServiceName, response.StatusCode, responseString);
            return responseString;
        }

    }
}