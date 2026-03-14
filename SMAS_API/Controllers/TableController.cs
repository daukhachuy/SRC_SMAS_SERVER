using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.TableDTO;
using SMAS_Services.TableService;
using System.Security.Claims;

namespace SMAS_API.Controllers
{
    [ApiController]
    public class TableSessionController : ControllerBase
    {
        private readonly ITableSessionService _service;
        private readonly ILogger<TableSessionController> _logger;

        public TableSessionController(ITableSessionService service,
                                      ILogger<TableSessionController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // Waiter mở bàn
        // POST /api/tables/{tableCode}/open
        [HttpPost("api/tables/{tableCode}/open")]
        [Authorize(Roles = "Manager,Admin,Waiter")]
        public async Task<IActionResult> OpenTable(string tableCode, [FromQuery] int? userId = null)
        {
            try
            {
                var resolvedUserId = userId ?? GetUserIdFromToken();
                if (resolvedUserId == null) return Unauthorized("Không xác định được người dùng.");

                var (success, errorCode, data) = await _service.OpenTableAsync(tableCode, resolvedUserId.Value);
                if (!success) return MapError(errorCode!);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi mở bàn {TableCode}.", tableCode);
                return StatusCode(500, "Đã xảy ra lỗi hệ thống.");
            }
        }

        // Waiter đóng bàn
        // POST /api/tables/{tableCode}/close
        [HttpPost("api/tables/{tableCode}/close")]
        [Authorize(Roles = "Manager,Admin,Waiter")]
        public async Task<IActionResult> CloseTable(string tableCode, [FromQuery] int? userId = null)
        {
            try
            {
                var resolvedUserId = userId ?? GetUserIdFromToken();
                if (resolvedUserId == null) return Unauthorized("Không xác định được người dùng.");

                var (success, errorCode, data) = await _service.CloseTableAsync(tableCode, resolvedUserId.Value);
                if (!success) return MapError(errorCode!);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi đóng bàn {TableCode}.", tableCode);
                return StatusCode(500, "Đã xảy ra lỗi hệ thống.");
            }
        }

        // Khách quét QR → lấy access + refresh token
        // POST /api/tables/{tableCode}/init
        [HttpPost("api/tables/{tableCode}/init")]
        [AllowAnonymous]
        public async Task<IActionResult> InitSession(string tableCode)
        {
            try
            {
                var (success, errorCode, data) = await _service.InitSessionAsync(tableCode);
                if (!success) return MapError(errorCode!);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi init session bàn {TableCode}.", tableCode);
                return StatusCode(500, "Đã xảy ra lỗi hệ thống.");
            }
        }

        // Refresh access token
        // POST /api/tables/refresh
        [HttpPost("api/tables/refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.RefreshToken))
                    return BadRequest(new { errorCode = "INVALID_QR_TOKEN" });

                var (success, errorCode, data) = await _service.RefreshAsync(dto.RefreshToken);
                if (!success) return MapError(errorCode!);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi refresh token.");
                return StatusCode(500, "Đã xảy ra lỗi hệ thống.");
            }
        }

        // FE kiểm tra realtime
        // GET /api/tables/{tableCode}/active-session
        [HttpGet("api/tables/{tableCode}/active-session")]
        [Authorize(Roles = "Manager,Admin,Waiter")]
        public async Task<IActionResult> GetActiveSession(string tableCode)
        {
            try
            {
                var result = await _service.GetActiveSessionAsync(tableCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lấy session bàn {TableCode}.", tableCode);
                return StatusCode(500, "Đã xảy ra lỗi hệ thống.");
            }
        }

        // Map errorCode → HTTP status chuẩn
        private IActionResult MapError(string errorCode) => errorCode switch
        {
            "TABLE_NOT_FOUND" => NotFound(new { errorCode, message = "Không tìm thấy bàn." }),
            "TABLE_ALREADY_ACTIVE" => Conflict(new { errorCode, message = "Bàn đang có phiên hoạt động." }),
            "SESSION_NOT_ACTIVE" => BadRequest(new { errorCode, message = "Bàn không có phiên đang hoạt động." }),
            "SESSION_EXPIRED" => BadRequest(new { errorCode, message = "Phiên đã hết hạn. Vui lòng liên hệ nhân viên." }),
            "TABLE_CLOSED" => BadRequest(new { errorCode, message = "Bàn đã đóng." }),
            "INVALID_QR_TOKEN" => Unauthorized(new { errorCode, message = "Token không hợp lệ." }),
            _ => BadRequest(new { errorCode })
        };

        private int? GetUserIdFromToken()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }
    }
}
