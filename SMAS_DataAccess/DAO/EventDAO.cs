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
    }

}
