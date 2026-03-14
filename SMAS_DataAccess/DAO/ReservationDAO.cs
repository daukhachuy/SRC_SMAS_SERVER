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

        public bool CheckCodeExists(string code) => _context.Reservations.Any(r => r.ReservationCode == code);

        public async Task<bool> CheckDuplicateReservation(int userId, DateOnly date, TimeOnly time)
        {
            return await _context.Reservations.AnyAsync(r =>
                r.UserId == userId &&
                r.ReservationDate == date &&
                r.ReservationTime == time &&
                r.Status != "Cancelled");
        }

        public async Task<Reservation> AddReservation(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            var addedReservation = await _context.Reservations.Include(u => u.User)
                                                        .Include(s => s.ConfirmedByNavigation.User)
                                                        .FirstOrDefaultAsync(r => r.ReservationCode == reservation.ReservationCode);
            return addedReservation!;
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByUserIdAsync(int userId)
        {
            return await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.ConfirmedByNavigation)
                    .ThenInclude(s => s.User)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.ReservationDate)
                .ThenByDescending(r => r.ReservationTime)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
