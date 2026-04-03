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

        public async Task<IEnumerable<DiscountResponse>> GetAllDiscountsAsync()
        {
           var discounts = await _discountRepository.GetAllDiscountsAsync();

            return discounts.Where(d => d.Status == "Active").ToList();
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
            if (await _discountRepository.ExistsCodeAsync(dto.Code))
                throw new InvalidOperationException($"Discount code '{dto.Code}' already exists.");

            if (dto.EndDate < dto.StartDate)
                throw new ArgumentException("EndDate must be greater than or equal to StartDate.");

            return await _discountRepository.CreateAsync(dto);
        }

        public async Task<DiscountResponse> UpdateAsync(int id, DiscountUpdateDto dto)
        {
            if (dto.EndDate < dto.StartDate)
                throw new ArgumentException("EndDate must be greater than or equal to StartDate.");

            return await _discountRepository.UpdateAsync(id, dto);
        }

        public async Task DeleteAsync(int id)
            => await _discountRepository.DeleteAsync(id);

        /// <summary>Validate mã giảm giá và tăng UsedCount nếu hợp lệ.</summary>
        public async Task<DiscountResponse?> ValidateAndApplyAsync(DiscountValidateDto dto)
        {
            var discount = await _discountRepository.GetDiscountByCodeAsync(dto.Code);
            if (discount is null) return null;

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (today < discount.StartDate || today > discount.EndDate) return null;
            if (discount.Status != "Active") return null;
            if (discount.UsageLimit.HasValue && discount.UsedCount >= discount.UsageLimit) return null;
            if (discount.MinOrderAmount.HasValue && dto.OrderAmount < discount.MinOrderAmount) return null;

            // Tăng UsedCount thông qua UpdateDto
            var updateDto = new DiscountUpdateDto
            {
                Description = discount.Description,
                DiscountType = discount.DiscountType,
                Value = discount.Value,
                MinOrderAmount = discount.MinOrderAmount,
                MaxDiscountAmount = discount.MaxDiscountAmount,
                StartDate = discount.StartDate,
                EndDate = discount.EndDate,
                UsageLimit = discount.UsageLimit,
                ApplicableFor = discount.ApplicableFor,
                Status = discount.Status,
                UsedCount = discount.UsedCount + 1
            };

            return await _discountRepository.UpdateAsync(discount.DiscountId, updateDto);
        }
    }
}
