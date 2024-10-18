namespace BoxManagement.Testusing BoxManagementService.Controllers;
using BoxManagementService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace BoxManagement.Test
{
    public class BoxControllerTests
    {
        private readonly BoxController _boxController;
        private readonly Mock<IConfiguration> _configMock;
        private readonly Mock<ILogger<BoxController>> _loggerMock;

        public BoxControllerTests()
        {
            _configMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<BoxController>>();
            _boxController = new BoxController(_configMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void GetAvailableBoxes_WithValidSecretKey_ReturnsOkResult()
        {
            // Arrange
            string secretKey = "validSecretKey";
            _configMock.Setup(config => config["Security:ValidSecretKey"]).Returns(secretKey);

            // Act
            var result = _boxController.GetAvailableBoxes(secretKey);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetAvailableBoxes_WithInvalidSecretKey_ReturnsUnauthorizedResult()
        {
            // Arrange
            string secretKey = "invalidSecretKey";
            _configMock.Setup(config => config["Security:ValidSecretKey"]).Returns("validSecretKey");

            // Act
            var result = _boxController.GetAvailableBoxes(secretKey);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public void GetAvailableBoxes_WithException_ReturnsBadRequestResult()
        {
            // Arrange
            string secretKey = "validSecretKey";
            _configMock.Setup(config => config["Security:ValidSecretKey"]).Returns(secretKey);
            _loggerMock.Setup(logger => logger.LogError(It.IsAny<Exception>(), It.IsAny<string>()));

            // Act
            var result = _boxController.GetAvailableBoxes(secretKey);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void FactoryBoxes_ReturnsListOfBoxes()
        {
            // Act
            var result = _boxController.FactoryBoxes();

            // Assert
            Assert.IsType<List<Box>>(result);
        }

        [Fact]
        public void IsAuthorized_WithValidSecretKey_ReturnsTrue()
        {
            // Arrange
            string secretKey = "validSecretKey";
            _configMock.Setup(config => config["Security:ValidSecretKey"]).Returns(secretKey);

            // Act
            var result = _boxController.IsAuthorized(secretKey);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsAuthorized_WithInvalidSecretKey_ReturnsFalse()
        {
            // Arrange
            string secretKey = "invalidSecretKey";
            _configMock.Setup(config => config["Security:ValidSecretKey"]).Returns("validSecretKey");

            // Act
            var result = _boxController.IsAuthorized(secretKey);

            // Assert
            Assert.False(result);
        }
    }
}
