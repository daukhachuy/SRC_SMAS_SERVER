using SMAS_BusinessObject.DTOs.BookEventDTO;
using SMAS_Repositories.BookEventRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.BookEventService
{
    public class BookEventService : IBookEventService
    {
        private readonly IBookEventRepository _bookEventRepository;

        public BookEventService(IBookEventRepository bookEventRepository)
        {
            _bookEventRepository = bookEventRepository;
        }

        public async Task<List<BookEventListResponseDTO>> GetAllActiveBookEventAsync()
        {
            return await _bookEventRepository.GetAllActiveBookEventAsync();
        }
        public async Task<BookEventListResponseDTO?> GetBookEventByIdAsync(int bookEventId)
        {
            return await _bookEventRepository.GetBookEventByIdAsync(bookEventId);
        }
        public async Task<List<BookEventListResponseDTO>> GetAllBookEventCompleteAndCancelAsync()
        {
            return await _bookEventRepository.GetAllBookEventCompleteAndCancelAsync();
        }
    }
}
