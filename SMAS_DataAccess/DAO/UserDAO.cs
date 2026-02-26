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


    }
}
