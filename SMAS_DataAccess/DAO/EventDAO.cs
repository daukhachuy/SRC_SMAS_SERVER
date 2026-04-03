using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace SMAS_DataAccess.DAO
{
    public class EventDAO
    {
        private readonly RestaurantDbContext _context;

        public EventDAO(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<List<Event>> GetAllEventsAsync()
        {
            return await _context.Events.ToListAsync();
        }

        public async Task<Event?> GetEventByIdAsync(int eventId)
        {
            return await _context.Events
                .FirstOrDefaultAsync(e => e.EventId == eventId && (e.IsActive == true || e.IsActive == null));
        }
        public async Task<Event> CreateAsync(Event entity)
        {
            _context.Events.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Event> UpdateAsync(Event entity)
        {
            _context.Events.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }

}
