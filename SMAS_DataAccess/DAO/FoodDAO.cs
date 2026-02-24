using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_DataAccess.DAO
{
    public class FoodDAO
    {
        private readonly RestaurantDbContext _context;

        public FoodDAO(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<List<Food>> GetAllFoodsCategoryAsync()
        {
            return await _context.Foods.Include(c => c.Categories).ToListAsync();
        }

        public async Task<List<Food>> GetTopBestSellersAsync(int topN)
        {
            return await _context.Foods
                .AsNoTracking()
                .Where(f =>
                    (f.IsAvailable == true || f.IsAvailable == null)    
                )
                .OrderByDescending(f => f.OrderCount ?? 0)
                .ThenByDescending(f => f.IsFeatured ?? false)   
                .ThenBy(f => f.Name)                            
                .Take(topN)
                .ToListAsync();
        }
    }
}
