using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SMAS_BusinessObject.DTOs.Auth;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace SMAS_Services.AuthServices
{
    public class TokenService
    {
        private readonly JwtSettings _jwtSettings;

        public TokenService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateToken(User user)
        {
            var role = user.Role ?? "Customer";
            if (role == "Staff") role = user.Staff.Position;
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Fullname),          
            new Claim(ClaimTypes.Role,role),
            new Claim("avatar", user.Avatar ?? "")
        };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpiryInHours),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
