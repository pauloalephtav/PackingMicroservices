using APIGateway.Infra.Interfaces;
using APIGateway.Models;
using APIGateway.Models.Request;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

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
            string key = GetOrderProcessingServiceApiKey();

            string requestJson = JsonConvert.SerializeObject(CreateOrderRequest(orders));

            string content = await SendRequestAsync(apiUrl + "/api/Order/Process", HttpMethod.Post, requestJson, key);

            return JsonConvert.DeserializeObject<OrderResponse>(content);
        }

        private static OrderProcessingRequest CreateOrderRequest(List<Order> orders)
        {
            return new OrderProcessingRequest
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