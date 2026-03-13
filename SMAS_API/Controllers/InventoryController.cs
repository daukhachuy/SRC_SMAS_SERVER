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
    }
}
