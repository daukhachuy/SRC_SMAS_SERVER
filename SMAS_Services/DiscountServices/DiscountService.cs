using SMAS_BusinessObject.DTOs.DiscountDTO;
using SMAS_Repositories.DiscountRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.DiscountServices
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository _discountRepository;

        public DiscountService(IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
        }

        public async Task<IEnumerable<DiscountResponse>> GetAllDiscountsAsync(string? filterStatus = "Active")
        {
            var discounts = await _discountRepository.GetAllDiscountsAsync();

            if (string.IsNullOrEmpty(filterStatus))
                return discounts.ToList();

            return discounts.Where(d => d.Status == filterStatus).ToList();
        }
        public async Task<DiscountResponse?> GetDiscountByCodeAsync(string Code)
        {
            return await _discountRepository.GetDiscountByCodeAsync(Code);
        }
        public async Task<DiscountResponse> GetByIdAsync(int id)
        {
            return await _discountRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Discount with id {id} not found.");
        }

        public async Task<DiscountResponse> CreateAsync(DiscountCreateDto dto)
        {
            if (dto.EndDate < dto.StartDate)
                throw new ArgumentException("EndDate must be greater than or equal to StartDate.");

            // Check trùng code
            var existingCode = await _discountRepository.ExistsCodeAsync(dto.Code.Trim().ToUpper());
            if (existingCode)
                throw new InvalidOperationException($"Mã giảm giá '{dto.Code}' đã tồn tại.");

            return await _discountRepository.CreateAsync(dto);
        }

        public async Task<DiscountResponse> UpdateAsync(int id, DiscountUpdateDto dto)
        {
            if (dto.EndDate < dto.StartDate)
                throw new ArgumentException("EndDate must be greater than or equal to StartDate.");

            return await _discountRepository.UpdateAsync(id, dto);
        }
        public Task<bool> DeleteAsync(int id) => _discountRepository.DeleteAsync(id);
        public Task<bool> UpdateStatusAsync(int id, string status) => _discountRepository.UpdateStatusAsync(id, status);
    }
}
