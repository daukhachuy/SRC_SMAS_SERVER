using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_DataAccess.DAO
{
    public class AuthDAO
    {
        private readonly RestaurantDbContext _context;

        public  AuthDAO(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetActiveUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == email && (u.IsDeleted == false || u.IsDeleted == null));
        }

        public async Task UpdatePasswordAsync(int userId, string passwordHash)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return;
            user.PasswordHash = passwordHash;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
