using BoxManagementService.Models;
using Microsoft.AspNetCore.Mvc;

namespace BoxManagementService.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class BoxController(IConfiguration config, ILogger<BoxController> logger) : ControllerBase
    {
        private readonly IConfiguration _config = config;
        private readonly ILogger<BoxController> _logger = logger;

        [HttpGet(Name = "GetAvailableBoxes")]
        [ProducesResponseType(typeof(List<Box>), 200)]
        public IActionResult GetAvailableBoxes(string secretKey)
        {
            try
            {
                if (!IsAuthorized(secretKey))
                {
                    _logger.LogWarning("Unauthorized request with secretKey: {secretKey}", secretKey);
                    return Unauthorized("Invalid SecretKey.");
                }

                _logger.LogInformation("Avaiable boxes returned successfully!");
                return Ok(FactoryBoxes());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        //TODO: Necessary to move to a service
        // and implement a factory pattern to create the boxes
        // and a repository to store the boxes
        private static List<Box> FactoryBoxes() => [
                new() { BoxId = "Caixa 1", Height = 30, Width = 40, Length = 80 },
                new() { BoxId = "Caixa 2", Height = 80, Width = 50, Length = 40 },
                new() { BoxId = "Caixa 2", Height = 50, Width = 80, Length = 60 }
            ];

        //TODO: NEcessary to move to a authentication service
        // and implement a token based authentication
        private bool IsAuthorized(string secretKey)
        {
            var validSecretKey = _config["Security:ValidSecretKey"];

            return secretKey == validSecretKey;
        }
    }
}
