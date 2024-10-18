using APIGateway.Controllers;
using APIGateway.Infra.Interfaces;
using APIGateway.Models;
using APIGateway.Models.Request;
using Castle.Core.Configuration;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace APIGateway.Test
{
    public class OrderControllerTests
    {
        private readonly Mock<IConfiguration> _configMock;
        private Mock<IOrdersService> _ordersServiceMock;
        private OrderController _orderController;

        public OrderControllerTests()
        {
            _configMock = new Mock<IConfiguration>();
            _ordersServiceMock = new Mock<IOrdersService>();
            _orderController = new OrderController(_ordersServiceMock.Object);
        }

        [Fact]
        public async Task ProcessOrders_WithValidRequest_ReturnsOkResult()
        {
            var request = GetOrderRequest();

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

            _ordersServiceMock.Setup(x => x.CallOrderProcessingService(request.Orders))
                .ReturnsAsync(orderResponse);

            var result = await _orderController.ProcessOrders(request);

            result.Should().BeOfType<OkObjectResult>();
            var okObjectResult = result as OkObjectResult;
            Assert.Equal(200, okObjectResult.StatusCode);
            okObjectResult.Value.Should().BeEquivalentTo(orderResponse);
        }

        [Fact]
        public async Task ProcessOrders_WithEmptyRequest_ReturnsBadRequest()
        {
            var request = new OrderRequest
            {
                Orders = []
            };

            var result = await _orderController.ProcessOrders(request);

            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("No orders to process.", badRequestResult.Value);
        }

        [Fact]
        public async Task ProcessOrders_WithException_ReturnsBadRequest()
        {
            var request = GetOrderRequest();

            var errorMessage = "An error occurred while processing orders.";
            _ordersServiceMock.Setup(x => x.CallOrderProcessingService(request.Orders))
                .ThrowsAsync(new Exception(errorMessage));

            var result = await _orderController.ProcessOrders(request);

            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal(errorMessage, badRequestResult.Value);
        }

        private OrderRequest GetOrderRequest()
        {
            return new OrderRequest
            {
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

