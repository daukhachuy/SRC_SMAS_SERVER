using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.Combo;
using SMAS_Services.ComboServices;
using System.Security.Claims;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/combo")]
    public class ComboController : ControllerBase
    {
        private readonly IComboService _comboService;

        public ComboController(IComboService comboService)
        {
            _comboService = comboService;
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAvailableCombos()
        //{
        //    var combos = await _comboService.GetAvailableCombosAsync();
        //    if (combos == null || !combos.Any())
        //    {
        //        return NotFound(new { MsgCode = "MSG_025", Message = "Không có combo nào đang hoạt động !" });
        //    }
        //    return Ok(combos);
        //}

        [HttpPost("filter")]
        public async Task<IActionResult> GetCombosFilter([FromQuery] CombosFilterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (request.MinPrice.HasValue &&
               request.MaxPrice.HasValue &&
               request.MinPrice > request.MaxPrice)
            {
                return BadRequest(new
                {
                    message = "MinPrice không được lớn hơn MaxPrice"
                });
            }
            var filteredCombos = await _comboService.GetCombosFilterAsync(request);
            if (filteredCombos == null || !filteredCombos.Any())
            {
                return NotFound(new { MsgCode = "MSG_026", Message = "Không có combo nào phù hợp với tiêu chí lọc !" });
            }
            return Ok(filteredCombos);
        }
        [Authorize(Roles = "Admin,Manager")]
        [HttpPatch("status-combo/{id}")]
        public async Task<IActionResult> UpdateComboStatus(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "ComboId phải lớn hơn 0" });
            }
            var result = await _comboService.UpdateStatusByComboId(id);
            if (!result)
                return NotFound(new { MsgCode = "MSG_021", Message = "Không tìm thấy combo !" });
            return Ok(new { MsgCode = "MSG_022", Message = "Cập nhật trạng thái món ăn thành công !" });
        }


        // GET: api/combos        -> lấy tất cả
        // GET: api/combos?id=2   -> lấy theo id
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAsync([FromQuery] int? id)
        {
            if (id.HasValue)
            {
                var combo = await _comboService.GetByIdAsync(id.Value);
                if (combo == null)
                    return NotFound(new { message = $"Không tìm thấy combo với Id = {id}." });
                return Ok(combo);
            }

            return Ok(await _comboService.GetAllAsync());
        }

        // POST: api/combos
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ComboListResponse>> CreateAsync([FromBody] ComboCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int createdBy))
                return Unauthorized();

            var created = await _comboService.CreateAsync(dto, createdBy);
            return Ok(created);
        }

        // PUT: api/combos/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ComboListResponse>> UpdateAsync(int id, [FromBody] ComboUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (data, msgCode, message) = await _comboService.UpdateAsync(id, dto);

            if (data == null)
            {
                if (msgCode == "MSG_404")
                    return NotFound(new { MsgCode = msgCode, Message = message });

                return BadRequest(new { MsgCode = msgCode, Message = message });
            }

            return Ok(data);
        }

        // DELETE: api/combos/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var success = await _comboService.DeleteAsync(id);
            if (!success)
                return NotFound(new { message = $"Không tìm thấy combo với Id = {id}." });

            return Ok(new { message = $"Đã xóa combo Id = {id}." });
        }

        // PATCH: api/combos/{id}/status?isAvailable=true|false
        [Authorize(Roles = "Admin,Manager")]
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatusAsync(int id, [FromQuery] bool isAvailable)
        {
            var success = await _comboService.UpdateStatusAsync(id, isAvailable);
            if (!success)
                return NotFound(new { message = $"Không tìm thấy combo với Id = {id}." });

            return Ok(new { message = $"Đã cập nhật trạng thái combo Id = {id} thành {isAvailable}." });
        }
        // POST: api/combo/{comboId}/foods
        // Body: { "foodId": 5, "quantity": 2 }
        [Authorize(Roles = "Admin")]
        [HttpPost("{comboId:int}/foods")]
        public async Task<IActionResult> AddFoodToCombo(int comboId, [FromBody] ComboFoodInputDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (success, msgCode, message) = await _comboService.AddFoodToComboAsync(
                comboId, dto.FoodId, dto.Quantity);

            if (!success)
            {
                if (msgCode == "MSG_404")
                    return NotFound(new { MsgCode = msgCode, Message = message });
                return BadRequest(new { MsgCode = msgCode, Message = message });
            }

            return Ok(new { MsgCode = "MSG_034", Message = "Thêm món vào combo thành công." });
        }

        // DELETE: api/combo/{comboId}/foods/{foodId}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{comboId:int}/foods/{foodId:int}")]
        public async Task<IActionResult> RemoveFoodFromCombo(int comboId, int foodId)
        {
            var (success, msgCode, message) = await _comboService.RemoveFoodFromComboAsync(comboId, foodId);

            if (!success)
            {
                if (msgCode == "MSG_404")
                    return NotFound(new { MsgCode = msgCode, Message = message });
                return BadRequest(new { MsgCode = msgCode, Message = message });
            }

            return Ok(new { MsgCode = "MSG_035", Message = "Xóa món khỏi combo thành công." });
        }

        // PATCH: api/combo/{comboId}/foods/{foodId}/quantity?quantity=3
        [Authorize(Roles = "Admin")]
        [HttpPatch("{comboId:int}/foods/{foodId:int}/quantity")]
        public async Task<IActionResult> UpdateFoodQuantity(int comboId, int foodId, [FromQuery] int quantity)
        {
            var (success, msgCode, message) = await _comboService.UpdateFoodQuantityAsync(
                comboId, foodId, quantity);

            if (!success)
            {
                if (msgCode == "MSG_404")
                    return NotFound(new { MsgCode = msgCode, Message = message });
                return BadRequest(new { MsgCode = msgCode, Message = message });
            }

            return Ok(new { MsgCode = "MSG_036", Message = "Cập nhật số lượng món thành công." });
        }
    }
}
