using SMAS_BusinessObject.DTOs.StaffDTO;
using SMAS_BusinessObject.DTOs.WorkShiftDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.StaffService
{
    public interface IWorkStaffService
    {
        Task<IEnumerable<StaffWorkingTodayDto>> GetStaffWorkingTodayAsync();
        Task<IEnumerable<FilterStaffByPositionDto>> GetFilterStaffByPositionAsync(List<string> positions);
        Task<WorkHistoryResponseDto?> GetAllWorkHistoryByStaffIdAsync(int staffId, int month, int year);
        Task<WorkNextSevenDayResponseDto> GetAllWorkNextSevenDayByPositionAsync(List<string> positions);
        Task<IEnumerable<WorkShiftDto>> GetAllWorkShiftAsync();

        Task<(bool Success, string? ErrorMessage, CreateWorkStaffRequestDto? Data)> CreateWorkStaffAsync(CreateWorkStaffRequestDto dto);
        Task<(bool Success, string? ErrorMessage, UpdateWorkStaffRequestDto? Data)> UpdateWorkStaffAsync(int workStaffId, UpdateWorkStaffRequestDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteWorkStaffAsync(int workStaffId);

        Task<int> GetSumWorkShiftThisMonthByJwtIdAsync(int userId);
        Task<double> GetSumTimeWorkedThisMonthByJwtIdAsync(int userId);

        Task<IEnumerable<ScheduleWorkResponseDTO>> GetScheduleWorkOnWeekbyStaffIdAsync(int staffId, DateOnly date);

        Task<IEnumerable<WorkStaffResponseDTO>> GetWorkScheduleNotCheckinByStaff(int userId);
    }
}
