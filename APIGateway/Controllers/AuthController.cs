using APIGateway.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

            var tokenString = GenerateJWT(userLogin.Username);
            return Ok(new { token = tokenString });
        }

        //TODO: Move this method to a service class
        private string GenerateJWT(string username)
        {
            var keyBytes = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);

            var securityKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.Aes128CbcHmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, username)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireDays"])),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
