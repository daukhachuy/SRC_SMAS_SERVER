using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.Food;
using SMAS_BusinessObject.Models;
using SMAS_Services.AuthServices;
using SMAS_Services.FoodServices;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/food")]
    public class FoodController : Controller
    {
        private readonly IFoodService _ifoodservice;

        public FoodController(IFoodService ifoodservice)
        {
            _ifoodservice = ifoodservice;
        }
        // GET: api/foods        -> lấy tất cả
        // GET: api/foods?id=5   -> lấy theo id
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] int? id)
        {
            if (id.HasValue)
            {
                var food = await _ifoodservice.GetByIdAsync(id.Value);
                if (food == null)
                    return NotFound(new { message = $"Không tìm thấy món ăn với Id = {id}." });

                return Ok(food);
            }

            var foods = await _ifoodservice.GetAllAsync();
            return Ok(foods);
        }
        // GET: api/foods/5  -> dùng cho CreatedAtAction
        [AllowAnonymous]
        [HttpGet("{id:int}", Name = "GetFoodById")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var food = await _ifoodservice.GetByIdAsync(id);
            if (food == null)
                return NotFound(new { message = $"Không tìm thấy món ăn với Id = {id}." });

            return Ok(food);
        }
        // POST: api/foods
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<FoodListResponse>> CreateAsync([FromBody] FoodCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _ifoodservice.CreateAsync(dto);
            return CreatedAtRoute("GetFoodById", new { id = created.FoodId }, created);
        }

        // PUT: api/foods/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<FoodListResponse>> UpdateAsync(int id, [FromBody] FoodUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _ifoodservice.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound(new { message = $"Không tìm thấy món ăn với Id = {id}." });

            return Ok(updated);
        }

        // DELETE: api/foods/{id} 
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var success = await _ifoodservice.DeleteAsync(id);
            if (!success)
                return NotFound(new { message = $"Không tìm thấy món ăn với Id = {id}." });

            return Ok(new { message = $"Đã xóa món ăn Id = {id}." });
        }

        // PATCH: api/foods/{id}/status?isAvailable=true|false
        [Authorize(Roles = "Admin,Manager")]
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatusAsync(int id, [FromQuery] bool isAvailable)
        {
            var success = await _ifoodservice.UpdateStatusAsync(id, isAvailable);
            if (!success)
                return NotFound(new { message = $"Không tìm thấy món ăn với Id = {id}." });

            return Ok(new { message = $"Đã cập nhật trạng thái món ăn Id = {id} thành {isAvailable}." });
        }

        [HttpGet("category")]
        public async Task<ActionResult<FoodListResponse>> getall()
        {
            var result = await _ifoodservice.GetAllFoodsCategoryAsync();
            if (result == null || !result.Any())
            {
                return NotFound(new { MsgCode = "MSG_017", Message = "Không có món ăn nào !" });
            }
            return Ok(result);
        }
        [HttpGet("discount")]
        public async Task<ActionResult<FoodListResponse>> getallfooddiscount()
        {
            var result = await _ifoodservice.GetAllFoodsDiscountAsync();
            if (result == null || !result.Any())
            {
                return NotFound(new { MsgCode = "MSG_018", Message = "Không có món ăn nào đang giảm giá !" });
            }
            return Ok(result);
        }

        [HttpGet("best-sellers")]
        public async Task<IActionResult> GetTopBestSellers([FromQuery] int top = 10)
        {
            var result = await _ifoodservice.GetTopBestSellersAsync(top);
            if (result == null || !result.Any())
            {
                return NotFound(new { MsgCode = "MSG_019", Message = "Không có món ăn nào được bán chạy !" });
            }
            return Ok(result);
        }

        [HttpGet("BuffetId/{id}")]
        public async Task<IActionResult> GetBuffetDetail(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    message = "BuffetId phải lớn hơn 0"
                });
            }
            var result = await _ifoodservice.GetBuffetWithFoodsAsync(id);

            if (result == null)
                return NotFound(new { MsgCode = "MSG_020", Message = "Không tìm thấy buffer !" });

            return Ok(result);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterFoods([FromQuery] FoodFilterRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate logic business
            if (request.MinPrice.HasValue &&
                request.MaxPrice.HasValue &&
                request.MinPrice > request.MaxPrice)
            {
                return BadRequest(new
                {
                    message = "MinPrice không được lớn hơn MaxPrice"
                });
            }

            var result = await _ifoodservice.FilterFoodsAsync(request);

            return Ok(result);
        }


        [Authorize(Roles = "Admin,Manager")]
        [HttpPatch("status-food/{id}")]
        public async Task<IActionResult> UpdateFoodStatus(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "FoodId phải lớn hơn 0" });
            }
            var result = await _ifoodservice.UpdateStatusByFoodId(id);
            if (!result)
                return NotFound(new { MsgCode = "MSG_021", Message = "Không tìm thấy món ăn !" });
            return Ok(new { MsgCode = "MSG_022", Message = "Cập nhật trạng thái món ăn thành công !" });
        }
    }
}
