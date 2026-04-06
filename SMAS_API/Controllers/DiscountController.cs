using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.DiscountDTO;
using SMAS_Services.DiscountServices;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/discount")]
    [Authorize(Roles = "Admin")]
    public class DiscountController : Controller
    {
        private readonly IDiscountService _discountService;

        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }


        [HttpGet("code/{code}")]
        public async Task<ActionResult<DiscountResponse>> GetDiscountByCode(string code)
        {
            var result = await _discountService.GetDiscountByCodeAsync(code);
            if (result == null)
            {
                return NotFound(new { MsgCode = "MSG_013", Message = "Mã giảm giá không tồn tại !" });
            }
            return Ok(result);
        }

        // GET: api/discounts        -> lấy tất cả
        // GET: api/discounts?id=2   -> lấy theo id
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] int? id)
        {
            if (id.HasValue)
            {
                var discount = await _discountService.GetByIdAsync(id.Value);
                if (discount == null)
                    return NotFound(new { message = $"Không tìm thấy discount với Id = {id}." });
                return Ok(discount);
            }

            return Ok(await _discountService.GetAllDiscountsAsync());
        }

        // POST: api/discounts
        [HttpPost]
        public async Task<ActionResult<DiscountResponse>> CreateAsync([FromBody] DiscountCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _discountService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetAsync), new { id = created.DiscountId }, created);
        }

        // PUT: api/discounts/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<DiscountResponse>> UpdateAsync(int id, [FromBody] DiscountUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _discountService.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound(new { message = $"Không tìm thấy discount với Id = {id}." });

            return Ok(updated);
        }

        // DELETE: api/discounts/{id} 
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var success = await _discountService.DeleteAsync(id);
            if (!success)
                return NotFound(new { message = $"Không tìm thấy discount với Id = {id}." });

            return Ok(new { message = $"Đã xóa discount Id = {id}." });
        }

        // PATCH: api/discounts/{id}/status?status=Active
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatusAsync(int id, [FromQuery] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return BadRequest(new { message = "Status không được để trống." });

            var success = await _discountService.UpdateStatusAsync(id, status);
            if (!success)
                return NotFound(new { message = $"Không tìm thấy discount với Id = {id}." });

            return Ok(new { message = $"Đã cập nhật trạng thái discount Id = {id} thành '{status}'." });
        }
    }
}
