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

        public async Task<bool> ExistsAsync(int month, int year)
        {
            return await _context.SalaryRecords
                .AnyAsync(s => s.Month == month && s.Year == year);
        }

        public async Task CreateBatchAsync(List<SalaryRecord> records)
        {
            _context.SalaryRecords.AddRange(records);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Staff>> GetAllActiveStaffAsync()
        {
            return await _context.Staff
                .Include(s => s.User)
                .Where(s => s.IsWorking == true && s.User.IsActive == true && s.User.IsDeleted != true)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<WorkStaff>> GetWorkStaffByMonthAsync(int userId, int month, int year)
        {
            return await _context.WorkStaffs
                .Include(ws => ws.Shift)
                .Where(ws => ws.UserId == userId
                          && ws.WorkDay.Month == month
                          && ws.WorkDay.Year == year)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<SalaryRecord>> GetAllByMonthAsync(int month, int year)
        {
            return await _context.SalaryRecords
                .Include(s => s.User)
                    .ThenInclude(u => u.Staff)
                .Where(s => s.Month == month && s.Year == year)
                .OrderBy(s => s.User.Fullname)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<SalaryRecord?> GetByUserAndMonthAsync(int userId, int month, int year)
        {
            return await _context.SalaryRecords
                .Include(s => s.User)
                    .ThenInclude(u => u.Staff)
                .Where(s => s.UserId == userId && s.Month == month && s.Year == year)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<SalaryRecord?> GetByIdAsync(int salaryRecordId)
        {
            return await _context.SalaryRecords
                .FirstOrDefaultAsync(s => s.SalaryRecordId == salaryRecordId);
        }

        public async Task UpdateAsync(SalaryRecord record)
        {
            _context.SalaryRecords.Update(record);
            await _context.SaveChangesAsync();
        }
    }
}
