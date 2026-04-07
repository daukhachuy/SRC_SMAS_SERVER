using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.Models;
using System;
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
        public async Task<bool> DeleteAsync(int id)
        {
            var evt = await _context.Events.FindAsync(id);
            if (evt == null) return false;

            _context.Events.Remove(evt);
            await _context.SaveChangesAsync();
            return true;
        }

        // Patch status: ch? c?p nh?t IsActive
        public async Task<bool> UpdateStatusAsync(int id, bool isActive)
        {
            var evt = await _context.Events.FindAsync(id);
            if (evt == null) return false;

            evt.IsActive = isActive;
            evt.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
