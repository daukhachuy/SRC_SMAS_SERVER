using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<bool> CreateStaffAsync(Staff request)
        {
            var user = _context.Users.Where(s => s.UserId == request.UserId && s.Role == "Customer").FirstOrDefault();
            if (user == null) return false;
            try
            {            
                user.Role = "Staff";
                _context.Staff.Add(request);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> CreateStaffWithUserAsync(User user, Staff staff)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                staff.UserId = user.UserId;
                await _context.Staff.AddAsync(staff);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}
