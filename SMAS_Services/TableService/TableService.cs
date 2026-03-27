using SMAS_BusinessObject.DTOs.TableDTO;
using SMAS_Repositories.TableRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.TableService
{
    public class TableService : ITableService
    {
        private readonly ITableRepository _repo;

        public TableService(ITableRepository repo)
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

        //public async Task<List<TableResponseDTO>> GetAllTableAsync()
        //{
        //    return await _repo.GetAllTableAsync();
        //}
        public async Task<List<TableResponseDTO>> GetTablesAsync(string? tableType, string? status)
        {
            return await _repo.GetTablesAsync(tableType, status);
        }

        public async Task<TableResponseDTO> CreateTableAsync(CreateTableDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TableName))
                throw new ArgumentException("Tên bàn không được để trống.");
            if (dto.NumberOfPeople < 1)
                throw new ArgumentException("Số lượng khách phải lớn hơn 0.");

            return await _repo.CreateTableAsync(dto);
        }

        public async Task<TableResponseDTO?> UpdateTableAsync(int tableId, UpdateTableDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TableName))
                throw new ArgumentException("Tên bàn không được để trống.");
            if (dto.NumberOfPeople < 1)
                throw new ArgumentException("Số lượng khách phải lớn hơn 0.");

            return await _repo.UpdateTableAsync(tableId, dto);
        }

        public async Task<bool> DeleteTableAsync(int tableId)
        {
            return await _repo.DeleteTableAsync(tableId);
        }
    }   
}
