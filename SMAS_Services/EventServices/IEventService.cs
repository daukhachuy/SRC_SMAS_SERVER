using SMAS_BusinessObject.DTOs.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.EventServices
{
    public interface IEventService
    {
        Task<IEnumerable<EventListResponse>> GetAllEventsAsync();
        Task<EventListResponse?> GetEventByIdAsync(int eventId);
        Task<EventListResponse> CreateAsync(EventCreateDto dto);
        Task<EventListResponse> UpdateAsync(int id, EventUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateStatusAsync(int id, bool isActive);
    }

}
