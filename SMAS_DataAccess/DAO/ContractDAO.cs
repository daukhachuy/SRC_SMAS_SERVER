using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_DataAccess.DAO
{
    public class ContractDAO
    {
        private readonly RestaurantDbContext _context;

        public ContractDAO(RestaurantDbContext context)
        {
            _context = context;
        }
        /// Lấy Contract theo BookingCode của BookEvent
      
        public async Task<Contract?> GetContractByBookEventCodeAsync(string bookingCode)
        {
            return await _context.Contracts
                .Include(c => c.Customer)
                .Include(c => c.BookEvent)
                    .ThenInclude(be => be!.Event)
                .Include(c => c.Payments)
                .FirstOrDefaultAsync(c =>
                    c.BookEvent != null &&
                    c.BookEvent.BookingCode == bookingCode);
        }
    }
}
