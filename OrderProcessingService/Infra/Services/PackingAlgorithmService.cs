using Newtonsoft.Json;
using OrderProcessingService.Infra.Interfaces;
using OrderProcessingService.Models;
using OrderProcessingService.Models.Request;
using OrderProcessingService.Models.Response;
using System.Net.Http.Headers;
using System.Text;

namespace OrderProcessingService.Infra.Services
{
    public class PackingAlgorithmService(IConfiguration configuration) : IPackingAlgorithmService
    {
        private readonly IConfiguration _config = configuration;
        private string GetPackingAlgorithmServiceUrl() => _config["PackingAlgorithmService:BaseURL"] ??
            throw new ArgumentNullException("BaseURL of PackingAlgorithmService is not confired.");

        private string GetPackingAlgorithmServiceApiKey() => _config["PackingAlgorithmService:SecretKey"] ??
            throw new ArgumentNullException("ApiKey of PackingAlgorithmService is not confired.");

        public async Task<List<PackingResponse>> CallPackingAlgorithmService(List<Order> orders)
        {
            string apiUrl = GetPackingAlgorithmServiceUrl();
            string key = GetPackingAlgorithmServiceApiKey();

            string requestJson = JsonConvert.SerializeObject(CreatePackingRequest(orders));

            string content = await SendRequestAsync(apiUrl + "/api/Pack/Process", HttpMethod.Post, requestJson, key);

            return JsonConvert.DeserializeObject<List<PackingResponse>>(content);
        }

        private static PackingRequest CreatePackingRequest(List<Order> orders)
        {
            return new PackingRequest
            {
                Orders = orders
            };
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
