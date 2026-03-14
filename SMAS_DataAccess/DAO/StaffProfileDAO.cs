using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SMAS_DataAccess.DAO
{
    public class StaffProfileDAO
    {
        private readonly RestaurantDbContext _context;

        public StaffProfileDAO(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetProfileAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Staff)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId && u.IsDeleted != true);
        }

        public async Task<User?> GetForUpdateAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Staff)
                .FirstOrDefaultAsync(u => u.UserId == userId && u.IsDeleted != true);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
