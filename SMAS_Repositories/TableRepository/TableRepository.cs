using Microsoft.Extensions.Caching.Memory;
using SMAS_BusinessObject.Cache;
using SMAS_BusinessObject.DTOs.TableDTO;
using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;
using SMAS_Services.TableService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.TableRepository
{
    public class TableRepository : ITableRepository
    {
        private readonly TableDAO _dao;
        private readonly IMemoryCache _cache;
        private readonly ITableTokenHelper _tokenHelper; 

        private const int SESSION_TTL_HOURS = 12;
        private static string CacheKey(int tableId) => $"table_session_{tableId}";
        public TableRepository(TableDAO dao, IMemoryCache cache, ITableTokenHelper tokenHelper) 
        {
            _dao = dao;
            _cache = cache;
            _tokenHelper = tokenHelper;
        }

        public async Task<(bool Success, string? ErrorCode, OpenTableResponseDto? Data)> OpenTableAsync(string tableCode, int openedBy)
        {
            if (!int.TryParse(tableCode, out int tableId))
                return (false, "TABLE_NOT_FOUND", null);

            var table = await _dao.GetTableByCodeAsync(tableCode); // đã hỗ trợ TableId
            if (table == null)
                return (false, "TABLE_NOT_FOUND", null);

            var cacheKey = CacheKey(tableId);

            // Nếu đã có session ACTIVE thì trả về
            if (_cache.TryGetValue(cacheKey, out TableSessionCache? existing) && existing?.Status == "ACTIVE")
            {
                return (true, null, new OpenTableResponseDto
                {
                    TableCode = tableId.ToString(),
                    Status = existing.Status,
                    OpenedAt = existing.OpenedAt,
                    ExpiresAt = existing.ExpiresAt
                });
            }

            var now = DateTime.UtcNow;
            var session = new TableSessionCache
            {
                TableCode = tableId.ToString(),
                TableId = tableId,
                SessionNonce = Guid.NewGuid().ToString("N"),
                Status = "ACTIVE",
                OpenedBy = openedBy,
                OpenedAt = now,
                ExpiresAt = now.AddHours(SESSION_TTL_HOURS)
            };

            _cache.Set(cacheKey, session, session.ExpiresAt - now);
            await _dao.UpdateTableStatusAsync(tableId, "OPEN");

            return (true, null, new OpenTableResponseDto
            {
                TableCode = tableId.ToString(),
                Status = "ACTIVE",
                OpenedAt = session.OpenedAt,
                ExpiresAt = session.ExpiresAt
            });
        }
        public async Task<(bool Success, string? ErrorCode, CloseTableResponseDto? Data)> CloseTableAsync(string tableCode, int closedBy)
        {
            if (!int.TryParse(tableCode, out int tableId))
                return (false, "TABLE_NOT_FOUND", null);

            var table = await _dao.GetTableByCodeAsync(tableCode);
            if (table == null)
                return (false, "TABLE_NOT_FOUND", null);

            var cacheKey = CacheKey(tableId);

            if (!_cache.TryGetValue(cacheKey, out TableSessionCache? session) || session?.Status != "ACTIVE")
                return (false, "SESSION_NOT_ACTIVE", null);

            _cache.Remove(cacheKey);
            await _dao.UpdateTableStatusAsync(tableId, "AVAILABLE");

            return (true, null, new CloseTableResponseDto
            {
                TableCode = tableId.ToString(),
                Status = "CLOSED",
                ClosedAt = DateTime.UtcNow
            });
        }

        public async Task<(bool Success, string? ErrorCode, TableInitResponseDto? Data)> InitSessionAsync(string tableCode)
        {
            // Chuyển tableCode thành TableId
            if (!int.TryParse(tableCode, out int tableId))
                return (false, "TABLE_NOT_FOUND", null);

            var table = await _dao.GetTableByCodeAsync(tableCode);
            if (table == null)
                return (false, "TABLE_NOT_FOUND", null);

            var cacheKey = $"table_session_{tableId}";

            // Nếu chưa có session nhưng bàn đang OPEN → tự động tạo (rất hữu ích cho bàn cũ)
            if (!_cache.TryGetValue(cacheKey, out TableSessionCache? session) ||
                session?.Status != "ACTIVE" ||
                DateTime.UtcNow > session.ExpiresAt)
            {
                if (table.Status != "OPEN")
                    return (false, "SESSION_NOT_ACTIVE", null);

                // Tự động tạo session mới
                session = new TableSessionCache
                {
                    TableCode = tableId.ToString(),
                    TableId = tableId,
                    SessionNonce = Guid.NewGuid().ToString("N"),
                    Status = "ACTIVE",
                    OpenedBy = 0,                    // Có thể lấy từ Order sau nếu cần
                    OpenedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddHours(12)
                };

                _cache.Set(cacheKey, session, session.ExpiresAt - DateTime.UtcNow);

                Console.WriteLine($"[AutoCreateSession] Đã tự tạo session cho bàn {tableId}");
            }

            var accessToken = _tokenHelper.GenerateAccessToken(tableId.ToString(), session!.SessionNonce);
            var refreshToken = _tokenHelper.GenerateRefreshToken(tableId.ToString(), session.SessionNonce);
            var orderCode = await _dao.GetActiveOrderCodeByTableIdAsync(tableId);

            return (true, null, new TableInitResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TableCode = tableId.ToString(),
                TableName = table.TableName,
                ExpiresInSeconds = _tokenHelper.AccessTokenExpiresInSeconds,
                OrderCode = orderCode  // null nếu chưa có đơn
            });
        }

        public async Task<(bool Success, string? ErrorCode, RefreshTokenResponseDto? Data)> RefreshAsync(string refreshToken)
        {
            var principal = _tokenHelper.ValidateToken(refreshToken);
            if (principal == null)
                return (false, "INVALID_QR_TOKEN", null);

            var tableCodeStr = principal.FindFirst("tableCode")?.Value;
            var sessionNonce = principal.FindFirst("sessionNonce")?.Value;
            var tokenType = principal.FindFirst("tokenType")?.Value;

            if (tokenType != "table_refresh" || !int.TryParse(tableCodeStr, out int tableId) || sessionNonce == null)
                return (false, "INVALID_QR_TOKEN", null);

            var cacheKey = CacheKey(tableId);

            if (!_cache.TryGetValue(cacheKey, out TableSessionCache? session))
                return (false, "SESSION_NOT_ACTIVE", null);

            if (session!.Status != "ACTIVE" || session.SessionNonce != sessionNonce)
                return (false, "SESSION_NOT_ACTIVE", null);

            if (DateTime.UtcNow > session.ExpiresAt)
            {
                _cache.Remove(cacheKey);
                return (false, "SESSION_EXPIRED", null);
            }

            var newAccessToken = _tokenHelper.GenerateAccessToken(tableId.ToString(), sessionNonce);

            return (true, null, new RefreshTokenResponseDto
            {
                AccessToken = newAccessToken,
                ExpiresInSeconds = _tokenHelper.AccessTokenExpiresInSeconds
            });
        }

        public async Task<ActiveSessionResponseDto?> GetActiveSessionAsync(string tableCode)
        {
            if (!int.TryParse(tableCode, out int tableId))
                return null;

            var cacheKey = CacheKey(tableId);
            if (!_cache.TryGetValue(cacheKey, out TableSessionCache? session))
                return null;

            if (session?.Status != "ACTIVE" || DateTime.UtcNow > session.ExpiresAt)
                return null;

            return new ActiveSessionResponseDto
            {
                TableCode = tableId.ToString(),
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

            var tableCodeStr = principal.FindFirst("tableCode")?.Value;
            var sessionNonce = principal.FindFirst("sessionNonce")?.Value;
            var tokenType = principal.FindFirst("tokenType")?.Value;

            if (tokenType != "table_access" || !int.TryParse(tableCodeStr, out int tableId) || sessionNonce == null)
                return (false, "INVALID_QR_TOKEN", null);

            var cacheKey = CacheKey(tableId);

            if (!_cache.TryGetValue(cacheKey, out TableSessionCache? session))
                return (false, "TABLE_CLOSED", null);

            if (session!.Status != "ACTIVE")
                return (false, "TABLE_CLOSED", null);

            if (session.SessionNonce != sessionNonce)
                return (false, "SESSION_NOT_ACTIVE", null);

            if (DateTime.UtcNow > session.ExpiresAt)
            {
                _cache.Remove(cacheKey);
                return (false, "SESSION_EXPIRED", null);
            }

            return (true, null, tableId.ToString());
        }
        //public async Task<List<TableResponseDTO>> GetAllTableAsync()
        //{
        //    var tables = await _dao.GetAllTableAsync();

        //    return tables.Select(t => new TableResponseDTO
        //    {
        //        TableId = t.TableId,
        //        TableName = t.TableName,
        //        TableType = t.TableType,
        //        NumberOfPeople = t.NumberOfPeople,
        //        Status = t.Status,
        //        IsActive = t.IsActive,
        //        QrCode = t.QrCode
        //    }).ToList();
        //}

        public async Task<List<TableResponseDTO>> GetTablesAsync(string? tableType, string? status, bool? isActive)
     => await _dao.GetTablesAsync(tableType, status, isActive);
        public async Task<TableResponseDTO> CreateTableAsync(CreateTableDto dto)
        {
            var table = new Table
            {
                TableName = dto.TableName,
                TableType = dto.TableType,
                NumberOfPeople = dto.NumberOfPeople,
                Status = "AVAILABLE",
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            var created = await _dao.CreateTableAsync(table);
            return new TableResponseDTO
            {
                TableId = created.TableId,
                TableName = created.TableName,
                TableType = created.TableType,
                NumberOfPeople = created.NumberOfPeople,
                Status = created.Status,
                IsActive = created.IsActive,
                QrCode = created.QrCode,
                CurrentGuests = 0,
                CurrentAmount = 0
            };
        }

        public async Task<TableResponseDTO?> UpdateTableAsync(int tableId, UpdateTableDto dto)
        {
            var table = await _dao.GetTableByIdAsync(tableId);
            if (table == null) return null;

            table.TableName = dto.TableName;
            table.TableType = dto.TableType;
            table.NumberOfPeople = dto.NumberOfPeople;
            table.UpdatedAt = DateTime.Now;

            await _dao.UpdateTableAsync(table);

            return new TableResponseDTO
            {
                TableId = table.TableId,
                TableName = table.TableName,
                TableType = table.TableType,
                NumberOfPeople = table.NumberOfPeople,
                Status = table.Status,
                IsActive = table.IsActive,
                QrCode = table.QrCode,
                CurrentGuests = 0,
                CurrentAmount = 0
            };
        }

        public async Task<bool> DeleteTableAsync(int tableId)
        {
            var isOccupied = await _dao.IsTableOccupiedAsync(tableId);
            if (isOccupied)
                throw new InvalidOperationException("Không thể xóa bàn đang có khách.");

            return await _dao.SoftDeleteTableAsync(tableId);
        }
        public async Task<TableResponseDTO?> ToggleTableActiveAsync(int tableId, bool isActive)
        {
            // Không cho tắt bàn đang có khách
            if (!isActive)
            {
                var table = await _dao.GetTableByIdRawAsync(tableId);  // tìm kể cả IsActive=false
                if (table == null) return null;

                if (table.Status == "OPEN")
                    throw new InvalidOperationException("Không thể vô hiệu hóa bàn đang có khách.");
            }

            return await _dao.ToggleTableActiveAsync(tableId, isActive);
        }
    }
}
