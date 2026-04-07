using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.BookEventDTO;
using SMAS_BusinessObject.DTOs.Workflow;
using SMAS_Services.BookEventService;
using SMAS_Services.ContractWorkflow;
using SMAS_Services.ManagerServices;
using System.Security.Claims;

namespace SMAS_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/book-event")]
    public class BookEventController : ControllerBase
    {
        private readonly IBookEventService _bookEventService;
        private readonly IManagerService _managerService;
        private readonly IContractWorkflowService _contractWorkflowService;

        public BookEventController(
            IBookEventService bookEventService,
            IManagerService managerService,
            IContractWorkflowService contractWorkflowService)
        {
            _bookEventService = bookEventService;
            _managerService = managerService;
            _contractWorkflowService = contractWorkflowService;
        }

        /// <summary>
        /// Tất cả đặt sự kiện (BookEvent) sắp xếp theo thời gian tạo tăng dần
        /// </summary>
        [Authorize(Roles = "Manager")]
        [HttpGet("asc-created-at")]
        public async Task<IActionResult> GetAllBookEventASCCreatedAt()
        {
            var result = await _managerService.GetAllBookEventsAscCreatedAtAsync();
            return Ok(result);
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

        [Authorize(Roles = "Manager")]
        [HttpPost("{id:int}/review")]
        public async Task<IActionResult> ReviewBookEvent([FromRoute] int id, [FromBody] BookEventReviewRequestDTO request)
        {
            var staffId = GetUserId();
            if (staffId == null)
                return Unauthorized();

            var (dto, status, error) = await _contractWorkflowService.ReviewBookEventAsync(id, request, staffId.Value);
            return status switch
            {
                200 => Ok(dto),
                400 => BadRequest(new { message = error }),
                404 => NotFound(new { message = error }),
                _ => StatusCode(status, new { message = error })
            };
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("{id:int}/contract")]
        public async Task<IActionResult> CreateContractFromBookEvent([FromRoute] int id, [FromBody] CreateContractFromBookEventRequestDTO request)
        {
            var (dto, status, error) = await _contractWorkflowService.CreateContractFromBookEventAsync(id, request);
            return status switch
            {
                201 => Created($"/api/book-event/{dto!.BookEventId}/detail", dto),
                400 => BadRequest(new { message = error }),
                404 => NotFound(new { message = error }),
                _ => StatusCode(status, new { message = error })
            };
        }

        [Authorize(Roles = "Manager,Staff")]
        [HttpGet("{id:int}/detail")]
        public async Task<IActionResult> GetBookEventDetail([FromRoute] int id)
        {
            var (dto, status, error) = await _contractWorkflowService.GetBookEventDetailAsync(id);
            return status switch
            {
                200 => Ok(dto),
                404 => NotFound(new { message = error }),
                _ => StatusCode(status, new { message = error })
            };
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("{id:int}/confirm")]
        public async Task<IActionResult> ConfirmBookEvent([FromRoute] int id)
        {
            var staffId = GetUserId();
            if (staffId == null)
                return Unauthorized();

            var (dto, status, error) = await _contractWorkflowService.ConfirmBookEventAsync(id, staffId.Value);
            return status switch
            {
                200 => Ok(dto),
                400 => BadRequest(new { message = error }),
                404 => NotFound(new { message = error }),
                _ => StatusCode(status, new { message = error })
            };
        }

        /// <summary>
        /// Lịch sử đặt sự kiện của khách đang đăng nhập (CustomerId từ JWT).
        /// </summary>
        [Authorize(Roles = "Customer")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyBookEventHistory()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int customerId))
                    return Unauthorized();

                var bookEvents = await _bookEventService.GetMyBookEventHistoryAsync(customerId);

                if (bookEvents == null || bookEvents.Count == 0)
                    return Ok(new { data = (object?)null, message = "Bạn chưa có lịch sử đặt sự kiện." });

                return Ok(new { data = bookEvents, message = "Lấy lịch sử đặt sự kiện thành công." });
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

        private int? GetUserId()
        {
            var v = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(v, out var id) ? id : null;
        }
    }
}
