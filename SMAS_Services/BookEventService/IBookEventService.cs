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
    }
}
