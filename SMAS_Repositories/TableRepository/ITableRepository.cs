using SMAS_BusinessObject.DTOs.TableDTO;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.TableRepository
{
    public interface ITableRepository
    {
        Task<(bool Success, string? ErrorCode, OpenTableResponseDto? Data)> OpenTableAsync(string tableCode, int openedBy);
        Task<(bool Success, string? ErrorCode, CloseTableResponseDto? Data)> CloseTableAsync(string tableCode, int closedBy);
        Task<(bool Success, string? ErrorCode, TableInitResponseDto? Data)> InitSessionAsync(string tableCode);
        Task<(bool Success, string? ErrorCode, RefreshTokenResponseDto? Data)> RefreshAsync(string refreshToken);
        Task<ActiveSessionResponseDto?> GetActiveSessionAsync(string tableCode);

        // Guard dùng ở create order
        (bool Valid, string? ErrorCode, string? TableCode) ValidateAccessToken(string accessToken);

        //Task<List<TableResponseDTO>> GetAllTableAsync();
        Task<List<TableResponseDTO>> GetTablesAsync(string? tableType, string? status, bool? isActive);
        Task<TableResponseDTO> CreateTableAsync(CreateTableDto dto);
        Task<TableResponseDTO?> UpdateTableAsync(int tableId, UpdateTableDto dto);
        Task<bool> DeleteTableAsync(int tableId);
        Task<TableResponseDTO?> ToggleTableActiveAsync(int tableId, bool isActive);
    }
}
