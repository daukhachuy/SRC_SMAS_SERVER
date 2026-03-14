using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.TableService
{
    public interface ITableTokenHelper
    {
        string GenerateAccessToken(string tableCode, string sessionNonce);
        string GenerateRefreshToken(string tableCode, string sessionNonce);
        System.Security.Claims.ClaimsPrincipal? ValidateToken(string token);
        int AccessTokenExpiresInSeconds { get; }
    }
}
