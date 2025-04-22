using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TaskManagement.Authentication;

namespace TaskManagement.Controllers
{
   

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly TokenGenerator _tokenGenerator;

        private readonly IConfiguration _configuration;
        public AuthController(TokenGenerator tokenGenerator, IConfiguration configuration)
        {
            _tokenGenerator = tokenGenerator;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest model) 
        {
           
            if (!ValidateUser(model.Email, model.Password))
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

           //TODO replace with actual data
            var claims = new[]
            {
             new Claim(ClaimTypes.NameIdentifier, "1"),
             new Claim(ClaimTypes.Email, model.Email),
             new Claim(ClaimTypes.Name, "John Doe"), 
             new Claim(ClaimTypes.Role, "User"),    
             new Claim("isAdmin", "false"), 
        };

           
            var token = _tokenGenerator.GenerateToken(claims);

          
            return Ok(new { token });
        }
        [HttpGet("test")]
        [Authorize] 
        public ActionResult<string> GetTest()
        {
            return Ok("Authenticated!");
        }

        private bool ValidateUser(string email, string password)
        {
            
            return email == "test@example.com" && password == "password";
        }
    }

  
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
