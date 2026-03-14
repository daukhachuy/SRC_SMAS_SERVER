using Microsoft.Extensions.Caching.Memory;
using SMAS_BusinessObject.Cache;
using SMAS_BusinessObject.DTOs.TableDTO;
using SMAS_DataAccess.DAO;
using SMAS_Services.TableService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.TableRepository
{
    public class TableSessionRepository : ITableSessionRepository
    {
        private readonly TableSessionDAO _dao;
        private readonly IMemoryCache _cache;
        private readonly ITableTokenHelper _tokenHelper; 

        private const int SESSION_TTL_HOURS = 12;
        private static string CacheKey(string tableCode) => $"table_session_{tableCode.ToUpper()}";

        public TableSessionRepository(TableSessionDAO dao, IMemoryCache cache, ITableTokenHelper tokenHelper) 
        {
            _dao = dao;
            _cache = cache;
            _tokenHelper = tokenHelper;
        }

        public async Task<(bool Success, string? ErrorCode, OpenTableResponseDto? Data)> OpenTableAsync(
            string tableCode, int openedBy)
        {
            var table = await _dao.GetTableByCodeAsync(tableCode);
            if (table == null)
                return (false, "TABLE_NOT_FOUND", null);

            if (_cache.TryGetValue(CacheKey(tableCode), out TableSessionCache? existing) && existing?.Status == "ACTIVE")
                return (false, "TABLE_ALREADY_ACTIVE", null);

            var now = DateTime.UtcNow;
            var session = new TableSessionCache
            {
                TableCode = tableCode.ToUpper(),
                TableId = table.TableId,
                SessionNonce = Guid.NewGuid().ToString("N"),
                Status = "ACTIVE",
                OpenedBy = openedBy,
                OpenedAt = now,
                ExpiresAt = now.AddHours(SESSION_TTL_HOURS)
            };

            _cache.Set(CacheKey(tableCode), session, session.ExpiresAt - now);
            await _dao.UpdateTableStatusAsync(table.TableId, "OPEN");

            return (true, null, new OpenTableResponseDto
            {
                TableCode = session.TableCode,
                Status = session.Status,
                OpenedAt = session.OpenedAt,
                ExpiresAt = session.ExpiresAt
            });
        }

        public async Task<(bool Success, string? ErrorCode, CloseTableResponseDto? Data)> CloseTableAsync(
            string tableCode, int closedBy)
        {
            var table = await _dao.GetTableByCodeAsync(tableCode);
            if (table == null)
                return (false, "TABLE_NOT_FOUND", null);

            if (!_cache.TryGetValue(CacheKey(tableCode), out TableSessionCache? session) || session?.Status != "ACTIVE")
                return (false, "SESSION_NOT_ACTIVE", null);

            _cache.Remove(CacheKey(tableCode));
            await _dao.UpdateTableStatusAsync(table.TableId, "AVAILABLE");

            return (true, null, new CloseTableResponseDto
            {
                TableCode = tableCode.ToUpper(),
                Status = "CLOSED",
                ClosedAt = DateTime.UtcNow
            });
        }

        public async Task<(bool Success, string? ErrorCode, TableInitResponseDto? Data)> InitSessionAsync(string tableCode)
        {
            var table = await _dao.GetTableByCodeAsync(tableCode);
            if (table == null)
                return (false, "TABLE_NOT_FOUND", null);

            if (!_cache.TryGetValue(CacheKey(tableCode), out TableSessionCache? session))
                return (false, "SESSION_NOT_ACTIVE", null);

            if (session!.Status != "ACTIVE")
                return (false, "SESSION_NOT_ACTIVE", null);

            if (DateTime.UtcNow > session.ExpiresAt)
            {
                _cache.Remove(CacheKey(tableCode));
                return (false, "SESSION_EXPIRED", null);
            }

            var accessToken = _tokenHelper.GenerateAccessToken(tableCode, session.SessionNonce);
            var refreshToken = _tokenHelper.GenerateRefreshToken(tableCode, session.SessionNonce);

            return (true, null, new TableInitResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TableCode = tableCode.ToUpper(),
                TableName = table.TableName,
                ExpiresInSeconds = _tokenHelper.AccessTokenExpiresInSeconds
            });
        }

        public async Task<(bool Success, string? ErrorCode, RefreshTokenResponseDto? Data)> RefreshAsync(string refreshToken)
        {
            var principal = _tokenHelper.ValidateToken(refreshToken);
            if (principal == null)
                return (false, "INVALID_QR_TOKEN", null);

            var tableCode = principal.FindFirst("tableCode")?.Value;
            var sessionNonce = principal.FindFirst("sessionNonce")?.Value;
            var tokenType = principal.FindFirst("tokenType")?.Value;

            if (tokenType != "table_refresh" || tableCode == null || sessionNonce == null)
                return (false, "INVALID_QR_TOKEN", null);

            if (!_cache.TryGetValue(CacheKey(tableCode), out TableSessionCache? session))
                return (false, "SESSION_NOT_ACTIVE", null);

            if (session!.Status != "ACTIVE" || session.SessionNonce != sessionNonce)
                return (false, "SESSION_NOT_ACTIVE", null);

            if (DateTime.UtcNow > session.ExpiresAt)
            {
                _cache.Remove(CacheKey(tableCode));
                return (false, "SESSION_EXPIRED", null);
            }

            var newAccessToken = _tokenHelper.GenerateAccessToken(tableCode, sessionNonce);

            return (true, null, new RefreshTokenResponseDto
            {
                AccessToken = newAccessToken,
                ExpiresInSeconds = _tokenHelper.AccessTokenExpiresInSeconds
            });
        }

        public async Task<ActiveSessionResponseDto?> GetActiveSessionAsync(string tableCode)
        {
            if (!_cache.TryGetValue(CacheKey(tableCode), out TableSessionCache? session))
                return null;

            if (session?.Status != "ACTIVE" || DateTime.UtcNow > session.ExpiresAt)
                return null;

            return new ActiveSessionResponseDto
            {
                TableCode = session.TableCode,
                Status = session.Status,
                OpenedAt = session.OpenedAt,
                ExpiresAt = session.ExpiresAt
            };
        }

        public (bool Valid, string? ErrorCode, string? TableCode) ValidateAccessToken(string accessToken)
        {
            var principal = _tokenHelper.ValidateToken(accessToken);
            if (principal == null)
                return (false, "INVALID_QR_TOKEN", null);

            var tableCode = principal.FindFirst("tableCode")?.Value;
            var sessionNonce = principal.FindFirst("sessionNonce")?.Value;
            var tokenType = principal.FindFirst("tokenType")?.Value;

            if (tokenType != "table_access" || tableCode == null || sessionNonce == null)
                return (false, "INVALID_QR_TOKEN", null);

            if (!_cache.TryGetValue(CacheKey(tableCode), out TableSessionCache? session))
                return (false, "TABLE_CLOSED", null);

            if (session!.Status != "ACTIVE")
                return (false, "TABLE_CLOSED", null);

            if (session.SessionNonce != sessionNonce)
                return (false, "SESSION_NOT_ACTIVE", null);

            if (DateTime.UtcNow > session.ExpiresAt)
            {
                _cache.Remove(CacheKey(tableCode));
                return (false, "SESSION_EXPIRED", null);
            }

            return (true, null, tableCode);
        }
    }
}
