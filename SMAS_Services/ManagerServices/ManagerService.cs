using SMAS_BusinessObject.DTOs.ManagerDTO;
using SMAS_BusinessObject.DTOs.ReservationDTO;
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

        public async Task<SumReservationTodayResponseDTO> GetSumReservationTodayAsync()
        {
            return await _managerRepository.GetSumReservationTodayAsync();
        }

        public async Task<IEnumerable<ReservationListResponse>> GetReservationsWaitConfirmAsync()
        {
            return await _managerRepository.GetReservationsWaitConfirmAsync();
        }

        public async Task<IEnumerable<ReservationListResponse>> GetAllReservationsDescCreatedAtAsync()
        {
            return await _managerRepository.GetAllReservationsDescCreatedAtAsync();
        }

        public async Task<IEnumerable<BookEventListResponseDTO>> GetAllBookEventsAscCreatedAtAsync()
        {
            return await _managerRepository.GetAllBookEventsAscCreatedAtAsync();
        }

        public async Task<IEnumerable<UpcomingEventResponseDTO>> GetUpcomingEventsAsync()
        {
            return await _managerRepository.GetUpcomingEventsAsync();
        }

        public async Task<NumberContractNeedSignedResponseDTO> GetNumberContractNeedSignedAsync()
        {
            return await _managerRepository.GetNumberContractNeedSignedAsync();
        }
    }
}
