using Microsoft.AspNetCore.Mvc;
using PackingAlgorithmService.Models.Request;
using PackingAlgorithmService.Models.Response;
using PackingAlgorithmService.Services;
using System.Text;

namespace PackingAlgorithmService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PackController(IConfiguration config, ILogger<PackController> logger,
        IPackingService packingService) : ControllerBase
    {
        private readonly IConfiguration _config = config;
        private readonly IPackingService _packingService = packingService;
        private readonly ILogger<PackController> _logger = logger;

        /// <summary>
        /// Pack orders choosing the best matching box for each product
        /// </summary>
        /// <param name="request"></param>
        /// <returns>
        /// </returns>
        [HttpPost("Process")]
        [ProducesResponseType(typeof(PackingResponse), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PackOrders([FromBody] PackingRequest request)
        {
            try
            {
                var secretKey = Request.Headers.Authorization.ToString();

                if (!IsAuthorized(secretKey))
                {
                    _logger.LogWarning("Unauthorized request.", nameof(PackController));
                    return Unauthorized("Invalid SecretKey.");
                }

                if (request.Orders == null || request.Orders.Count == 0)
                {
                    _logger.LogWarning("No orders to process.", nameof(PackController));
                    return BadRequest("No orders to process.");
                }

                var result = new List<PackingResponse>();

                foreach (var order in request.Orders)
                {
                    var packedOrder = await _packingService.PackOrder(order);
                    result.Add(new PackingResponse
                    {
                        OrderId = order.OrderId,
                        Boxes = packedOrder
                    });
                }

                _logger.LogInformation("Orders processed successfully.", nameof(PackController));
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, nameof(PackController));
                return BadRequest(ex.Message);
            }
        }

        //TODO: NEcessary to move to a authentication service
        // and implement a token based authentication
        private bool IsAuthorized(string secretKey)
        {
            var key = _config["Security:ValidSecretKey"];
            if (string.IsNullOrEmpty(key))
            {
                _logger.LogWarning("SecretKey is not configured.");
                return false;
            }

            if (string.IsNullOrEmpty(secretKey))
            {
                _logger.LogWarning("SecretKey is not provided by client.");
                return false;
            }

            var validSecretKey = Convert.ToBase64String(Encoding.ASCII.GetBytes(key));

            return secretKey == "Basic " + validSecretKey;
        }

    }
}
