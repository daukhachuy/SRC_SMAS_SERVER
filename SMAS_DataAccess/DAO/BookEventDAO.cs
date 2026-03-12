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
                .Where(be => be.Status == "Cancelled" || be.Status == "Completed")
                .OrderByDescending(be => be.ReservationDate)
                .ToListAsync();
        }
    }
}
