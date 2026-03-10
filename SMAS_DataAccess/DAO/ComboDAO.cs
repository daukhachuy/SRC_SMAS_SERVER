using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SMAS_DataAccess.DAO
{
    public class ComboDAO
    {
        private readonly RestaurantDbContext _context;

        public ComboDAO(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<List<Combo>> GetAvailableCombosWithFoodsAsync()
        {
            return await _context.Combos
                .AsNoTracking()
                .Include(c => c.ComboFoods)
                    .ThenInclude(cf => cf.Food)   
                .Where(c =>
                    (c.IsAvailable == true || c.IsAvailable == null) &&
                    (c.ExpiryDate == null || c.ExpiryDate >= DateOnly.FromDateTime(DateTime.UtcNow))
                )
                .OrderByDescending(c => c.CreatedAt)   
                .ToListAsync();
        }

        public async Task<decimal> GetComboPriceAsync(int comboId)
        {
            return await _context.Combos
                .Where(c => c.ComboId == comboId && c.IsAvailable == true)
                .Select(c => c.Price)
                .FirstOrDefaultAsync();
        }
    }
}
