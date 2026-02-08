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
    }

}
