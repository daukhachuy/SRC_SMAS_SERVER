using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.DiscountDTO;
using SMAS_Services.DiscountServices;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/discount")]
  
    public class DiscountController : Controller
    {
        private readonly IDiscountService _discountService;

        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        // GET: Có id → lấy theo id | Không có → lấy all
        [HttpGet]
        public async Task<IActionResult> GetDiscountAsync([FromQuery] int? id)
        {
            if (id.HasValue)
            {
                var discount = await _discountService.GetByIdAsync(id.Value);
                if (discount == null)
                {
                    return NotFound(new { MsgCode = "MSG_013", Message = "Mã giảm giá không tồn tại !" });
                }

                return Ok(new { Message = "Lấy mã giảm giá thành công", Data = discount });
            }

            var list = await _discountService.GetAllDiscountsAsync();
            if (list == null || !list.Any())
            {
                return NotFound(new { MsgCode = "MSG_014", Message = "Không có mã giảm giá nào !" });
            }

            return Ok(new { Message = "Lấy danh sách thành công", Data = list });
        }
       
        [HttpGet("code/{code}")]
        public async Task<ActionResult<DiscountResponse>> GetDiscountByCodeAsync(string code)
        {
            var result = await _discountService.GetDiscountByCodeAsync(code);
            if (result == null)
            {
                return NotFound(new { MsgCode = "MSG_013", Message = "Mã giảm giá không tồn tại !" });
            }
            return Ok(result);
        }

        [Authorize(Roles = "Manager/Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DiscountCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _discountService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetDiscountAsync),
                    new { id = result.DiscountId },
                    new { Message = "Tạo mã giảm giá thành công", Data = result }
                );
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [Authorize(Roles = "Manager/Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateDiscountAsync(int id, [FromBody] DiscountUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _discountService.UpdateAsync(id, dto);
                return Ok(new { Message = "Cập nhật thành công", Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [Authorize(Roles = "Manager/Admin")]
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> DeleteDiscountAsync(int id)
        {
            try
            {
                await _discountService.DeleteAsync(id);
                return Ok(new { Message = "Xóa mã giảm giá thành công" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        // VALIDATE
        [HttpPost("validate")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateDiscountAsync([FromBody] DiscountValidateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _discountService.ValidateAndApplyAsync(dto);
            if (result == null)
            {
                return BadRequest(new
                {
                    Message = "Discount code is invalid, expired, or not applicable."
                });
            }

            return Ok(new { Message = "Áp dụng mã thành công", Data = result });
        }
    }
}
