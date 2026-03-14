using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SMAS_Services.TableService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SMAS_API.Helpers
{
    public class TableTokenHelper : ITableTokenHelper
    {
        private readonly IConfiguration _config;
        private const int ACCESS_TOKEN_MINUTES = 30;
        private const int REFRESH_TOKEN_HOURS = 12;

        public TableTokenHelper(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateAccessToken(string tableCode, string sessionNonce)
            => GenerateToken(tableCode, sessionNonce, ACCESS_TOKEN_MINUTES * 60, "table_access");

        public string GenerateRefreshToken(string tableCode, string sessionNonce)
            => GenerateToken(tableCode, sessionNonce, REFRESH_TOKEN_HOURS * 3600, "table_refresh");

        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]!));
                var handler = new JwtSecurityTokenHandler();

                return handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out _);
            }
            catch
            {
                return null;
            }
        }

        public int AccessTokenExpiresInSeconds => ACCESS_TOKEN_MINUTES * 60;

        private string GenerateToken(string tableCode, string sessionNonce, int expireSeconds, string tokenType)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim("tableCode",    tableCode),
            new Claim("sessionNonce", sessionNonce),
            new Claim("tokenType",    tokenType)
        };

            var jwt = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddSeconds(expireSeconds),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }

}
