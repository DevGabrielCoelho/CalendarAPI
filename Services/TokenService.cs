using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CalendarAPI.Interfaces;
using CalendarAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace CalendarAPI.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            var signinKey = _configuration["JWT:SigninKey"] ?? throw new ArgumentNullException("JWT:SigninKey", "The signin key is missing in the configuration.");
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signinKey));
        }

        public string CreateToken(User user)
        {
            if (string.IsNullOrEmpty(user.Email))
                throw new InvalidOperationException("User's email is not initialized.");

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = creds,
                Issuer = _configuration["JWT:Issuer"],
                Audience = _configuration["JWT:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
