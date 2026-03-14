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


        public async Task<Buffet?> GetBuffetWithFoodsAsync(int buffetId)
        {
            return await _context.Buffets
                .Include(b => b.BuffetFoods)
                    .ThenInclude(bf => bf.Food)
                .FirstOrDefaultAsync(b => b.BuffetId == buffetId);
        }

        public async Task<List<Food>> FilterFoodsAsync(List<int>? categoryIds, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Foods
                                .Include(f => f.Categories)
                                .Where(f => f.IsAvailable == true)
                                .AsQueryable();

            if (categoryIds != null && categoryIds.Any())
            {
                query = query.Where(f =>
                    f.Categories.Any(c => categoryIds.Contains(c.CategoryId)));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(f =>
                    (f.PromotionalPrice ?? f.Price) >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(f =>
                    (f.PromotionalPrice ?? f.Price) <= maxPrice.Value);
            }

            return await query.OrderByDescending(f => f.CreatedAt).ToListAsync();
        }


        public async Task<decimal> GetFoodPriceAsync(int foodId)
        {
            return await _context.Foods
                .Where(f => f.FoodId == foodId)
                .Select(f => f.PromotionalPrice ?? f.Price)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateStatusByFoodId(int foodId)
        {
            var food = await _context.Foods.FindAsync(foodId);

            if (food == null)
                return false;
            food.IsAvailable = !food.IsAvailable;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
