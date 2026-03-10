using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.Event;
using SMAS_Services.EventServices;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventListResponse>>> GetAll()
        {
            var result = await _eventService.GetAllEventsAsync();
            if (result == null || !result.Any())
            {
                return NotFound(new { MsgCode = "MSG_015", Message = "Không có sự kiện nào !" });
            }
            return Ok(result);
        }
    }
}
