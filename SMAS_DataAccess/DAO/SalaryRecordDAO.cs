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
            var fromDate = new DateTime(now.Year, now.Month, 1).AddMonths(-5);
            var fromYear = fromDate.Year;
            var fromMonth = fromDate.Month;

            return await _context.SalaryRecords
                .Where(s => s.UserId == userId
                         && (s.Year * 100 + s.Month) >= (fromYear * 100 + fromMonth)
                         && (s.Year * 100 + s.Month) <= (now.Year * 100 + now.Month))
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
