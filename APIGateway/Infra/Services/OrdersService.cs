using APIGateway.Infra.Interfaces;
using APIGateway.Models;
using APIGateway.Models.Request;
using System.Text.Json;

namespace APIGateway.Infra.Services
{
    public class OrdersService(IConfiguration configuration) : IOrdersService
    {
        private readonly IConfiguration _config = configuration;
        private string GetOrderProcessingServiceUrl() => _config["OrderProcessingService:BaseURL"] ??
            throw new ArgumentNullException("BaseURL of OrderProcessingService is not confired.");

        private string GetOrderProcessingServiceApiKey() => _config["OrderProcessingService:SecretKey"] ??
            throw new ArgumentNullException("ApiKey of OrderProcessingService is not confired.");

        public async Task<OrderResponse> CallOrderProcessingService(List<Order> orders)
        {
            string apiUrl = GetOrderProcessingServiceUrl();

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(apiUrl);
                var response = await httpClient.PostAsJsonAsync("/process", CreateOrderRequest(orders));
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OrderResponse>(jsonString);
                return result;
            }
        }

        private OrderProcessingRequest CreateOrderRequest(List<Order> orders)
        {
            return new OrderProcessingRequest
            {
                SecretKey = GetOrderProcessingServiceApiKey(),
                Orders = orders
            };
        }
    }
}