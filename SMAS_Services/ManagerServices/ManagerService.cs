using SMAS_BusinessObject.DTOs.ManagerDTO;
using SMAS_BusinessObject.DTOs.ReservationDTO;
using SMAS_Repositories.ManagerRepositories;
using SMAS_Services.NotificationServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMAS_Services.ManagerServices
{
    public class ManagerService : IManagerService
    {
        private readonly IManagerRepository _managerRepository;
        private readonly INotificationService _notificationService;

        public ManagerService(IManagerRepository managerRepository, INotificationService notificationService)
        {
            _managerRepository = managerRepository;
            _notificationService = notificationService;
        }

        public async Task<TableAvailabilityResponseDTO> GetTableAvailabilityAsync(DateOnly date, string? timeSlot)
        {
            return await _managerRepository.GetTableAvailabilityAsync(date, timeSlot);
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

        public async Task<bool> DeleteReservationByReservationCodeAsync(string reservationCode, string cancellationReason, int? managerUserId)
        {
            var allPending = await _managerRepository.GetAllReservationsDescCreatedAtAsync();
            var target = allPending.FirstOrDefault(r => r.ReservationCode == reservationCode);

            var deleted = await _managerRepository.DeleteReservationByReservationCodeAsync(
                reservationCode, cancellationReason, managerUserId);

            if (deleted && target != null)
            {
                await _notificationService.CreateAutoNotificationAsync(
                    userId: target.UserId,
                    senderId: managerUserId,
                    title: "Đặt bàn đã bị hủy",
                    content: $"Đặt bàn {target.ReservationCode} ngày " +
                             $"{target.ReservationDate:dd/MM/yyyy} đã bị hủy. " +
                             $"Lý do: {cancellationReason}",
                    type: "Reservation",
                    severity: "Warning"
                );
            }

            return deleted;
        }

        public async Task<ReservationListResponse?> PatchConfirmReservationAsync(string reservationCode, int? managerUserId)
        {
            var result = await _managerRepository.PatchConfirmReservationAsync(
                reservationCode, managerUserId);

            if (result != null)
            {
                await _notificationService.CreateAutoNotificationAsync(
                    userId: result.UserId,
                    senderId: managerUserId,
                    title: "Đặt bàn được xác nhận",
                    content: $"Đặt bàn {result.ReservationCode} ngày " +
                             $"{result.ReservationDate:dd/MM/yyyy} lúc " +
                             $"{result.ReservationTime:HH\\:mm} cho " +
                             $"{result.NumberOfGuests} khách đã được xác nhận. " +
                             $"Hẹn gặp bạn tại nhà hàng!",
                    type: "Reservation",
                    severity: "Information"
                );
            }

            return result;
        }
    }
}
