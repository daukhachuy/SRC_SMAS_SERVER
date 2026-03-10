using SMAS_BusinessObject.DTOs.ManagerDTO;
using SMAS_Repositories.ManagerRepositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMAS_Services.ManagerServices
{
    public class ManagerService : IManagerService
    {
        private readonly IManagerRepository _managerRepository;

        public ManagerService(IManagerRepository managerRepository)
        {
            _managerRepository = managerRepository;
        }

        public async Task<IEnumerable<OrderTodayResponseDTO>> GetOrdersTodayAsync()
        {
            return await _managerRepository.GetOrdersTodayAsync();
        }

        public async Task<IEnumerable<TableEmptyResponseDTO>> GetEmptyTablesAsync()
        {
            return await _managerRepository.GetEmptyTablesAsync();
        }

        public async Task<RevenueWeekResponseDTO> GetRevenuePreviousSevenDaysAsync()
        {
            return await _managerRepository.GetRevenuePreviousSevenDaysAsync();
        }

        public async Task<IEnumerable<OrderTodayResponseDTO>> GetFourNewestOrdersAsync()
        {
            return await _managerRepository.GetFourNewestOrdersAsync();
        }

        public async Task<IEnumerable<StaffWorkTodayResponseDTO>> GetStaffWorkTodayAsync()
        {
            return await _managerRepository.GetStaffWorkTodayAsync();
        }

        public async Task<IEnumerable<NotificationResponseDTO>> GetNotificationsByUserIdAsync(int userId)
        {
            return await _managerRepository.GetNotificationsByUserIdAsync(userId);
        }
    }
}
