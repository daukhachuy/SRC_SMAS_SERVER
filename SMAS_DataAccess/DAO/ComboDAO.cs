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


        public async Task<IEnumerable<Combo>> GetAllAsync()
        {
            return await _context.Combos
                .AsNoTracking()
                 .Include(c => c.ComboFoods)
            .ThenInclude(cf => cf.Food)
                .ToListAsync();
        }

        public async Task<Combo?> GetByIdAsync(int id)
        {
            return await _context.Combos
                .AsNoTracking()
                 .Include(c => c.ComboFoods)
            .ThenInclude(cf => cf.Food)
                .FirstOrDefaultAsync(c => c.ComboId == id);
        }
        public async Task<Combo?> GetByIdTrackedAsync(int id)
        {
            return await _context.Combos
                .Include(c => c.ComboFoods)
                    .ThenInclude(cf => cf.Food)
                .FirstOrDefaultAsync(c => c.ComboId == id);
        }

        public async Task<Combo> CreateAsync(Combo combo)
        {
            _context.Combos.Add(combo);
            await _context.SaveChangesAsync();

            // Load Food cho từng ComboFood 
            foreach (var cf in combo.ComboFoods)
                await _context.Entry(cf).Reference(x => x.Food).LoadAsync();

            return combo;
        }

        public async Task<Combo> UpdateAsync(Combo combo, List<ComboFood> newFoods)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var oldFoods = combo.ComboFoods.ToList();
                var oldDict = oldFoods.ToDictionary(cf => cf.FoodId);
                var newDict = newFoods.ToDictionary(cf => cf.FoodId);

                var toRemove = oldFoods.Where(cf => !newDict.ContainsKey(cf.FoodId)).ToList();
                if (toRemove.Any())
                    _context.ComboFoods.RemoveRange(toRemove);

                foreach (var oldCf in oldFoods)
                {
                    if (newDict.TryGetValue(oldCf.FoodId, out var newCf)
                        && oldCf.Quantity != newCf.Quantity)
                    {
                        oldCf.Quantity = newCf.Quantity;
                    }
                }

                var toAdd = newFoods
                    .Where(cf => !oldDict.ContainsKey(cf.FoodId))
                    .Select(cf => new ComboFood
                    {
                        ComboId = combo.ComboId,
                        FoodId = cf.FoodId,
                        Quantity = cf.Quantity
                    })
                    .ToList();
                if (toAdd.Any())
                    await _context.ComboFoods.AddRangeAsync(toAdd);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                await _context.Entry(combo).Collection(c => c.ComboFoods).LoadAsync();
                foreach (var cf in combo.ComboFoods)
                    await _context.Entry(cf).Reference(x => x.Food).LoadAsync();

                return combo;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var combo = await _context.Combos
                .Include(c => c.ComboFoods)
                .FirstOrDefaultAsync(c => c.ComboId == id);

            if (combo == null) return false;

            // Xóa các ComboFood con trước
            _context.ComboFoods.RemoveRange(combo.ComboFoods);

            // Sau đó xóa Combo
            _context.Combos.Remove(combo);

            await _context.SaveChangesAsync();
            return true;
        }

        // Patch status: chỉ cập nhật IsAvailable
        public async Task<bool> UpdateStatusAsync(int id, bool isAvailable)
        {
            var combo = await _context.Combos.FindAsync(id);
            if (combo == null) return false;

            combo.IsAvailable = isAvailable;
            combo.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<Dictionary<int, bool?>> GetFoodAvailabilityAsync(IEnumerable<int> foodIds)
        {
            var ids = foodIds.Distinct().ToList();
            return await _context.Foods
                .AsNoTracking()
                .Where(f => ids.Contains(f.FoodId))
                .ToDictionaryAsync(f => f.FoodId, f => f.IsAvailable);
        }
        public async Task<List<Combo>> GetAvailableCombosWithFoodsAsync()
        {
            return await _context.Combos
                .AsNoTracking()
                .Include(c => c.ComboFoods)
                    .ThenInclude(cf => cf.Food)   
                .Where(c =>
                    //(c.IsAvailable == true || c.IsAvailable == null) &&
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

        public async Task<bool> UpdateStatusByComboId(int comboId)
        {
            var combo = await _context.Combos.FindAsync(comboId);

            if (combo == null)
                return false;
            combo.IsAvailable = !combo.IsAvailable;
            await _context.SaveChangesAsync();
            return true;
        }

        // Kiểm tra ComboFood đã tồn tại chưa
        public async Task<ComboFood?> GetComboFoodAsync(int comboId, int foodId)
        {
            return await _context.ComboFoods
                .FirstOrDefaultAsync(cf => cf.ComboId == comboId && cf.FoodId == foodId);
        }

        // Thêm 1 món vào combo
        public async Task<bool> AddFoodToComboAsync(int comboId, int foodId, int quantity)
        {
            var comboFood = new ComboFood
            {
                ComboId = comboId,
                FoodId = foodId,
                Quantity = quantity
            };
            _context.ComboFoods.Add(comboFood);

            // Cập nhật UpdatedAt của combo
            var combo = await _context.Combos.FindAsync(comboId);
            if (combo != null) combo.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        // Xóa 1 món khỏi combo
        public async Task<bool> RemoveFoodFromComboAsync(int comboId, int foodId)
        {
            var comboFood = await _context.ComboFoods
                .FirstOrDefaultAsync(cf => cf.ComboId == comboId && cf.FoodId == foodId);
            if (comboFood == null) return false;

            _context.ComboFoods.Remove(comboFood);

            var combo = await _context.Combos.FindAsync(comboId);
            if (combo != null) combo.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        // Cập nhật quantity của 1 món trong combo
        public async Task<bool> UpdateFoodQuantityAsync(int comboId, int foodId, int quantity)
        {
            var comboFood = await _context.ComboFoods
                .FirstOrDefaultAsync(cf => cf.ComboId == comboId && cf.FoodId == foodId);
            if (comboFood == null) return false;

            comboFood.Quantity = quantity;

            var combo = await _context.Combos.FindAsync(comboId);
            if (combo != null) combo.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        // Kiểm tra combo có tồn tại
        public async Task<bool> ComboExistsAsync(int comboId)
        {
            return await _context.Combos.AnyAsync(c => c.ComboId == comboId);
        }

        // Đếm số món còn lại trong combo (để chặn xóa món cuối cùng)
        public async Task<int> CountFoodsInComboAsync(int comboId)
        {
            return await _context.ComboFoods.CountAsync(cf => cf.ComboId == comboId);
        }
    }
}
