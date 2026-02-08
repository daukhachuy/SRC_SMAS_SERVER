using SMAS_BusinessObject.DTOs.Event;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.EventRepositories
{
    public class EventRepository : IEventRepository
    {
        private readonly EventDAO _eventDAO;

        public EventRepository(EventDAO eventDAO)
        {
            _eventDAO = eventDAO;
        }

        public async Task<IEnumerable<EventListResponse>> GetAllEventsAsync()
        {
            var events = await _eventDAO.GetAllEventsAsync();
            return events.Select(e => new EventListResponse
            {
                EventId = e.EventId,
                Title = e.Title,
                Description = e.Description,
                EventType = e.EventType,
                Image = e.Image,
                MinGuests = e.MinGuests,
                MaxGuests = e.MaxGuests,
                BasePrice = e.BasePrice,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                CreatedBy = e.CreatedBy,
                IsActive = e.IsActive
            });
        }
    }

}
