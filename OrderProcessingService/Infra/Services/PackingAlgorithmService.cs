using OrderProcessingService.Infra.Interfaces;
using OrderProcessingService.Models;
using OrderProcessingService.Models.Request;
using OrderProcessingService.Models.Response;
using System.Text.Json;

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

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(apiUrl);
                var response = await httpClient.PostAsJsonAsync("/process", CreatePackingRequest(orders));
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<PackingResponse>>(jsonString);
                return result;
            }
        }

        private PackingRequest CreatePackingRequest(List<Order> orders)
        {
            return new PackingRequest
            {
                SecretKey = GetPackingAlgorithmServiceApiKey(),
                Orders = orders
            };
        }
    }
}
