using SMAS_BusinessObject.DTOs.BookEventDTO;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.BookEventRepository
{
    public interface IBookEventRepository
    {
        Task<List<BookEventListResponseDTO>> GetAllActiveBookEventAsync();
        Task<BookEventListResponseDTO?> GetBookEventByIdAsync(int bookEventId);
        Task<List<BookEventListResponseDTO>> GetAllBookEventCompleteAndCancelAsync();
        Task<List<BookEventListResponseDTO>> GetBookEventsByCustomerIdAsync(int customerId);
        Task<BookEvent> CreateBookEventWithDetailsAsync(
            BookEvent bookEvent,
            List<SMAS_BusinessObject.Models.BookEventService> bookEventServices,
            List<EventFood> eventFoods);
        Task<BookEventCheckInResponseDTO> CheckInBookEventAsync(int bookEventId, int managerUserId, List<int> tableIds);
        Task<BookEventCheckoutResponseDTO> CheckoutBookEventAsync(int bookEventId, int managerUserId);
        Task<int> NotifyManagersBeforeUpcomingEventsAsync(int hoursBeforeStart);
    }
}
