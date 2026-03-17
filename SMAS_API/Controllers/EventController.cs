using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.Event;
using SMAS_Services.EventServices;
using SMAS_Services.ManagerServices;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IManagerService _managerService;

        public EventController(IEventService eventService, IManagerService managerService)
        {
            _eventService = eventService;
            _managerService = managerService;
        }

        /// <summary>
        /// Danh sách sự kiện sắp tới (ReservationDate >= hôm nay)
        /// </summary>
        [Authorize(Roles = "Manager")]
        [HttpGet("upcoming-events")]
        public async Task<IActionResult> GetAllUpcomingEvent()
        {
            var result = await _managerService.GetUpcomingEventsAsync();
            return Ok(result);
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
