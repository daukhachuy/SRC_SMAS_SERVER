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

        public async Task<EventListResponse?> GetEventByIdAsync(int eventId)
        {
            return await _eventRepository.GetEventByIdAsync(eventId);
        }
        public async Task<EventListResponse> CreateAsync(EventCreateDto dto)
        {
            if (dto.MinGuests.HasValue && dto.MaxGuests.HasValue && dto.MinGuests > dto.MaxGuests)
                throw new ArgumentException("MinGuests must be less than or equal to MaxGuests.");

            return await _eventRepository.CreateAsync(dto);
        }

        public async Task<EventListResponse> UpdateAsync(int id, EventUpdateDto dto)
        {
            if (dto.MinGuests.HasValue && dto.MaxGuests.HasValue && dto.MinGuests > dto.MaxGuests)
                throw new ArgumentException("MinGuests must be less than or equal to MaxGuests.");

            return await _eventRepository.UpdateAsync(id, dto);
        }

        public async Task<EventListResponse> PatchStatusAsync(int id, EventStatusPatchDto dto)
            => await _eventRepository.PatchStatusAsync(id, dto);

        public async Task DeleteAsync(int id)
            => await _eventRepository.DeleteAsync(id);
    }

}
