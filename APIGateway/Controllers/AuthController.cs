using APIGateway.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace APIGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IConfiguration config) : ControllerBase
    {
        private readonly IConfiguration _config = config;

        /// <summary>
        /// Autentica o usuário e retorna um token JWT.
        /// Obs.: Username = "admin" e Password = "password"
        /// </summary>
        /// <param name="userLogin">Dados de login do usuário</param>
        /// <returns>Token JWT</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(IActionResult), 200)]
        [ProducesResponseType(401)]
        public IActionResult Login([FromBody] UserLogin userLogin)
        {
            // TODO: Replace this with a real authentication
            if (userLogin.Username != "admin" || userLogin.Password != "password")
            {
                return Unauthorized();
            }

            var tokenString = GenerateJWT();
            return Ok(new { token = tokenString });
        }

        private string GenerateJWT()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Audience"],
              null,
              expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireDays"])),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
