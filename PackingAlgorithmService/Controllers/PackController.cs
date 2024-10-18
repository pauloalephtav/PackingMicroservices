using Microsoft.AspNetCore.Mvc;
using PackingAlgorithmService.Models;
using PackingAlgorithmService.Models.Request;

namespace PackingAlgorithmService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PackController : ControllerBase
    {
        private readonly IConfiguration _config = config;
        private readonly IPackingAlgorithmService _packingAlgorithm = packingAlgorithm;
        private readonly ILogger<OrderController> _logger = logger;

        /// <summary>
        /// Pack orders choosing the best matching box for each product
        /// </summary>
        /// <param name="request"></param>
        /// <returns>
        /// </returns>
        [HttpPost("Process")]
        [ProducesResponseType(typeof(OrderResponse), 200)]
        [ProducesResponseType(400)]
        public IActionResult PackOrders([FromBody] OrderRequest request)
        {
            try
            {
                if (!IsAuthorized(request.SecretKey))
                {
                    _logger.LogWarning("Unauthorized request.");
                    return Unauthorized("Invalid SecretKey.");
                }

                if (request.Orders == null || request.Orders.Count == 0)
                {
                    _logger.LogWarning("No orders to process.");
                    return BadRequest("No orders to process.");
                }

                var orderResponse = new OrderResponse()
                {
                    Orders = await _packingAlgorithm.CallPackingAlgorithmService(request.Orders)
                };

                _logger.LogInformation("Orders processed successfully.");
                return Ok(orderResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        //TODO: NEcessary to move to a authentication service
        // and implement a token based authentication
        private bool IsAuthorized(string secretKey)
        {
            var validSecretKey = _config["Security:ValidSecretKey"];

            return secretKey == validSecretKey;
        }
        [HttpPost("PackOrders")]
        [ProducesResponseType(typeof(OrderProcessingResult), 200)]
        public IActionResult PackOrders([FromBody] OrderRequest request)
        {
            var result = new List<OrderProcessingResult>();

            foreach (var order in orders)
            {
                var packedOrder = PackOrder(order);
                result.Add(new OrderProcessingResult
                {
                    OrderId = order.Id,
                    Boxes = packedOrder
                });
            }

            return Ok(result);
        }

        private List<Box> PackOrder(Order order)
        {
            // Obter a lista de caixas disponíveis do Box Management Service
            var availableBoxes = GetAvailableBoxes();
            var packedBoxes = new List<Box>();

            foreach (var product in order.Products)
            {
                var box = FindBoxForProduct(product, availableBoxes);
                if (box != null)
                {
                    box.Products.Add(product);
                }
                else
                {
                    throw new Exception($"No suitable box found for product {product.Name}");
                }
            }

            return packedBoxes;
        }

        private List<Box> GetAvailableBoxes()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://boxmanagementservice/api/boxes");
                var response = httpClient.GetAsync("/").Result;
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsAsync<List<Box>>().Result;
            }
        }

        private Box FindBoxForProduct(Product product, List<Box> availableBoxes)
        {
            foreach (var box in availableBoxes)
            {
                if (box.Height >= product.Height && box.Width >= product.Width && box.Length >= product.Length)
                {
                    return box;
                }
            }
            return null;
        }
    }






    [ApiController]
    [Route("[controller]")]
    public class PackController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<PackController> _logger;

        public PackController(ILogger<PackController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
