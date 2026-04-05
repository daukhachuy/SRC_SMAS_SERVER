using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_DataAccess.DAO
{
    public class DiscountDao
    {

        private readonly RestaurantDbContext _context;

        public DiscountDao(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<List<Discount>> GetAllDiscountsAsync()
        {
            return await _context.Discounts.ToListAsync();
        }

        public async Task<Discount?> GetDiscountByIdAsync(string Code)
        {
            //DateTime now = DateTime.Now;
            //return await _context.Discounts.FirstOrDefaultAsync(d => d.Code == Code && d.Status == "Active"
            //                     && d.StartDate.ToDateTime(TimeOnly.MinValue) <= now
            //                     && d.EndDate.ToDateTime(TimeOnly.MaxValue) >= now);

            return await _context.Discounts.AsNoTracking().FirstOrDefaultAsync(d => d.Code == Code && d.Status == "Active");
        }


        //Hoang lam
        public async Task<IEnumerable<Discount>> GetAllAsync()
        {
            return await _context.Discounts
                .Include(d => d.CreatedByNavigation)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Discount?> GetByIdAsync(int id)
        {
            return await _context.Discounts
                .Include(d => d.CreatedByNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DiscountId == id);
        }

        public async Task<Discount?> GetByCodeAsync(string code)
        {
            return await _context.Discounts
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Code == code);
        }

        public async Task<Discount> CreateAsync(Discount discount)
        {
            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync();
            return discount;
        }

        public async Task<Discount> UpdateAsync(Discount discount)
        {
            _context.Discounts.Update(discount);
            await _context.SaveChangesAsync();
            return discount;
        }

        public async Task UpdateStatusAsync(int discount)
        {
            var existing = await _context.Discounts.FindAsync(discount);
            if (existing != null) return;
            if (existing.Status == "Expired") return; // Không đổi trạng thái nếu đã hết hạn
            existing.Status = existing.Status == "Active" ? "Inactive" : "Active";
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsCodeAsync(string code, int? excludeId = null)
        {
            return await _context.Discounts.AnyAsync(d =>
                d.Code == code && (excludeId == null || d.DiscountId != excludeId));
        }
    }
}
