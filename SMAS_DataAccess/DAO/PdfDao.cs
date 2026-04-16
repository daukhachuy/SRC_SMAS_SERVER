using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.DTOs.AIDTO;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_DataAccess.DAO
{
    public class PdfDao
    {
        private readonly RestaurantDbContext _context;

        public PdfDao(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetInvoiceByIdAsync(string OrderCode)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.ServedByNavigation)
                    .ThenInclude(s => s!.User)
                .Include(o => o.Delivery)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Food)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Buffet)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Combo)
                .Include(o => o.Payments)
                    .ThenInclude(p => p.ReceivedByNavigation)
                .FirstOrDefaultAsync(o => o.OrderCode == OrderCode);
        }

        public async Task<Contract?> GetContractByIdAsync(string contractcode)
        {
            return await _context.Contracts
                .Include(c => c.Customer)
                .Include(c => c.BookEvent)
                    .ThenInclude(be => be.Event)
                .Include(c => c.BookEvent)
                    .ThenInclude(cm => cm.ConfirmedByNavigation)
                    .ThenInclude(u => u.User)
                .Include(c => c.Payments)
                .FirstOrDefaultAsync(c => c.BookEvent != null && c.ContractCode == contractcode);
        }

        public async Task<BookEvent?> GetBookEventAsync(int bookEventId)
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
    }
}
