using SMAS_BusinessObject.DTOs.StaffDTO;
using SMAS_BusinessObject.DTOs.WorkShiftDTO;
using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;
using SMAS_Repositories.WorkStaffRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.StaffRepository
{
    public class WorkStaffRepository : IWorkStaffRepository
    {
        private readonly WorkStaffDAO _workStaffDAO;

        public WorkStaffRepository(WorkStaffDAO workStaffDAO)
        {
            _workStaffDAO = workStaffDAO;
        }

        public async Task<IEnumerable<StaffWorkingTodayDto>> GetStaffWorkingTodayAsync()
        {
            var workStaffs = await _workStaffDAO.GetStaffWorkingTodayAsync();

            return workStaffs.Select(ws => MapToDto(ws));
        }
        public async Task<IEnumerable<FilterStaffByPositionDto>> GetFilterStaffByPositionAsync(List<string> positions)
        {
            var workStaffs = await _workStaffDAO.GetFilterStaffByPositionAsync(positions);
            return workStaffs.Select(ws => MapToFilterDto(ws));
        }

        public async Task<WorkHistoryResponseDto?> GetAllWorkHistoryByStaffIdAsync(int staffId, int month, int year)
        {
            var (user, workStaffs) = await _workStaffDAO.GetAllWorkHistoryByStaffIdAsync(staffId, month, year);

            if (user == null) return null;

            var details = workStaffs.Select(ws => MapToDetailDto(ws)).ToList();

            return new WorkHistoryResponseDto
            {
                UserId = user.UserId,
                FullName = user.Fullname ?? string.Empty,
                AvatarUrl = user.Avatar,

                Position = user.Staff?.Position,

                TotalShifts = details.Count,
                TotalHours = details.Sum(d => d.DailyTime ?? 0),
                TotalAbsent = details.Count(d => d.Status == "Vắng"),

                Details = details
            };
        }
        public async Task<WorkNextSevenDayResponseDto> GetAllWorkNextSevenDayByPositionAsync(List<string> positions)
        {
            var workStaffs = await _workStaffDAO.GetAllWorkNextSevenDayByPositionAsync(positions);

            // Tạo danh sách 7 ngày tới
            var dateRange = Enumerable.Range(1, 7)
                .Select(i => DateOnly.FromDateTime(DateTime.Today.AddDays(i)))
                .ToList();

            var shiftRows = workStaffs
                .GroupBy(ws => ws.ShiftId)
                .Select(shiftGroup =>
                {
                    var shift = shiftGroup.First().Shift;

                    // Với mỗi ngày trong 7 ngày, lấy danh sách nhân viên làm ca này
                    var days = dateRange.Select(date => new ScheduleSlotDto
                    {
                        WorkDay = date,
                        Staffs = shiftGroup
                            .Where(ws => ws.WorkDay == date)
                            .Select(ws => new StaffSlotDto
                            {
                                UserId = ws.UserId,
                                FullName = ws.User?.Fullname ?? string.Empty,
                                AvatarUrl = ws.User?.Avatar,
                                Position = ws.User?.Staff?.Position
                            }).ToList()
                    }).ToList();

                    return new ShiftRowDto
                    {
                        ShiftId = shiftGroup.Key,
                        ShiftName = shift?.ShiftName,
                        StartTime = shift?.StartTime,
                        EndTime = shift?.EndTime,
                        Days = days
                    };
                })
                .OrderBy(s => s.StartTime)
                .ToList();

            return new WorkNextSevenDayResponseDto
            {
                DateRange = dateRange,
                Shifts = shiftRows
            };
        }

        public async Task<IEnumerable<WorkShiftDto>> GetAllWorkShiftAsync()
        {
            var shifts = await _workStaffDAO.GetAllWorkShiftAsync();
            return shifts.Select(s => MapToDto(s));
        }

        private static WorkShiftDto MapToDto(WorkShift s) =>
            new()
            {
                ShiftId = s.ShiftId,
                ShiftName = s.ShiftName,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                TypeStaff = s.TypeStaff,
                AdditionalWork = s.AdditionalWork
            };

        public async Task<bool> IsAlreadyAssignedAsync(int userId, int shiftId, DateOnly workDay)
        => await _workStaffDAO.IsAlreadyAssignedAsync(userId, shiftId, workDay);

        public async Task<CreateWorkStaffRequestDto> CreateWorkStaffAsync(CreateWorkStaffRequestDto dto)
        {
            var entity = new WorkStaff
            {
                UserId = dto.UserId,
                ShiftId = dto.ShiftId,
                WorkDay = dto.WorkDay,
                IsWorking = true,
                Note = dto.Note
            };

            var created = await _workStaffDAO.CreateWorkStaffAsync(entity);

            // Trả lại dto với dữ liệu đã tạo
            return new CreateWorkStaffRequestDto
            {
                UserId = created.UserId,
                ShiftId = created.ShiftId,
                WorkDay = created.WorkDay,
                Note = created.Note
            };
        }

        public async Task<(bool Success, string? ErrorMessage, UpdateWorkStaffRequestDto? Data)> UpdateWorkStaffAsync(int workStaffId, UpdateWorkStaffRequestDto dto)
        {
            var entity = await _workStaffDAO.GetByIdAsync(workStaffId);
            if (entity == null)
                return (false, "Không tìm thấy ca làm việc.", null);

            // Thay thế nhân viên → kiểm tra trùng ca
            if (dto.ReplaceUserId.HasValue && dto.ReplaceUserId.Value != entity.UserId)
            {
                var isDuplicate = await _workStaffDAO.IsAlreadyAssignedAsync(
                    dto.ReplaceUserId.Value, entity.ShiftId, entity.WorkDay, workStaffId);

                if (isDuplicate)
                    return (false, "Nhân viên thay thế đã được phân công ca này trong ngày.", null);

                entity.UserId = dto.ReplaceUserId.Value;
            }

            // Cập nhật giờ bắt đầu / kết thúc
            if (dto.CheckInTime.HasValue)
                entity.CheckInTime = dto.CheckInTime;

            if (dto.CheckOutTime.HasValue)
                entity.CheckOutTime = dto.CheckOutTime;

            // Tính tổng giờ làm nếu có cả 2 mốc thời gian
            if (entity.CheckInTime.HasValue && entity.CheckOutTime.HasValue)
            {
                var totalHours = (decimal)(entity.CheckOutTime.Value - entity.CheckInTime.Value).TotalHours;
                entity.DailyTime = Math.Round(totalHours, 2);
            }

            // Cập nhật trạng thái
            if (dto.IsWorking.HasValue)
                entity.IsWorking = dto.IsWorking;

            if (dto.Note != null)
                entity.Note = dto.Note;

            await _workStaffDAO.UpdateWorkStaffAsync(entity);

            return (true, null, new UpdateWorkStaffRequestDto
            {
                ReplaceUserId = entity.UserId,
                CheckInTime = entity.CheckInTime,
                CheckOutTime = entity.CheckOutTime,
                IsWorking = entity.IsWorking,
                Note = entity.Note
            });
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteWorkStaffAsync(int workStaffId)
        {
            var deleted = await _workStaffDAO.DeleteWorkStaffAsync(workStaffId);
            if (!deleted)
                return (false, "Không tìm thấy ca làm việc.");

            return (true, null);
        }

        private static StaffWorkingTodayDto MapToDto(WorkStaff ws)
        {
            return new StaffWorkingTodayDto
            {
                UserId = ws.UserId,
                FullName = ws.User?.Fullname ?? string.Empty,
                AvatarUrl = ws.User?.Avatar,
                Position = ws.User?.Staff?.Position,
                Note = ws.Note,
                CheckInTime = ws.CheckInTime
            };
        }

        private static FilterStaffByPositionDto MapToFilterDto(Staff s) =>
           new()
           {
               UserId = s.UserId,
               FullName = s.User?.Fullname ?? string.Empty,
               AvatarUrl = s.User?.Avatar,
               Position = s.Position
           };

        private static WorkHistoryDetailDto MapToDetailDto(WorkStaff ws)
        {
            var checkIn = ws.CheckInTime.HasValue ? TimeOnly.FromDateTime(ws.CheckInTime.Value) : (TimeOnly?)null;
            var checkOut = ws.CheckOutTime.HasValue ? TimeOnly.FromDateTime(ws.CheckOutTime.Value) : (TimeOnly?)null;

            return new WorkHistoryDetailDto
            {
                WorkDay = ws.WorkDay,
                ShiftName = ws.Shift?.ShiftName,
                CheckInTime = checkIn,
                CheckOutTime = checkOut,
                DailyTime = ws.DailyTime,
                Status = GetStatus(ws)
            };
        }

        // Tính trạng thái dựa vào IsWorking, CheckInTime và StartTime của ca
        private static string GetStatus(WorkStaff ws)
        {
            // Vắng: không làm việc hoặc không check-in
            if (ws.IsWorking == false || ws.CheckInTime == null)
                return "Vắng";

            // Muộn: check-in trễ hơn StartTime của ca
            if (ws.Shift?.StartTime != null)
            {
                var checkIn = TimeOnly.FromDateTime(ws.CheckInTime.Value);
                var startTime = ws.Shift.StartTime.Value;

                var lateMinutes = (int)(checkIn - startTime).TotalMinutes;
                if (lateMinutes > 0)
                    return $"Muộn ({lateMinutes}p)";
            }

            return "Đúng giờ";
        }

        public async  Task<int> GetSumWorkShiftThisMonthByJwtIdAsync(int userId)
        {
            return  await   _workStaffDAO.GetSumWorkShiftThisMonthByJwtIdAsync(userId);
        }
        public async  Task<double> GetSumTimeWorkedThisMonthByJwtIdAsync(int userId)
        {
            return await _workStaffDAO.GetSumTimeWorkedThisMonthByJwtIdAsync(userId);
        }

        public async Task<IEnumerable<ScheduleWorkResponseDTO>> GetScheduleWorkOnWeekbyStaffIdAsync(int staffId, DateOnly date)
        {
           return await _workStaffDAO.GetScheduleWorkOnWeekbyStaffIdAsync(staffId, date);
        }

        public async Task<IEnumerable<WorkStaffResponseDTO>> GetWorkScheduleNotCheckinByStaff(int userId)
        {
            var workStaffs = await _workStaffDAO.GetWorkScheduleNotCheckinByStaff(userId);
            return workStaffs.Select(ws => new WorkStaffResponseDTO
            {
                ShiftName = ws.Shift?.ShiftName,
                WorkDay = ws.WorkDay,
                StartTime = ws.Shift?.StartTime,
                EndTime = ws.Shift?.EndTime,
                WorkStaffId= ws.WorkStaffId
            });
        }
    }

}
