using Newtonsoft.Json;
using PackingAlgorithmService.Infra.Interfaces;
using PackingAlgorithmService.Models;
using System.Net.Http.Headers;
using System.Text;

namespace OrderProcessingService.Infra.Services
{
    public class BoxManagementService(IConfiguration configuration) : IBoxManagementService
    {
        private readonly IConfiguration _config = configuration;
        private string GetBoxManagementServiceUrl() => _config["BoxManagementService:BaseURL"] ??
            throw new ArgumentNullException("BaseURL of BoxManagementService is not confired.");

        private string GetBoxManagementServiceApiKey() => _config["BoxManagementService:SecretKey"] ??
            throw new ArgumentNullException("ApiKey of BoxManagementService is not confired.");

        public async Task<List<Box>> GetAvailableBoxes()
        {
            string apiUrl = GetBoxManagementServiceUrl();

            //TODO: Implement authentication and authorization
            // remove the hardcoded key and url parameters
            string key = GetBoxManagementServiceApiKey();

            string content = await SendRequestAsync(apiUrl + "/api/Box/GetAvailableBoxes", HttpMethod.Get, "", key);

            return JsonConvert.DeserializeObject<List<Box>>(content);
        }

        private async Task<string> SendRequestAsync(string url, HttpMethod httpMethod, string requestJson, string apiKey)
        {
            using (HttpClient client = new())
            {
                client.Timeout = TimeSpan.FromMinutes(5);
                HttpRequestMessage httpRequest = new(httpMethod, url);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(Encoding.ASCII.GetBytes(apiKey)));

                if (httpMethod == HttpMethod.Post)
                    httpRequest.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.SendAsync(httpRequest);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    return content;
                else
                    throw new ArgumentException($"Erro request: {response.StatusCode} => {content}");
            }
        }
    }
}
