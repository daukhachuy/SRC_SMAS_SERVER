using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_DataAccess.DAO
{
    public class BuffetDAO
    {
        private readonly RestaurantDbContext _context;

        public BuffetDAO(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<List<Buffet>> GetAllBuffetsAsync()
        {
            return await _context.Buffets
                //.Where(b => b.IsAvailable == true)
                .OrderByDescending(b => b.CreatedAt)
                 .Include(b => b.BuffetFoods)
            .ThenInclude(bf => bf.Food)
                .ToListAsync();
        }

        public async Task<bool> UpdateStatusByBuffetId(int buffetId)
        {
            var buffet = await _context.Buffets.FindAsync(buffetId);

            if (buffet == null)
                return false;
            buffet.IsAvailable = !buffet.IsAvailable;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Buffet?> GetByIdTrackedAsync(int id)
        {
            return await _context.Buffets
                .Include(b => b.BuffetFoods)
                    .ThenInclude(bf => bf.Food)
                .FirstOrDefaultAsync(b => b.BuffetId == id);
        }

        public async Task<Dictionary<int, bool?>> GetFoodAvailabilityAsync(IEnumerable<int> foodIds)
        {
            var ids = foodIds.Distinct().ToList();
            return await _context.Foods
                .AsNoTracking()
                .Where(f => ids.Contains(f.FoodId))
                .ToDictionaryAsync(f => f.FoodId, f => f.IsAvailable);
        }
        public async Task<Buffet?> GetByIdAsync(int id)
        {
            return await _context.Buffets
                .AsNoTracking()
                 .Include(b => b.BuffetFoods)
            .ThenInclude(bf => bf.Food)
                .FirstOrDefaultAsync(b => b.BuffetId == id);
        }

        public async Task<Buffet> CreateAsync(Buffet buffet)
        {
            _context.Buffets.Add(buffet);
            await _context.SaveChangesAsync();

            // Reload Food navigation cho response
            foreach (var bf in buffet.BuffetFoods)
                await _context.Entry(bf).Reference(x => x.Food).LoadAsync();

            return buffet;
        }
        public async Task<Buffet> UpdateAsync(Buffet buffet, List<BuffetFood> newFoods)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var oldFoods = buffet.BuffetFoods.ToList();
                var oldDict = oldFoods.ToDictionary(bf => bf.FoodId);
                var newDict = newFoods.ToDictionary(bf => bf.FoodId);

                // REMOVE
                var toRemove = oldFoods.Where(bf => !newDict.ContainsKey(bf.FoodId)).ToList();
                if (toRemove.Any())
                    _context.BuffetFoods.RemoveRange(toRemove);

                // UPDATE Quantity + IsUnlimited
                foreach (var oldBf in oldFoods)
                {
                    if (newDict.TryGetValue(oldBf.FoodId, out var newBf))
                    {
                        if (oldBf.Quantity != newBf.Quantity)
                            oldBf.Quantity = newBf.Quantity;
                        if (oldBf.IsUnlimited != newBf.IsUnlimited)
                            oldBf.IsUnlimited = newBf.IsUnlimited;
                    }
                }

                // ADD
                var toAdd = newFoods
                    .Where(bf => !oldDict.ContainsKey(bf.FoodId))
                    .Select(bf => new BuffetFood
                    {
                        BuffetId = buffet.BuffetId,
                        FoodId = bf.FoodId,
                        Quantity = bf.Quantity,
                        IsUnlimited = bf.IsUnlimited
                    })
                    .ToList();
                if (toAdd.Any())
                    await _context.BuffetFoods.AddRangeAsync(toAdd);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                await _context.Entry(buffet).Collection(b => b.BuffetFoods).LoadAsync();
                foreach (var bf in buffet.BuffetFoods)
                    await _context.Entry(bf).Reference(x => x.Food).LoadAsync();

                return buffet;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var buffet = await _context.Buffets
                .Include(b => b.BuffetFoods)
                .FirstOrDefaultAsync(b => b.BuffetId == id);
            if (buffet == null) return false;

            _context.BuffetFoods.RemoveRange(buffet.BuffetFoods);
            _context.Buffets.Remove(buffet);
            await _context.SaveChangesAsync();
            return true;
        }

        // Patch status: chỉ cập nhật IsAvailable
        public async Task<bool> UpdateStatusAsync(int id, bool isAvailable)
        {
            var buffet = await _context.Buffets.FindAsync(id);
            if (buffet == null) return false;

            buffet.IsAvailable = isAvailable;
            buffet.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        // ⭐ MỚI: Add/Remove/Update từng món
        public async Task<BuffetFood?> GetBuffetFoodAsync(int buffetId, int foodId)
        {
            return await _context.BuffetFoods
                .FirstOrDefaultAsync(bf => bf.BuffetId == buffetId && bf.FoodId == foodId);
        }

        public async Task<bool> BuffetExistsAsync(int buffetId)
        {
            return await _context.Buffets.AnyAsync(b => b.BuffetId == buffetId);
        }

        public async Task<int> CountFoodsInBuffetAsync(int buffetId)
        {
            return await _context.BuffetFoods.CountAsync(bf => bf.BuffetId == buffetId);
        }

        public async Task AddFoodToBuffetAsync(int buffetId, int foodId, int? quantity, bool? isUnlimited)
        {
            _context.BuffetFoods.Add(new BuffetFood
            {
                BuffetId = buffetId,
                FoodId = foodId,
                Quantity = quantity,
                IsUnlimited = isUnlimited
            });

            var buffet = await _context.Buffets.FindAsync(buffetId);
            if (buffet != null) buffet.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveFoodFromBuffetAsync(int buffetId, int foodId)
        {
            var bf = await GetBuffetFoodAsync(buffetId, foodId);
            if (bf == null) return false;

            _context.BuffetFoods.Remove(bf);

            var buffet = await _context.Buffets.FindAsync(buffetId);
            if (buffet != null) buffet.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
