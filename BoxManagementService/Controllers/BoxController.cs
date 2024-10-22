using BoxManagementService.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BoxManagementService.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class BoxController(IConfiguration config, ILogger<BoxController> logger) : ControllerBase
    {
        private readonly IConfiguration _config = config;
        private readonly ILogger<BoxController> _logger = logger;

        [HttpGet("GetAvailableBoxes")]
        [ProducesResponseType(typeof(List<Box>), 200)]
        public IActionResult GetAvailableBoxes()
        {
            try
            {
                var secretKey = Request.Headers.Authorization.ToString();

                if (!IsAuthorized(secretKey))
                {
                    _logger.LogWarning("Unauthorized request with secretKey: {secretKey}", secretKey);
                    return Unauthorized("Invalid SecretKey.");
                }

                _logger.LogInformation("Available boxes returned successfully!");
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
        private static List<Box> FactoryBoxes() => new List<Box>
        {
            new Box { BoxId = "Caixa 1", Height = 30, Width = 40, Length = 80 },
            new Box { BoxId = "Caixa 2", Height = 80, Width = 50, Length = 40 },
            new Box { BoxId = "Caixa 3", Height = 80, Width = 80, Length = 60 }
        };

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
            var teste1 = Encoding.ASCII.GetBytes(key);

            var validSecretKey = Convert.ToBase64String(Encoding.ASCII.GetBytes(key));

            return secretKey == "Basic " + validSecretKey;
        }
    }
}
