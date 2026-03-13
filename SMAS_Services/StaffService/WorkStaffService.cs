using SMAS_BusinessObject.DTOs.StaffDTO;
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
    }
}

