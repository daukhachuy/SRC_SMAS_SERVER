using SMAS_BusinessObject.DTOs.Combo;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.ComboRepositories
{
    public interface IComboRepository
    {
        Task<IEnumerable<ComboListResponse>> GetAllAsync();
        Task<ComboListResponse?> GetByIdAsync(int id);
        Task<ComboListResponse> CreateAsync(ComboCreateDto dto);
        Task<ComboListResponse?> UpdateAsync(int id, ComboUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateStatusAsync(int id, bool isAvailable);
        Task<IEnumerable<ComboListResponse>> GetAvailableComboListAsync();

        Task<bool> UpdateStatusByComboId(int comboId);
    }
}
