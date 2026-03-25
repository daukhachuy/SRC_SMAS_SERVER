using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.DTOs.WorkShiftDTO;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_DataAccess.DAO
{
    public class WorkStaffDAO
    {
        private readonly RestaurantDbContext _context;

        public WorkStaffDAO(RestaurantDbContext context)
        {
            _context = context;
        }

        /// Query trực tiếp từ DB: lấy tất cả WorkStaff có WorkDay = hôm nay và IsWorking = true.
        /// Include đầy đủ navigation properties để Repository có thể mapping
        public async Task<IEnumerable<WorkStaff>> GetStaffWorkingTodayAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            return await _context.WorkStaffs
                .Include(ws => ws.User)
                    .ThenInclude(u => u.Staff)   // cần: Position
                .Where(ws => ws.WorkDay == today && ws.IsWorking == true)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Staff>> GetFilterStaffByPositionAsync(List<string> positions)
        {
            var query = _context.Staff
                .Include(s => s.User)
                .AsQueryable();

            if (positions != null && positions.Any())
                query = query.Where(s => positions.Contains(s.Position));

            return await query
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<(User? user, IEnumerable<WorkStaff> workStaffs)> GetAllWorkHistoryByStaffIdAsync(int staffId, int month, int year)
        {
            var user = await _context.Users
                .Include(u => u.Staff)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == staffId);

            var workStaffs = await _context.WorkStaffs
                .Include(ws => ws.Shift)
                .Where(ws => ws.UserId == staffId
                          && ws.WorkDay.Month == month
                          && ws.WorkDay.Year == year)
                .OrderByDescending(ws => ws.WorkDay)
                .AsNoTracking()
                .ToListAsync();

            return (user, workStaffs);
        }

        public async Task<IEnumerable<WorkStaff>> GetAllWorkNextSevenDayByPositionAsync(List<string> positions)
        {
            var from = DateOnly.FromDateTime(DateTime.Today);
            var to = DateOnly.FromDateTime(DateTime.Today.AddDays(6));

            var query = _context.WorkStaffs
                .Include(ws => ws.Shift)
                .Include(ws => ws.User)
                    .ThenInclude(u => u.Staff)
                .Where(ws => ws.WorkDay >= from && ws.WorkDay <= to)
                .AsQueryable();

            if (positions != null && positions.Any())
                query = query.Where(ws => ws.User.Staff != null
                                       && positions.Contains(ws.User.Staff.Position));

            return await query
                .OrderBy(ws => ws.ShiftId)
                .ThenBy(ws => ws.WorkDay)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<WorkShift>> GetAllWorkShiftAsync()
        {
            return await _context.WorkShifts
                .Where(s => s.IsActive == true
                         && s.TypeStaff != null
                         && (s.TypeStaff.Contains("Waiter") || s.TypeStaff.Contains("Kitchen")))
                .OrderBy(s => s.StartTime)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<WorkStaff?> GetByIdAsync(int workStaffId)
        {
            return await _context.WorkStaffs
                .Include(ws => ws.User)
                    .ThenInclude(u => u.Staff)
                .Include(ws => ws.Shift)
                .FirstOrDefaultAsync(ws => ws.WorkStaffId == workStaffId);
        }

        public async Task<WorkStaff?> UpdateWorkStaffAsync(WorkStaff workStaff)
        {
            _context.WorkStaffs.Update(workStaff);
            await _context.SaveChangesAsync();
            return workStaff;
        }

        public async Task<bool> DeleteWorkStaffAsync(int workStaffId)
        {
            var entity = await _context.WorkStaffs.FindAsync(workStaffId);
            if (entity == null) return false;

            _context.WorkStaffs.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsAlreadyAssignedAsync(int userId, int shiftId, DateOnly workDay, int excludeWorkStaffId = 0)
        {
            return await _context.WorkStaffs
                .AnyAsync(ws => ws.UserId == userId
                             && ws.ShiftId == shiftId
                             && ws.WorkDay == workDay
                             && (excludeWorkStaffId == 0 || ws.WorkStaffId != excludeWorkStaffId));
        }


        public async Task<WorkStaff> CreateWorkStaffAsync(WorkStaff workStaff)
        {
            _context.WorkStaffs.Add(workStaff);
            await _context.SaveChangesAsync();
            return workStaff;
        }

        public async Task<int> GetSumWorkShiftThisMonthByJwtIdAsync(int userId)
        {
            var now = DateTime.Now;

            var count = await _context.WorkStaffs
                .Where(w => w.UserId == userId
                         && w.WorkDay.Month == now.Month
                         && w.WorkDay.Year == now.Year)
                .CountAsync();

            return count;
        }

        public async Task<double> GetSumTimeWorkedThisMonthByJwtIdAsync(int userId)
        {
            var now = DateTime.Now;

            var shifts = await _context.WorkStaffs
                .Where(w => w.UserId == userId
                         && w.WorkDay.Month == now.Month
                         && w.WorkDay.Year == now.Year)
                .Include(w => w.Shift)
                .ToListAsync();

            double totalHours = 0;

            foreach (var shift in shifts)
            {
                if (shift.Shift.StartTime != null && shift.Shift.EndTime != null && shift.IsWorking == true)
                {
                    var start = shift.Shift.StartTime.Value.ToTimeSpan();
                    var end = shift.Shift.EndTime.Value.ToTimeSpan();

                    totalHours += (end - start).TotalHours;
                }
            }

            return totalHours;
        }
        public async Task<IEnumerable<ScheduleWorkResponseDTO>> GetScheduleWorkOnWeekbyStaffIdAsync(int staffId, DateOnly date)
        {
            var endDate = date.AddDays(7);

            var schedules = await _context.WorkStaffs
                .Where(w => w.UserId == staffId
                    && w.WorkDay >= date
                    && w.WorkDay <= endDate)
                .Include(w => w.Shift)
                .Select(w => new ScheduleWorkResponseDTO
                {
                    WorkDate = w.WorkDay,
                    ShiftName = w.Shift.ShiftName,
                    StartTime = w.Shift.StartTime,
                    EndTime = w.Shift.EndTime,
                    AdditionalWork = w.Shift.AdditionalWork,
                    Note = w.Note
                })
                .OrderBy(w => w.WorkDate)
                .ToListAsync();

            return schedules;
        }

        public async Task<List<WorkStaff>> GetWorkScheduleNotCheckinByStaff(int userId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
    
                return await _context.WorkStaffs
                    .Where(w => w.UserId == userId && w.WorkDay > today && w.IsWorking == false)
                    .Include(w => w.Shift)
                    .ToListAsync();
        }

    }
}
