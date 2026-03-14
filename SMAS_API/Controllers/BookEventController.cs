using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.BookEventDTO;
using SMAS_Services.BookEventService;
using System.Security.Claims;

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

        /// <summary>
        /// Hoàn thành đặt sự kiện (sau khi điền đủ 3 bước và bấm "Hoàn thành thực đơn").
        /// CustomerId lấy tự động từ JWT (không cần gửi trong body).
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CompleteBookEvent([FromBody] CreateBookEventApiRequestDTO request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                if (request == null)
                    return BadRequest(new { message = "Dữ liệu đặt sự kiện không hợp lệ." });

                if (request.EventId <= 0)
                    return BadRequest(new { message = "Vui lòng chọn loại sự kiện." });
                if (request.NumberOfGuests <= 0)
                    return BadRequest(new { message = "Số lượng bàn phải lớn hơn 0." });

                var fullRequest = new CreateBookEventRequestDTO
                {
                    CustomerId = userId,
                    NumberOfGuests = request.NumberOfGuests,
                    ReservationDate = request.ReservationDate,
                    ReservationTime = request.ReservationTime,
                    Note = request.Note,
                    Area = request.Area,
                    EventId = request.EventId,
                    Services = request.Services ?? new List<BookEventServiceItemDTO>(),
                    Foods = request.Foods ?? new List<EventFoodItemDTO>()
                };

                var result = await _bookEventService.CreateBookEventWithDetailsAsync(fullRequest);
                return Ok(new { data = result, message = result.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống.", detail = ex.Message });
            }
        }
    }
}
