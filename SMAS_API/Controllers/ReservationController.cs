using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.ManagerDTO;
using SMAS_BusinessObject.DTOs.ReservationDTO;
using SMAS_Services.ManagerServices;
using SMAS_Services.ReservationServices;
using System.Security.Claims;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/reservation")]
    public class ReservationController : Controller
    {
        private readonly IReservationService _reservationservice;
        private readonly IManagerService _managerService;

        public ReservationController(IReservationService reservationservice, IManagerService managerService)
        {
            _reservationservice = reservationservice;
            _managerService = managerService;
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

        /// <summary>
        /// Tổng số lượng đặt bàn trong ngày hôm nay
        /// </summary>
        [Authorize(Roles = "Manager")]
        [HttpGet("sum-today")]
        public async Task<IActionResult> GetSumReservationToday()
        {
            var result = await _managerService.GetSumReservationTodayAsync();
            return Ok(result);
        }

        /// <summary>
        /// Danh sách đặt bàn chờ Manager xác nhận (Pending)
        /// </summary>
        [Authorize(Roles = "Manager")]
        [HttpGet("wait-confirm")]
        public async Task<IActionResult> GetReservationWaitConfirm()
        {
            var result = await _managerService.GetReservationsWaitConfirmAsync();
            return Ok(result);
        }

        /// <summary>
        /// Tất cả đặt bàn sắp xếp theo thời gian tạo giảm dần (mới nhất trước)
        /// </summary>
        [Authorize(Roles = "Manager")]
        [HttpGet("desc-created-at")]
        public async Task<IActionResult> GetAllReservationDESCCreatedAt()
        {
            var result = await _managerService.GetAllReservationsDescCreatedAtAsync();
            return Ok(result);
        }

        /// <summary>
        /// Manager bấm Cancel đặt bàn theo ReservationCode
        /// </summary>
        [Authorize(Roles = "Manager")]
        [HttpDelete("{reservationCode}")]
        public async Task<IActionResult> DeleteReservationByReservationCode(string reservationCode, [FromBody] CancelReservationRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(reservationCode))
                return BadRequest("ReservationCode không hợp lệ.");
            if (dto == null || string.IsNullOrWhiteSpace(dto.CancellationReason))
                return BadRequest("CancellationReason là bắt buộc.");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? managerUserId = null;
            if (int.TryParse(userIdClaim, out int uid))
                managerUserId = uid;

            var deleted = await _managerService.DeleteReservationByReservationCodeAsync(reservationCode.Trim(), dto.CancellationReason.Trim(), managerUserId);
            if (!deleted)
                return NotFound("Không tìm thấy đặt bàn với mã này.");
            return NoContent();
        }

        /// <summary>
        /// Manager bấm Confirm khi reservation đang Pending
        /// </summary>
        [Authorize(Roles = "Manager")]
        [HttpPatch("{reservationCode}/confirm")]
        public async Task<IActionResult> PatchConfirmReservation(string reservationCode)
        {
            if (string.IsNullOrWhiteSpace(reservationCode))
                return BadRequest("ReservationCode không hợp lệ.");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? managerUserId = null;
            if (int.TryParse(userIdClaim, out int uid))
                managerUserId = uid;

            var result = await _managerService.PatchConfirmReservationAsync(reservationCode.Trim(), managerUserId);
            if (result == null)
                return NotFound("Không tìm thấy đặt bàn với mã này hoặc không ở trạng thái Pending.");
            return Ok(result);
        }

        private int? GetUserIdFromToken()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? User.FindFirst("id")?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }
    }
}
