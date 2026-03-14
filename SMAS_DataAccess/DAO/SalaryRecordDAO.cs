using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SMAS_DataAccess.DAO
{
    public class SalaryRecordDAO
    {
        private readonly RestaurantDbContext _context;

        public SalaryRecordDAO(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SalaryRecord>> GetSalaryLastSixMonthsAsync(int userId)
        {
            var now = DateTime.Today;
            var fromMonth = now.Month - 5;
            var fromYear = now.Year;

            if (fromMonth <= 0)
            {
                fromMonth += 12;
                fromYear -= 1;
            }

            return await _context.SalaryRecords
                .Where(s => s.UserId == userId
                         && (s.Year > fromYear
                            || (s.Year == fromYear && s.Month >= fromMonth))
                         && (s.Year < now.Year
                            || (s.Year == now.Year && s.Month <= now.Month)))
                .OrderBy(s => s.Year)
                .ThenBy(s => s.Month)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<SalaryRecord?> GetCurrentMonthlySalaryDetailAsync(int userId)
        {
            var now = DateTime.Today;

            return await _context.SalaryRecords
                .Include(s => s.User)
                    .ThenInclude(u => u.Staff)
                .Where(s => s.UserId == userId
                         && s.Month == now.Month
                         && s.Year == now.Year)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}
