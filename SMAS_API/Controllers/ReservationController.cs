using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.ReservationDTO;
using SMAS_Services.ReservationServices;
using System.Security.Claims;

namespace SMAS_API.Controllers
{
    [Authorize]
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
    }
}
