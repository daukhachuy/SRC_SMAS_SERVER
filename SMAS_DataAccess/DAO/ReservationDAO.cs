using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_DataAccess.DAO
{
    public class ReservationDAO
    {
        private readonly RestaurantDbContext _context;

        public ReservationDAO(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<List<Reservation>> GetAllReservationsAsync()
        {
            return await _context.Reservations.Include(u => u.User)
                                              .Include(s => s.ConfirmedByNavigation.User)
                                              .ToListAsync();
        }
    }
}
