using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.IngredientDTO;
using SMAS_BusinessObject.DTOs.InventoryDTO;
using SMAS_Services.InventoryServices;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/inventory")]
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }
        [Authorize(Roles = "Manager,Admin")]
        [HttpGet("getall")]
        public async Task<ActionResult<InventoryResponseDTO>> GetAllInventoryAsync()
        {
            var result = await _inventoryService.GetAllInventoryAsync();
            if (result == null || !result.Any())
            {
                return NotFound(new { MsgCode = "MSG_035", Message = "Không có nguyên liệu nào !" });
            }
            return Ok(result);
        }
        [Authorize(Roles = "Manager,Admin")]
        [HttpGet("logs")]
        public async Task<ActionResult<InventorylogResponseDTO>> GetAllInventoryLogsAsync()
        {
            var result = await _inventoryService.GetAllInventoryLogsAsync();
            if (result == null || !result.Any())
            {
                return NotFound(new { MsgCode = "MSG_036", Message = "Không có lịch sử tồn kho nào !" });
            }
            return Ok(result);
        }
        [Authorize(Roles = "Manager,Admin")]
        [HttpGet("newbatchcode")]
        public async Task<ActionResult<string>> GetNewBatchCodeAsync()
        {
            var newBatchCode = await _inventoryService.GetNewBatchCodeAsync();
            return Ok(newBatchCode);
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost("create")]
        public async Task<ActionResult> CreateInventoryAsync([FromBody] CreateInventoryRequestDTO request)
        {
            var result = await _inventoryService.CreateInventoryAsync(request);
            if (!result)
            {
                return BadRequest(new { MsgCode = "MSG_037", Message = "Tạo lô nguyên liệu thất bại." });
            }
            return Ok(new { MsgCode = "MSG_038", Message = "Tạo lô nguyên liệu thành công." });
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost("create-export")]
        public async Task<ActionResult> CreateExportInventoryAsync([FromBody] ExImportInventoryRequestDTO request)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Không xác định được người dùng.");
            var result = await _inventoryService.ExportInventoryAsync(request, userId);
            if (!result)
            {
                return BadRequest(new { MsgCode = "MSG_039", Message = "Xuất kho nguyên liệu thất bại." });
            }
            return Ok(new { MsgCode = "MSG_040", Message = "Xuất kho nguyên liệu thành công." });
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost("create-import")]
        public async Task<ActionResult> CreateImportInventoryAsync([FromBody] ExImportInventoryRequestDTO request)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Không xác định được người dùng.");
            var result = await _inventoryService.ImportInventoryAsync(request, userId);
            if (!result)
            {
                return BadRequest(new { MsgCode = "MSG_041", Message = "Nhập kho nguyên liệu thất bại." });
            }
            return Ok(new { MsgCode = "MSG_042", Message = "Nhập kho nguyên liệu thành công." });
        }
    }
}
