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
        [Authorize(Roles = "Manager,Admin")]
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

        // GET: api/event?id=5  hoặc  api/event (all)
        [HttpGet]
        public async Task<IActionResult> GetEventAsync([FromQuery] int? id)
        {
            if (id.HasValue)
            {
                var evt = await _eventService.GetEventByIdAsync(id.Value);
                if (evt == null)
                {
                    return NotFound(new { MsgCode = "MSG_013", Message = "Sự kiện không tồn tại !" });
                }
                return Ok(new { Message = "Lấy sự kiện thành công", Data = evt });
            }

            var list = await _eventService.GetAllEventsAsync();
            if (!list.Any())
            {
                return NotFound(new { MsgCode = "MSG_014", Message = "Không có sự kiện nào !" });
            }
            return Ok(new { Message = "Lấy danh sách sự kiện thành công", Data = list });
        }

        // POST
        [Authorize(Roles = "Manager,Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateEventAsync([FromBody] EventCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _eventService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetEventAsync), new { id = result.EventId },
                    new { Message = "Tạo sự kiện thành công", Data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        [Authorize(Roles = "Manager,Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEventAsync(int id, [FromBody] EventUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _eventService.UpdateAsync(id, dto);
                return Ok(new { Message = "Cập nhật sự kiện thành công", Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> PatchEventStatusAsync(int id, [FromBody] EventStatusPatchDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _eventService.PatchStatusAsync(id, dto);
                return Ok(new { Message = "Cập nhật trạng thái sự kiện thành công", Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEventAsync(int id)
        {
            try
            {
                await _eventService.DeleteAsync(id);
                return Ok(new { Message = "Xóa sự kiện thành công" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
