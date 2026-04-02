using SMAS_BusinessObject.DTOs.DiscountDTO;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.DiscountRepositories
{
    public interface IDiscountRepository
    {
        Task<IEnumerable<DiscountResponse>> GetAllDiscountsAsync();

        Task<DiscountResponse?> GetDiscountByCodeAsync(string code);

        //Hoang lam 
        Task<DiscountResponse?> GetByIdAsync(int id);
        Task<DiscountResponse> CreateAsync(DiscountCreateDto dto);
        Task<DiscountResponse> UpdateAsync(int id, DiscountUpdateDto dto);
        Task DeleteAsync(int id);
        Task<bool> ExistsCodeAsync(string code, int? excludeId = null);
    }
}
