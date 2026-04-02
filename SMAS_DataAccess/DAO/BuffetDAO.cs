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

    }
}
