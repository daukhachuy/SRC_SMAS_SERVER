using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMAS_BusinessObject.DTOs.StaffDTO;

namespace SMAS_Repositories.WorkStaffRepository
{
    public interface IWorkStaffRepository
    {
        Task<IEnumerable<StaffWorkingTodayDto>> GetStaffWorkingTodayAsync();
        Task<IEnumerable<FilterStaffByPositionDto>> GetFilterStaffByPositionAsync(List<string> positions);
        Task<WorkHistoryResponseDto?> GetAllWorkHistoryByStaffIdAsync(int staffId, int month, int year);
        Task<WorkNextSevenDayResponseDto> GetAllWorkNextSevenDayByPositionAsync(List<string> positions);
        Task<IEnumerable<WorkShiftDto>> GetAllWorkShiftAsync();
        Task<bool> IsAlreadyAssignedAsync(int userId, int shiftId, DateOnly workDay);
        Task<CreateWorkStaffRequestDto> CreateWorkStaffAsync(CreateWorkStaffRequestDto dto);
        Task<(bool Success, string? ErrorMessage, UpdateWorkStaffRequestDto? Data)> UpdateWorkStaffAsync(int workStaffId, UpdateWorkStaffRequestDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteWorkStaffAsync(int workStaffId);
    }
}

