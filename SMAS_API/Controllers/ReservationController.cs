using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.ReservationDTO;
using SMAS_Services.ReservationServices;
using System.Security.Claims;

namespace SMAS_API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/reservation")]
    public class ReservationController : Controller
    {
        private readonly IReservationService _reservationservice;

        public ReservationController(IReservationService reservationservice)
        {
            _reservationservice = reservationservice;
        }

        [HttpGet]
        public async Task<ActionResult<ReservationListResponse>> GetAllReservation()
        {
            var  reservation = await _reservationservice.GetAllReservationsAsync();
            return Ok(reservation);
        }
        // GET api/reservation/my  - lấy reservation của người đang đăng nhập
        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<ReservationListResponse>>> GetMyReservations([FromQuery] int? userId = null)
        {
            var resolvedUserId = userId ?? GetUserIdFromToken();
            if (resolvedUserId == null)
                return Unauthorized("Không xác định được người dùng.");

            var result = await _reservationservice.GetMyReservationsAsync(resolvedUserId.Value);
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<ActionResult<ReservationListResponse>> PostReservation([FromBody] ReservationCreateRequestDTO dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Không xác định được người dùng.");

            int userId = int.Parse(userIdClaim);
            var result = await _reservationservice.CreatePendingReservation(dto, userId);
            if (result == null)
            {
                return BadRequest(new { MsgCode = "MSG_011", Message = "Bạn đã đặt chỗ vào ngày giờ này rồi !" });
            }
            if (result.ReservationId == 0)
            {
                return BadRequest(new { MsgCode = "MSG_012", Message = "Thêm lịch đặt chỗ không thành công !" });
            }
            return Ok( result );

        }

        private int? GetUserIdFromToken()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? User.FindFirst("id")?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }
    }
}
