using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TaskManagement.Authentication
{
    

    public class TokenGenerator
    {
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _key;
        private readonly int _durationInMinutes;

        public TokenGenerator(string issuer, string audience, string key, int durationInMinutes)
        {
            _issuer = issuer;
            _audience = audience;
            _key = key;
            _durationInMinutes = durationInMinutes;
        }

        public string GenerateToken(IEnumerable<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_durationInMinutes),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

}
