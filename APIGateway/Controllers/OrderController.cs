using APIGateway.Infra.Interfaces;
using APIGateway.Models;
using APIGateway.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIGateway.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController(IOrdersService ordersService) : ControllerBase
    {
        private readonly IOrdersService _ordersService = ordersService;

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
                if (request.Orders == null || request.Orders.Count == 0)
                {
                    return BadRequest("No orders to process.");
                }

                var orderResponse = await _ordersService.CallOrderProcessingService(request.Orders);

                return Ok(orderResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}