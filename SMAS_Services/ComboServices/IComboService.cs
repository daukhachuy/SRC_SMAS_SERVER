using SMAS_BusinessObject.DTOs.Combo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.ComboServices
{
    public interface IComboService
    {
        Task<IEnumerable<ComboListResponse>> GetAvailableCombosAsync();

        Task<IEnumerable<ComboListResponse>> GetCombosFilterAsync(CombosFilterRequest request);

        Task<bool> UpdateStatusByComboId(int comboId);
        Task<IEnumerable<ComboListResponse>> GetAllAsync();
        Task<ComboListResponse?> GetByIdAsync(int id);
        Task<ComboListResponse> CreateAsync(ComboCreateDto dto, int? createdBy);
        Task<(ComboListResponse? Data, string? MsgCode, string? Message)> UpdateAsync(
       int id, ComboUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateStatusAsync(int id, bool isAvailable);
        public Task<(bool Success, string? MsgCode, string? Message)> AddFoodToComboAsync(
   int comboId, int foodId, int quantity);
        public Task<(bool Success, string? MsgCode, string? Message)> RemoveFoodFromComboAsync(
          int comboId, int foodId);
        public Task<(bool Success, string? MsgCode, string? Message)> UpdateFoodQuantityAsync(
           int comboId, int foodId, int quantity);
    }
}
