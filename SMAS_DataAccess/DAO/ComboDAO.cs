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
                .ToListAsync();
        }

        public async Task<Combo?> GetByIdAsync(int id)
        {
            return await _context.Combos
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ComboId == id);
        }

        public async Task<Combo> CreateAsync(Combo combo)
        {
            _context.Combos.Add(combo);
            await _context.SaveChangesAsync();
            return combo;
        }

        public async Task<Combo> UpdateAsync(Combo combo)
        {
            _context.Combos.Update(combo);
            await _context.SaveChangesAsync();
            return combo;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var combo = await _context.Combos.FindAsync(id);
            if (combo == null) return false;

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

        public async Task<bool> UpdateStatusByComboId(int comboId)
        {
            var combo = await _context.Combos.FindAsync(comboId);

            if (combo == null)
                return false;
            combo.IsAvailable = !combo.IsAvailable;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
