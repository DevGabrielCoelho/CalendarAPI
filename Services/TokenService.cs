using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using CalendarAPI.Interfaces;
using CalendarAPI.Models;

namespace CalendarAPI.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            var signinKey = _configuration["JWT:SigninKey"];
            if (string.IsNullOrEmpty(signinKey))
                throw new ArgumentNullException("JWT:SigninKey", "The signin key is missing in the configuration.");
            
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signinKey));
        }
        public string CreateToken(User user)
        {
            if (user.Email == null)
                throw new InvalidOperationException("User's email is not initialized.");
            
            var claims = new List<Claim>{
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
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