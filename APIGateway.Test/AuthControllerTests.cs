using APIGateway.Controllers;
using APIGateway.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace APIGateway.Test
{
    public class AuthControllerTests
    {
        private readonly Mock<IConfiguration> _configMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _configMock = new Mock<IConfiguration>();
            _configMock.Setup(config => config["Jwt:Key"]).Returns("YourVeryLongAndSecureKeyHere1234567890");
            _configMock.Setup(config => config["Jwt:Issuer"]).Returns("APIGateway");
            _configMock.Setup(config => config["Jwt:Audience"]).Returns("APIGateway");
            _configMock.Setup(config => config["Jwt:ExpireMinutes"]).Returns("60");

            _controller = new AuthController(_configMock.Object);
        }

        [Fact]
        public void GenerateJWT_ReturnsToken()
        {
            var userLogin = new UserLogin
            {
                Username = "admin",
                Password = "password"
            };

            var result = _controller.Login(userLogin) as OkObjectResult;
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            var token = result.Value.GetType().GetProperty("token").GetValue(result.Value, null);
            Assert.NotNull(token);
        }

        [Fact]
        public void GenerateJWT_ReturnsUnauthorized()
        {
            var userLogin = new UserLogin
            {
                Username = "admin",
                Password = "wrongpassword"
            };

            var result = _controller.Login(userLogin) as UnauthorizedResult;
            Assert.NotNull(result);
            Assert.IsType<UnauthorizedResult>(result);
        }
    }

}