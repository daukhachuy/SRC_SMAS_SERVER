using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SMAS_DataAccess.DAO
{
    public class BookEventDAO
    {
        private readonly RestaurantDbContext _context;

        public BookEventDAO(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<List<BookEvent>> GetAllActiveBookEventAsync()
        {
            return await _context.BookEvents
                .Include(be => be.Customer)
                .Include(be => be.Event)
                .Include(be => be.ConfirmedByNavigation)
                    .ThenInclude(s => s!.User)
                .Include(be => be.Contract)
                .Include(be => be.BookEventServices)
                    .ThenInclude(s => s.Service)
                .Include(be => be.EventFoods)
                    .ThenInclude(ef => ef.Food)
                .Where(be => be.Status != "Cancelled" && be.Status != "Completed")
                .OrderByDescending(be => be.ReservationDate)
                .ToListAsync();
        }
        public async Task<BookEvent?> GetBookEventByIdAsync(int bookEventId)
        {
            return await _context.BookEvents
                .Include(be => be.Customer)
                .Include(be => be.Event)
                .Include(be => be.ConfirmedByNavigation)
                    .ThenInclude(s => s!.User)
                .Include(be => be.Contract)
                .Include(be => be.BookEventServices)
                    .ThenInclude(s => s.Service)
                .Include(be => be.EventFoods)
                    .ThenInclude(ef => ef.Food)
                .FirstOrDefaultAsync(be => be.BookEventId == bookEventId);
        }
        public async Task<List<BookEvent>> GetAllBookEventCompleteAndCancelAsync()
        {
            return await _context.BookEvents
                .Include(be => be.Customer)
                .Include(be => be.Event)
                .Include(be => be.ConfirmedByNavigation)
                    .ThenInclude(s => s!.User)
                .Include(be => be.Contract)
                .Include(be => be.BookEventServices)
                    .ThenInclude(s => s.Service)
                .Include(be => be.EventFoods)
                    .ThenInclude(ef => ef.Food)
                .Where(be => be.Status == "Cancelled" || be.Status == "Completed")
                .OrderByDescending(be => be.ReservationDate)
                .ToListAsync();
        }

        /// <summary>
        /// Tạo đặt sự kiện kèm dịch vụ và món ăn trong một transaction.
        /// Chỉ lưu DB khi tất cả thành công; nếu lỗi thì rollback.
        /// </summary>
        public async Task<BookEvent> CreateBookEventWithDetailsAsync(
            BookEvent bookEvent,
            List<BookEventService> bookEventServices,
            List<EventFood> eventFoods)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.BookEvents.Add(bookEvent);
                await _context.SaveChangesAsync();

                foreach (var s in bookEventServices)
                {
                    s.BookEventId = bookEvent.BookEventId;
                    _context.BookEventServices.Add(s);
                }
                foreach (var f in eventFoods)
                {
                    f.BookEventId = bookEvent.BookEventId;
                    _context.EventFoods.Add(f);
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return bookEvent;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<BookEvent?> GetBookEventForReviewAsync(int bookEventId)
        {
            return await _context.BookEvents.FirstOrDefaultAsync(be => be.BookEventId == bookEventId);
        }

        public async Task<BookEvent?> GetBookEventForCreateContractAsync(int bookEventId)
        {
            return await _context.BookEvents
                .Include(be => be.Event)
                .Include(be => be.EventFoods).ThenInclude(ef => ef.Food)
                .Include(be => be.BookEventServices).ThenInclude(bs => bs.Service)
                .FirstOrDefaultAsync(be => be.BookEventId == bookEventId);
        }

        public async Task<BookEvent?> GetBookEventForDetailAsync(int bookEventId)
        {
            return await _context.BookEvents
                .AsSplitQuery()
                .Include(be => be.Customer)
                .Include(be => be.ConfirmedByNavigation).ThenInclude(s => s!.User)
                .Include(be => be.Event)
                .Include(be => be.EventFoods).ThenInclude(ef => ef.Food)
                .Include(be => be.BookEventServices).ThenInclude(bs => bs.Service)
                .Include(be => be.Contract)
                    .ThenInclude(c => c!.Payments)
                .FirstOrDefaultAsync(be => be.BookEventId == bookEventId);
        }

        public async Task<BookEvent?> GetBookEventWithContractAndCustomerAsync(int bookEventId)
        {
            return await _context.BookEvents
                .Include(be => be.Customer)
                .Include(be => be.Contract)
                .FirstOrDefaultAsync(be => be.BookEventId == bookEventId);
        }

        public async Task UpdateBookEventAsync(BookEvent bookEvent)
        {
            _context.BookEvents.Update(bookEvent);
            await _context.SaveChangesAsync();
        }
    }
}
