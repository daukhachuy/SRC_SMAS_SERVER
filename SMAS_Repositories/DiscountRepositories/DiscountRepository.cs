using SMAS_BusinessObject.DTOs.DiscountDTO;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.DiscountRepositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly DiscountDao _context;

        public DiscountRepository(DiscountDao context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DiscountResponse>> GetAllDiscountsAsync()
        {
            var discounts = await _context.GetAllDiscountsAsync();
            return discounts.Select(d => new DiscountResponse
            {
                DiscountId = d.DiscountId,
                Code = d.Code,
                Description = d.Description,
                DiscountType = d.DiscountType,
                Value = d.Value,
                MinOrderAmount = d.MinOrderAmount,
                MaxDiscountAmount = d.MaxDiscountAmount,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                UsageLimit = d.UsageLimit,
                UsedCount = d.UsedCount,
                ApplicableFor = d.ApplicableFor,
                Status = d.Status,
                CreatedAt = d.CreatedAt
            });
        }
    }
}
