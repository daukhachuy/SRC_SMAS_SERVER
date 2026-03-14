using SMAS_BusinessObject.DTOs.TableDTO;
using SMAS_Repositories.TableRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.TableService
{
    public class TableSessionService : ITableSessionService
    {
        private readonly ITableSessionRepository _repo;

        public TableSessionService(ITableSessionRepository repo)
        {
            _repo = repo;
        }

        public async Task<(bool Success, string? ErrorCode, OpenTableResponseDto? Data)> OpenTableAsync(string tableCode, int openedBy)
            => await _repo.OpenTableAsync(tableCode, openedBy);

        public async Task<(bool Success, string? ErrorCode, CloseTableResponseDto? Data)> CloseTableAsync(string tableCode, int closedBy)
            => await _repo.CloseTableAsync(tableCode, closedBy);

        public async Task<(bool Success, string? ErrorCode, TableInitResponseDto? Data)> InitSessionAsync(string tableCode)
            => await _repo.InitSessionAsync(tableCode);

        public async Task<(bool Success, string? ErrorCode, RefreshTokenResponseDto? Data)> RefreshAsync(string refreshToken)
            => await _repo.RefreshAsync(refreshToken);

        public async Task<ActiveSessionResponseDto?> GetActiveSessionAsync(string tableCode)
            => await _repo.GetActiveSessionAsync(tableCode);

        public (bool Valid, string? ErrorCode, string? TableCode) ValidateAccessToken(string accessToken)
            => _repo.ValidateAccessToken(accessToken);
    }
}
