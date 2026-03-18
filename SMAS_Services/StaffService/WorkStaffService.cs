using SMAS_BusinessObject.DTOs.StaffDTO;
using SMAS_BusinessObject.DTOs.WorkShiftDTO;
using SMAS_Repositories.WorkStaffRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.StaffService
{
    public class WorkStaffService : IWorkStaffService
    {
        private readonly IWorkStaffRepository _workStaffRepository;

        public WorkStaffService(IWorkStaffRepository workStaffRepository)
        {
            _workStaffRepository = workStaffRepository;
        }


        public async Task<IEnumerable<StaffWorkingTodayDto>> GetStaffWorkingTodayAsync()
        {
            return await _workStaffRepository.GetStaffWorkingTodayAsync();
        }

        public async Task<IEnumerable<FilterStaffByPositionDto>> GetFilterStaffByPositionAsync(List<string> positions)
        {
            return await _workStaffRepository.GetFilterStaffByPositionAsync(positions);
        }

        public async Task<WorkHistoryResponseDto?> GetAllWorkHistoryByStaffIdAsync(int staffId, int month, int year)
        {
            return await _workStaffRepository.GetAllWorkHistoryByStaffIdAsync(staffId, month, year);
        }

        public async Task<WorkNextSevenDayResponseDto> GetAllWorkNextSevenDayByPositionAsync(List<string> positions)
        {
            return await _workStaffRepository.GetAllWorkNextSevenDayByPositionAsync(positions);
        }

        public async Task<IEnumerable<WorkShiftDto>> GetAllWorkShiftAsync()
        {
            return await _workStaffRepository.GetAllWorkShiftAsync();
    }
        public async Task<(bool Success, string? ErrorMessage, CreateWorkStaffRequestDto? Data)> CreateWorkStaffAsync(CreateWorkStaffRequestDto dto)
        {
            // Validate
            if (dto.UserId <= 0)
                return (false, "Nhân viên không hợp lệ.", null);

            if (dto.ShiftId <= 0)
                return (false, "Ca làm việc không hợp lệ.", null);

            if (dto.WorkDay == default)
                return (false, "Ngày làm việc không hợp lệ.", null);

            if (dto.WorkDay < DateOnly.FromDateTime(DateTime.Today))
                return (false, "Không thể phân công ca cho ngày đã qua.", null);

            // Kiểm tra trùng ca
            var isDuplicate = await _workStaffRepository.IsAlreadyAssignedAsync(dto.UserId, dto.ShiftId, dto.WorkDay);
            if (isDuplicate)
                return (false, $"Nhân viên đã được phân công ca này vào ngày {dto.WorkDay:dd/MM/yyyy}.", null);

            var result = await _workStaffRepository.CreateWorkStaffAsync(dto);
            return (true, null, result);
        }

        public async Task<(bool Success, string? ErrorMessage, UpdateWorkStaffRequestDto? Data)> UpdateWorkStaffAsync(int workStaffId, UpdateWorkStaffRequestDto dto)
        {
            // Validate giờ bắt đầu phải trước giờ kết thúc
            if (dto.CheckInTime.HasValue && dto.CheckOutTime.HasValue
                && dto.CheckInTime >= dto.CheckOutTime)
                return (false, "Giờ bắt đầu phải trước giờ kết thúc.", null);

            return await _workStaffRepository.UpdateWorkStaffAsync(workStaffId, dto);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteWorkStaffAsync(int workStaffId)
            => await _workStaffRepository.DeleteWorkStaffAsync(workStaffId);

        public async Task<int> GetSumWorkShiftThisMonthByJwtIdAsync(int userId)
        {
            return await _workStaffRepository.GetSumWorkShiftThisMonthByJwtIdAsync(userId);
        }
        public async Task<double> GetSumTimeWorkedThisMonthByJwtIdAsync(int userId)
        {
            return await _workStaffRepository.GetSumTimeWorkedThisMonthByJwtIdAsync(userId);
        }

        public async  Task<IEnumerable<ScheduleWorkResponseDTO>> GetScheduleWorkOnWeekbyStaffIdAsync(int staffId, DateOnly date)
        {
            return await _workStaffRepository.GetScheduleWorkOnWeekbyStaffIdAsync(staffId, date);
        }

        public async Task<IEnumerable<WorkStaffResponseDTO>> GetWorkScheduleNotCheckinByStaff(int userId)
        {
            var result =  await _workStaffRepository.GetWorkScheduleNotCheckinByStaff(userId);
            if( result == null || !result.Any()) return new List<WorkStaffResponseDTO>();

        }
    }

}


