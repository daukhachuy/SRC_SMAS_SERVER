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
        // Lấy tất cả Food còn hoạt động (IsAvailable != false)
        public async Task<IEnumerable<Food>> GetAllAsync()
        {
            return await _context.Foods
                .Where(f => f.IsAvailable != false)
                .AsNoTracking()
                .ToListAsync();
        }

        // Lấy Food theo Id
        public async Task<Food?> GetByIdAsync(int id)
        {
            return await _context.Foods
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.FoodId == id);
        }

        // Thêm mới Food
        public async Task<Food> CreateAsync(Food food)
        {
            _context.Foods.Add(food);
            await _context.SaveChangesAsync();
            return food;
        }

        // Cập nhật Food
        public async Task<Food> UpdateAsync(Food food)
        {
            _context.Foods.Update(food);
            await _context.SaveChangesAsync();
            return food;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var food = await _context.Foods.FindAsync(id);
            if (food == null) return false;

            _context.Foods.Remove(food);
            await _context.SaveChangesAsync();
            return true;
        }

        // Patch status: chỉ cập nhật IsAvailable
        public async Task<bool> UpdateStatusAsync(int id, bool isAvailable)
        {
            var food = await _context.Foods.FindAsync(id);
            if (food == null) return false;

            food.IsAvailable = isAvailable;
            food.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
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

        public async Task<Food?> GetFoodByIdAsync(int foodId)
        {
            return await _context.Foods
                .Include(f => f.Categories)
                .FirstOrDefaultAsync(f => f.FoodId == foodId && (f.IsAvailable == true || f.IsAvailable == null));
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
