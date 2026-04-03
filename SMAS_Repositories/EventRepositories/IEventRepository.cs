using SMAS_BusinessObject.DTOs.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.EventRepositories
{
    public interface IEventRepository
    {
        Task<IEnumerable<EventListResponse>> GetAllEventsAsync();
        Task<EventListResponse?> GetEventByIdAsync(int eventId);
        Task<EventListResponse> CreateAsync(EventCreateDto dto);
        Task<EventListResponse> UpdateAsync(int id, EventUpdateDto dto);
        Task DeleteAsync(int id);

    }

}
