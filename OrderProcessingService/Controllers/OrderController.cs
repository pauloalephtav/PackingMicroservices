using Microsoft.AspNetCore.Mvc;
using OrderProcessingService.Infra.Interfaces;
using OrderProcessingService.Models.Request;
using OrderProcessingService.Models.Response;

namespace OrderProcessingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController(IConfiguration config, IPackingAlgorithmService packingAlgorithm,
        ILogger<OrderController> logger) : ControllerBase
    {
        private readonly IConfiguration _config = config;
        private readonly IPackingAlgorithmService _packingAlgorithm = packingAlgorithm;
        private readonly ILogger<OrderController> _logger = logger;

        /// <summary>
        /// Process orders using the packing algorithm service
        /// </summary>
        /// <param name="request"></param>
        /// <returns>
        /// Returns the response from the packing algorithm service
        /// </returns>
        [HttpPost("Process")]
        [ProducesResponseType(typeof(OrderResponse), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ProcessOrders([FromBody] OrderRequest request)
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
    }
}