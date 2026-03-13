using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_Services.BookEventService;

namespace SMAS_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/book-event")]
    public class BookEventController : ControllerBase
    {
        private readonly IBookEventService _bookEventService;

        public BookEventController(IBookEventService bookEventService)
        {
            _bookEventService = bookEventService;
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("active")]
        public async Task<IActionResult> GetAllActiveBookEvent()
        {
            try
            {
                var bookEvents = await _bookEventService.GetAllActiveBookEventAsync();

                if (bookEvents == null || bookEvents.Count == 0)
                    return Ok(new { data = (object?)null, message = "Không có sự kiện đặt chỗ nào đang hoạt động." });

                return Ok(new { data = bookEvents, message = "Lấy danh sách đặt sự kiện thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống.", detail = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("{bookEventId}")]
        public async Task<IActionResult> GetBookEventById([FromRoute] int bookEventId)
        {
            try
            {
                var bookEvent = await _bookEventService.GetBookEventByIdAsync(bookEventId);

                if (bookEvent == null)
                    return Ok(new { data = (object?)null, message = $"Không tìm thấy sự kiện với id: {bookEventId}" });

                return Ok(new { data = bookEvent, message = "Lấy thông tin đặt sự kiện thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống.", detail = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("history")]
        public async Task<IActionResult> GetAllBookEventCompleteAndCancel()
        {
            try
            {
                var bookEvents = await _bookEventService.GetAllBookEventCompleteAndCancelAsync();

                if (bookEvents == null || bookEvents.Count == 0)
                    return Ok(new { data = (object?)null, message = "Không có sự kiện đã hoàn thành hoặc đã huỷ." });

                return Ok(new { data = bookEvents, message = "Lấy danh sách sự kiện thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống.", detail = ex.Message });
            }
        }

        }
}
