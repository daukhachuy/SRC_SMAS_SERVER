using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.Food;
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
        [HttpGet("category")]
        public async Task<ActionResult<FoodListResponse>> getall()
        { 
            var result = await _ifoodservice.GetAllFoodsCategoryAsync();
            return Ok(result);
        }
        [HttpGet("discount")]
        public async Task<ActionResult<FoodListResponse>> getallfooddiscount()
        {
            var result = await _ifoodservice.GetAllFoodsDiscountAsync();
            return Ok(result);
        }

        [HttpGet("best-sellers")]
        public async Task<IActionResult> GetTopBestSellers([FromQuery] int top = 10)
        {
            var result = await _ifoodservice.GetTopBestSellersAsync(top);
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
                return NotFound(new { message = "Buffet not found" });

            return Ok(result);
        }
    }
}
