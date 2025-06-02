using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Library.UserServices.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Library.UserServices.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _cfg;
        private readonly byte[] _key;

        public JwtTokenService(IConfiguration cfg)
        {
            _cfg = cfg;
            _key = Encoding.UTF8.GetBytes(_cfg["JwtSettings:Key"]!);
        }

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("username", user.Username)
            };

            var creds = new SigningCredentials(
                new SymmetricSecurityKey(_key),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _cfg["JwtSettings:Issuer"],
                audience: _cfg["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                           Convert.ToDouble(_cfg["JwtSettings:ExpiryMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
