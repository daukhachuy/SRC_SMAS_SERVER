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
        Task<BookEvent> CreateBookEventWithDetailsAsync(
            BookEvent bookEvent,
            List<SMAS_BusinessObject.Models.BookEventService> bookEventServices,
            List<EventFood> eventFoods);
    }
}
