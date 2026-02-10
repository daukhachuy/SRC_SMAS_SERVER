using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.Models;

namespace SMAS_DataAccess.DAO
{
    public class StaffDAO
    {
        private readonly RestaurantDbContext _context;

        public StaffDAO(RestaurantDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy staff theo UserId (khóa chính của Staff), kèm thông tin User.
        /// </summary>
        /// <param name="userId">UserId (id của staff)</param>
        /// <returns>Staff với User đã include, hoặc null nếu không tồn tại.</returns>
        public async Task<Staff?> GetStaffByIdAsync(int userId)
        {
            return await _context.Staff
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }
    }
}
