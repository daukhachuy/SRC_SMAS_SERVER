using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.BlogDTo;
using SMAS_Services.CategoryServices;
using SMAS_Services.ComboServices;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/category")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("lists")]
        public async Task<ActionResult<BlogResponse>> GetAllBlogs()
        {
            var result = await _categoryService.GetAllCategoryAsync();
            if (result == null || !result.Any())
            {
                return NotFound(new { MsgCode = "MSG_027", Message = "Không có danh mục nào !" });
            }
            return Ok(result);
        }
    }
}
