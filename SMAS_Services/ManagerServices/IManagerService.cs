using SMAS_BusinessObject.DTOs.ManagerDTO;
using SMAS_BusinessObject.DTOs.ReservationDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMAS_Services.ManagerServices
{
    public interface IManagerService
    {
        Task<TableAvailabilityResponseDTO> GetTableAvailabilityAsync(DateOnly date, string? timeSlot);
        Task<IEnumerable<OrderTodayResponseDTO>> GetOrdersTodayAsync();
        Task<IEnumerable<TableEmptyResponseDTO>> GetEmptyTablesAsync();
        Task<RevenueWeekResponseDTO> GetRevenuePreviousSevenDaysAsync();
        Task<IEnumerable<OrderTodayResponseDTO>> GetFourNewestOrdersAsync();
        Task<IEnumerable<StaffWorkTodayResponseDTO>> GetStaffWorkTodayAsync();
        
        Task<SumReservationTodayResponseDTO> GetSumReservationTodayAsync();
        Task<IEnumerable<ReservationListResponse>> GetReservationsWaitConfirmAsync();
        Task<IEnumerable<ReservationListResponse>> GetAllReservationsDescCreatedAtAsync();
        Task<IEnumerable<BookEventListResponseDTO>> GetAllBookEventsAscCreatedAtAsync();
        Task<IEnumerable<UpcomingEventResponseDTO>> GetUpcomingEventsAsync();
        Task<NumberContractNeedSignedResponseDTO> GetNumberContractNeedSignedAsync();
        Task<bool> DeleteReservationByReservationCodeAsync(string reservationCode, string cancellationReason, int? managerUserId);
        Task<ReservationListResponse?> PatchConfirmReservationAsync(string reservationCode, int? managerUserId);
    }
}
