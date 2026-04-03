using SMAS_BusinessObject.DTOs.DiscountDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.DiscountServices
{
    public interface IDiscountService
    {
        Task<IEnumerable<DiscountResponse>> GetAllDiscountsAsync();

        Task<DiscountResponse?> GetDiscountByCodeAsync(string code);
        Task<DiscountResponse> GetByIdAsync(int id);
        Task<DiscountResponse> CreateAsync(DiscountCreateDto dto);
        Task<DiscountResponse> UpdateAsync(int id, DiscountUpdateDto dto);
        Task DeleteAsync(int id);
        Task<DiscountResponse?> ValidateAndApplyAsync(DiscountValidateDto dto);
    }
}
