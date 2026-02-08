using SMAS_BusinessObject.DTOs.Event;
using SMAS_Repositories.EventRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.EventServices
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<IEnumerable<EventListResponse>> GetAllEventsAsync()
        {
            return await _eventRepository.GetAllEventsAsync();
        }
    }

}
