using SMAS_BusinessObject.DTOs.BookEventDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.BookEventService
{
    public interface IBookEventService
    {
        Task<List<BookEventListResponseDTO>> GetAllActiveBookEventAsync();
        Task<BookEventListResponseDTO?> GetBookEventByIdAsync(int bookEventId);
        Task<List<BookEventListResponseDTO>> GetAllBookEventCompleteAndCancelAsync();
        Task<List<BookEventListResponseDTO>> GetMyBookEventHistoryAsync(int customerId);
        Task<CreateBookEventResponseDTO> CreateBookEventWithDetailsAsync(CreateBookEventRequestDTO request);
        Task<BookEventCheckInResponseDTO> CheckInBookEventAsync(int bookEventId, int managerUserId, List<int> tableIds);
        Task<BookEventCheckoutResponseDTO> CheckoutBookEventAsync(int bookEventId, int managerUserId);
        Task<int> NotifyManagersBeforeUpcomingEventsAsync(int hoursBeforeStart);
    }
}
