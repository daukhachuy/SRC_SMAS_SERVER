using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.Event;
using SMAS_Services.EventServices;
using SMAS_Services.ManagerServices;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/events")]
    [Authorize(Roles = "Admin")]
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

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<EventListResponse>>> GetAll()
        //{
        //    var result = await _eventService.GetAllEventsAsync();
        //    if (result == null || !result.Any())
        //    {
        //        return NotFound(new { MsgCode = "MSG_015", Message = "Không có sự kiện nào !" });
        //    }
        //    return Ok(result);
        //}


        // GET: api/events        -> lấy tất cả
        // GET: api/events?id=2   -> lấy theo id
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] int? id)
        {
            if (id.HasValue)
            {
                var evt = await _eventService.GetEventByIdAsync(id.Value);
                if (evt == null)
                    return NotFound(new { message = $"Không tìm thấy event với Id = {id}." });
                return Ok(evt);
            }

            return Ok(await _eventService.GetAllEventsAsync());
        }

        // POST: api/events
        [HttpPost]
        public async Task<ActionResult<EventListResponse>> CreateAsync([FromBody] EventCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _eventService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetAsync), new { id = created.EventId }, created);
        }

        // PUT: api/events/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<EventListResponse>> UpdateAsync(int id, [FromBody] EventUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _eventService.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound(new { message = $"Không tìm thấy event với Id = {id}." });

            return Ok(updated);
        }

        // DELETE: api/events/{id} 
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var success = await _eventService.DeleteAsync(id);
            if (!success)
                return NotFound(new { message = $"Không tìm thấy event với Id = {id}." });

            return Ok(new { message = $"Đã xóa event Id = {id}." });
        }

        // PATCH: api/events/{id}/status?isActive=true|false
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatusAsync(int id, [FromQuery] bool isActive)
        {
            var success = await _eventService.UpdateStatusAsync(id, isActive);
            if (!success)
                return NotFound(new { message = $"Không tìm thấy event với Id = {id}." });

            return Ok(new { message = $"Đã cập nhật trạng thái event Id = {id} thành {isActive}." });
        }
    }
}
