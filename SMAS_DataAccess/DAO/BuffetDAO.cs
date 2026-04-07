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
    

        public async Task<Buffet?> GetByIdAsync(int id)
        {
            return await _context.Buffets
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BuffetId == id);
        }

        public async Task<Buffet> CreateAsync(Buffet buffet)
        {
            _context.Buffets.Add(buffet);
            await _context.SaveChangesAsync();
            return buffet;
        }

        public async Task<Buffet> UpdateAsync(Buffet buffet)
        {
            _context.Buffets.Update(buffet);
            await _context.SaveChangesAsync();
            return buffet;
        }

       
        public async Task<bool> DeleteAsync(int id)
        {
            var buffet = await _context.Buffets.FindAsync(id);
            if (buffet == null) return false;

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
    }
}
