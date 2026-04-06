using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.BlogDTo;
using SMAS_BusinessObject.DTOs.CategoryDTO;
using SMAS_Services.CategoryServices;
using SMAS_Services.ComboServices;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/category")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        //[HttpGet("lists")]
        //public async Task<ActionResult<CategoryResponse>> GetAllCategorysAsync()
        //{
        //    var result = await _categoryService.GetAllCategoriesAsync();
        //    if (result == null || !result.Any())
        //    {
        //        return NotFound(new { MsgCode = "MSG_027", Message = "Không có danh mục nào !" });
        //    }
        //    return Ok(result);
        //}
        // GET: api/categories        -> lấy tất cả
        // GET: api/categories?id=3   -> lấy theo id
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] int? id)
        {
            if (id.HasValue)
            {
                var category = await _categoryService.GetByIdAsync(id.Value);
                if (category == null)
                    return NotFound(new { message = $"Không tìm thấy danh mục với Id = {id}." });

                return Ok(category);
            }

            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        // POST: api/categories
        [HttpPost]
        public async Task<ActionResult<CategoryResponse>> CreateAsync([FromBody] CategoryCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _categoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetAsync), new { id = created.CategoryId }, created);
        }

        // PUT: api/categories/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<CategoryResponse>> UpdaUpdateAsyncte(int id, [FromBody] CategoryUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _categoryService.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound(new { message = $"Không tìm thấy danh mục với Id = {id}." });

            return Ok(updated);
        }

        // DELETE: api/categories/{id} -> xóa thật
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var success = await _categoryService.DeleteAsync(id);
            if (!success)
                return NotFound(new { message = $"Không tìm thấy danh mục với Id = {id}." });

            return Ok(new { message = $"Đã xóa danh mục Id = {id}." });
        }

        // PATCH: api/categories/{id}/status?isAvailable=true|false
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatusAsync(int id, [FromQuery] bool isAvailable)
        {
            var success = await _categoryService.UpdateStatusAsync(id, isAvailable);
            if (!success)
                return NotFound(new { message = $"Không tìm thấy danh mục với Id = {id}." });

            return Ok(new { message = $"Đã cập nhật trạng thái danh mục Id = {id} thành {isAvailable}." });
        }

        [HttpGet("lists-contaifood")]
        public async Task<ActionResult<CategoryResponse>> GetAllCategoryContainFoodAsync()
        {
            var result = await _categoryService.GetAllCategoryContainFoodAsync();
            if (result == null || !result.Any())
            {
                return NotFound(new { MsgCode = "MSG_027", Message = "Không có danh mục nào !" });
            }
            return Ok(result);
        }

    }
}
