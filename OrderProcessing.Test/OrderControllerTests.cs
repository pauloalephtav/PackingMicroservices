using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using OrderProcessingService.Controllers;
using OrderProcessingService.Infra.Interfaces;
using OrderProcessingService.Models;
using OrderProcessingService.Models.Request;
using OrderProcessingService.Models.Response;

namespace OrderProcessing.Test
{
    public class OrderControllerTests
    {
        private readonly Mock<IConfiguration> _configMock;
        private Mock<IPackingAlgorithmService> _packingAlgorithmMock;
        private OrderController _orderController;

        public OrderControllerTests()
        {
            _configMock = new Mock<IConfiguration>();
            _packingAlgorithmMock = new Mock<IPackingAlgorithmService>();
            _orderController = new OrderController(_configMock.Object, _packingAlgorithmMock.Object);
        }


        [Fact]
        public async Task ProcessOrders_WithValidRequest_ReturnsOkResult()
        {
            var request = GetOrderRequest("validSecretKey");

            var packingResponses = new List<PackingResponse>
            {
                new () {
                    OrderId = "1",
                        Boxes = [
                            new () {
                                BoxId = "Caixa 2",
                                Products = [
                                    new ProductResponse { ProductId =  "PS5" },
                                    new ProductResponse { ProductId = "Volante" }
                                ]
                            }
                        ]
                    },
                new () {
                    OrderId = "2",
                    Boxes = [
                        new () {
                            BoxId = "Caixa 1",
                            Products = [
                                new ProductResponse { ProductId =  "Joystick" },
                                new ProductResponse { ProductId = "Fifa 24" },
                                new ProductResponse { ProductId = "Call of Duty"}
                            ]
                        }
                    ]
                }
            };

            var orderResponse = new OrderResponse()
            {
                Orders = packingResponses
            };

            _configMock.Setup(c => c["Security:ValidSecretKey"]).Returns("validSecretKey");
            _packingAlgorithmMock.Setup(p => p.CallPackingAlgorithmService(request.Orders))
                .ReturnsAsync(packingResponses);

            var result = await _orderController.ProcessOrders(request) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            result.Value.Should().BeEquivalentTo(orderResponse);
        }

        [Fact]
        public async Task ProcessOrders_WithInvalidSecretKey_ReturnsUnauthorizedResult()
        {
            var request = GetOrderRequest("invalidSecretKey");

            _configMock.Setup(c => c["Security:ValidSecretKey"]).Returns("validSecretKey");

            var result = await _orderController.ProcessOrders(request) as UnauthorizedObjectResult;

            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
            Assert.Equal("Invalid SecretKey.", result.Value);
        }

        [Fact]
        public async Task ProcessOrders_WithNoOrders_ReturnsBadRequestResult()
        {
            var request = new OrderRequest
            {
                SecretKey = "validSecretKey",
                Orders = null
            };

            _configMock.Setup(c => c["Security:ValidSecretKey"]).Returns("validSecretKey");

            var result = await _orderController.ProcessOrders(request) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("No orders to process.", result.Value);
        }

        [Fact]
        public async Task ProcessOrders_WithEmptyOrders_ReturnsBadRequestResult()
        {
            var request = new OrderRequest
            {
                SecretKey = "validSecretKey",
                Orders = []
            };

            _configMock.Setup(c => c["Security:ValidSecretKey"]).Returns("validSecretKey");

            var result = await _orderController.ProcessOrders(request) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("No orders to process.", result.Value);
        }

        [Fact]
        public async Task ProcessOrders_WithException_ReturnsBadRequestResult()
        {
            var request = GetOrderRequest("validSecretKey");

            _configMock.Setup(c => c["Security:ValidSecretKey"]).Returns("validSecretKey");
            _packingAlgorithmMock.Setup(p => p.CallPackingAlgorithmService(request.Orders))
                .Throws(new Exception("An error occurred."));

            var result = await _orderController.ProcessOrders(request) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("An error occurred.", result.Value);
        }

        private OrderRequest GetOrderRequest(string secretKey)
        {
            return new OrderRequest
            {
                SecretKey = secretKey,
                Orders = [
                    new Order() {
                        OrderId = "1",
                        Products = [
                            new ("PS5", new Dimension(40, 10, 25)),
                            new ("Volante", new Dimension(40, 30, 30))
                        ]
                    },
                    new Order() {
                        OrderId = "2",
                        Products = [
                            new ("Joystick", new Dimension(40, 10, 25)),
                            new ("Fifa 24", new Dimension(10, 30, 10)),
                            new ("Call of Duty", new Dimension(30, 15, 10))
                        ]
                    }
                ]
            };
        }
    }
}