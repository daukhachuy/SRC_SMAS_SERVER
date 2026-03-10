using SMAS_BusinessObject.DTOs.ManagerDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMAS_Repositories.ManagerRepositories
{
    public interface IManagerRepository
    {
        Task<IEnumerable<OrderTodayResponseDTO>> GetOrdersTodayAsync();
        Task<IEnumerable<TableEmptyResponseDTO>> GetEmptyTablesAsync();
        Task<RevenueWeekResponseDTO> GetRevenuePreviousSevenDaysAsync();
        Task<IEnumerable<OrderTodayResponseDTO>> GetFourNewestOrdersAsync();
        Task<IEnumerable<StaffWorkTodayResponseDTO>> GetStaffWorkTodayAsync();
        Task<IEnumerable<NotificationResponseDTO>> GetNotificationsByUserIdAsync(int userId);
    }
}
