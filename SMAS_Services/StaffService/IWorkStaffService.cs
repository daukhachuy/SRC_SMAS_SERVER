using SMAS_BusinessObject.DTOs.StaffDTO;
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
    }
}
