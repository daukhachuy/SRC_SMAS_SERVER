using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SMAS_BusinessObject.DTOs.TableDTO;
using SMAS_BusinessObject.Enums;
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
        private readonly ITableService _tableService;
        private readonly ILogger<TableController> _logger;

        public TableController(
            IManagerService managerService,
            ITableService tableService,
            ILogger<TableController> logger)
        {
            _managerService = managerService;
            _tableService = tableService;
            _logger = logger;
        }

        /// Lấy danh sách bàn trống
        [Authorize(Roles = "Manager,Admin")]
        [HttpGet("empty")]
        public async Task<IActionResult> GetTableEmpty()
        {
            var result = await _managerService.GetEmptyTablesAsync();

            if (result == null || !result.Any())
                return Ok(new
                {
                    data = Array.Empty<object>(),
                    total = 0,                        
                    message = "Hiện tại không có bàn trống."
                });

            return Ok(new
            {
                data = result,
                total = result.Count(),             
                message = $"Có {result.Count()} bàn đang trống."
            });
        }

        /// <summary>
        /// Waiter mở bàn - POST /api/tables/{tableCode}/open
        /// </summary>
        [HttpPost("/api/tables/{tableCode}/open")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> OpenTable(string tableCode, [FromQuery] int? userId = null)
        {
            try
            {
                var resolvedUserId = userId ?? GetUserIdFromToken();
                if (resolvedUserId == null) return Unauthorized("Không xác định được người dùng.");

                var (success, errorCode, data) = await _tableService.OpenTableAsync(tableCode, resolvedUserId.Value);
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
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> CloseTable(string tableCode, [FromQuery] int? userId = null)
        {
            try
            {
                var resolvedUserId = userId ?? GetUserIdFromToken();
                if (resolvedUserId == null) return Unauthorized("Không xác định được người dùng.");

                var (success, errorCode, data) = await _tableService.CloseTableAsync(tableCode, resolvedUserId.Value);
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
                var (success, errorCode, data) = await _tableService.InitSessionAsync(tableCode);
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

                var (success, errorCode, data) = await _tableService.RefreshAsync(dto.RefreshToken);
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
                var result = await _tableService.GetActiveSessionAsync(tableCode);
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

        [HttpGet]
        public async Task<IActionResult> GetAllTables(
         [FromQuery] string? tableType,
         [FromQuery] string? status)
        {
            try
            {
                var result = await _tableService.GetTablesAsync(tableType, status);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống.", detail = ex.Message });
            }
        }

        /// <summary>Thêm bàn mới</summary>
        [HttpPost]
        public async Task<IActionResult> CreateTable([FromBody] CreateTableDto dto)
        {
            try
            {
                var result = await _tableService.CreateTableAsync(dto);
                return Ok(new { success = true, data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống.", detail = ex.Message });
            }
        }

        /// <summary>Cập nhật thông tin bàn</summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTable(int id, [FromBody] UpdateTableDto dto)
        {
            try
            {
                var result = await _tableService.UpdateTableAsync(id, dto);
                if (result == null)
                    return NotFound(new { success = false, message = $"Không tìm thấy bàn #{id}." });

                return Ok(new { success = true, data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống.", detail = ex.Message });
            }
        }

        /// <summary>Xóa bàn (soft delete — không cho xóa bàn đang có khách)</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTable(int id)
        {
            try
            {
                var result = await _tableService.DeleteTableAsync(id);
                if (!result)
                    return NotFound(new { success = false, message = $"Không tìm thấy bàn #{id}." });

                return Ok(new { success = true, message = $"Đã xóa bàn #{id}." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống.", detail = ex.Message });
            }
        }
        [HttpPost("/api/tables/{tableCode}/clear-cache")]
        public IActionResult ClearTableCache(string tableCode)
        {
            if (int.TryParse(tableCode, out int tableId))
            {
                HttpContext.RequestServices.GetRequiredService<IMemoryCache>()
                    .Remove($"table_session_{tableId}");
                return Ok(new { message = $"Đã xóa cache của bàn {tableId}" });
            }
            return BadRequest();
        }
    }
}
