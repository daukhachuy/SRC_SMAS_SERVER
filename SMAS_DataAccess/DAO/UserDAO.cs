using SMAS_BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_DataAccess.DAO
{
    public class UserDAO
    {
        private readonly RestaurantDbContext _context;

        public UserDAO(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId
                    && (u.IsDeleted == false || u.IsDeleted == null));
        }

        public async Task UpdateProfileAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }


        public async Task<List<User>> GetAllAcountCustomerAsync()
        {
            return await _context.Users.Where(r => r.Role == "Customer").ToListAsync();
        }

        public async Task<List<User>> GetAllAcountStaffAsync()
        {
            return await _context.Users
                                 .Where(r => r.Role == "Staff")
                                 .Include(u => u.Staff)
                                 .ToListAsync();
        }
        public async Task<bool>  UpdateStatusUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;
            user.IsDeleted = !(user.IsDeleted ?? false);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
