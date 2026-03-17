using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.TableDTO;
using SMAS_Services.ManagerServices;
using SMAS_Services.TableService;
using System.Security.Claims;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/table")]
    public class TableController : ControllerBase
    {
        private readonly IManagerService _managerService;
        private readonly ITableSessionService _tableSessionService;
        private readonly ILogger<TableController> _logger;

        public TableController(
            IManagerService managerService,
            ITableSessionService tableSessionService,
            ILogger<TableController> logger)
        {
            _managerService = managerService;
            _tableSessionService = tableSessionService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách bàn trống
        /// </summary>
        [Authorize(Roles = "Manager")]
        [HttpGet("empty")]
        public async Task<IActionResult> GetTableEmpty()
        {
            var result = await _managerService.GetEmptyTablesAsync();
            return Ok(result);
        }

        /// <summary>
        /// Waiter mở bàn - POST /api/tables/{tableCode}/open
        /// </summary>
        [HttpPost("/api/tables/{tableCode}/open")]
        [Authorize(Roles = "Manager,Admin,Waiter")]
        public async Task<IActionResult> OpenTable(string tableCode, [FromQuery] int? userId = null)
        {
            try
            {
                var resolvedUserId = userId ?? GetUserIdFromToken();
                if (resolvedUserId == null) return Unauthorized("Không xác định được người dùng.");

                var (success, errorCode, data) = await _tableSessionService.OpenTableAsync(tableCode, resolvedUserId.Value);
                if (!success) return MapError(errorCode!);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi mở bàn {TableCode}.", tableCode);
                return StatusCode(500, "Đã xảy ra lỗi hệ thống.");
            }
        }

        /// <summary>
        /// Waiter đóng bàn - POST /api/tables/{tableCode}/close
        /// </summary>
        [HttpPost("/api/tables/{tableCode}/close")]
        [Authorize(Roles = "Manager,Admin,Waiter")]
        public async Task<IActionResult> CloseTable(string tableCode, [FromQuery] int? userId = null)
        {
            try
            {
                var resolvedUserId = userId ?? GetUserIdFromToken();
                if (resolvedUserId == null) return Unauthorized("Không xác định được người dùng.");

                var (success, errorCode, data) = await _tableSessionService.CloseTableAsync(tableCode, resolvedUserId.Value);
                if (!success) return MapError(errorCode!);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi đóng bàn {TableCode}.", tableCode);
                return StatusCode(500, "Đã xảy ra lỗi hệ thống.");
            }
        }

        /// <summary>
        /// Khách quét QR → lấy access + refresh token - POST /api/tables/{tableCode}/init
        /// </summary>
        [HttpPost("/api/tables/{tableCode}/init")]
        [AllowAnonymous]
        public async Task<IActionResult> InitSession(string tableCode)
        {
            try
            {
                var (success, errorCode, data) = await _tableSessionService.InitSessionAsync(tableCode);
                if (!success) return MapError(errorCode!);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi init session bàn {TableCode}.", tableCode);
                return StatusCode(500, "Đã xảy ra lỗi hệ thống.");
            }
        }

        /// <summary>
        /// Refresh access token - POST /api/tables/refresh
        /// </summary>
        [HttpPost("/api/tables/refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.RefreshToken))
                    return BadRequest(new { errorCode = "INVALID_QR_TOKEN" });

                var (success, errorCode, data) = await _tableSessionService.RefreshAsync(dto.RefreshToken);
                if (!success) return MapError(errorCode!);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi refresh token.");
                return StatusCode(500, "Đã xảy ra lỗi hệ thống.");
            }
        }

        /// <summary>
        /// FE kiểm tra realtime - GET /api/tables/{tableCode}/active-session
        /// </summary>
        [HttpGet("/api/tables/{tableCode}/active-session")]
        [Authorize(Roles = "Manager,Admin,Waiter")]
        public async Task<IActionResult> GetActiveSession(string tableCode)
        {
            try
            {
                var result = await _tableSessionService.GetActiveSessionAsync(tableCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lấy session bàn {TableCode}.", tableCode);
                return StatusCode(500, "Đã xảy ra lỗi hệ thống.");
            }
        }

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
