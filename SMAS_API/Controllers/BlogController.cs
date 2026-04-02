using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.BlogDTo;
using SMAS_Services.BlogServices;
using Microsoft.AspNetCore.Authorization;
namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/blogs")]
    [Authorize(Roles = "Manager")]
    public class BlogController : Controller
    {
        private readonly IBlogServices _blogServices;

        public BlogController(IBlogServices blogServices)
        {
            _blogServices = blogServices;
        }

        [HttpGet("lists")]
        public async Task<ActionResult<BlogResponse>> GetAllBlogs()
        {
            var result = await _blogServices.GetAllBlogsAsync();
            if(result == null || !result.Any())
            {
                return NotFound(new { MsgCode = "MSG_029", Message = "Không có bài viết nào !" });
            }
            return Ok(result);
        }
        // GET: api/blog?id=5  hoặc  api/blog (all)
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int? id)
        {
            if (id.HasValue)
            {
                var blog = await _blogServices.GetByIdAsync(id.Value);
                if (blog == null)
                {
                    return NotFound(new { MsgCode = "MSG_013", Message = "Blog không tồn tại !" });
                }
                return Ok(new { Message = "Lấy blog thành công", Data = blog });
            }

            var list = await _blogServices.GetAllBlogsAsync();
            if (!list.Any())
            {
                return NotFound(new { MsgCode = "MSG_014", Message = "Không có blog nào !" });
            }
            return Ok(new { Message = "Lấy danh sách blog thành công", Data = list });
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BlogCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _blogServices.CreateAsync(dto);
                return CreatedAtAction(nameof(Get), new { id = result.BlogId },
                    new { Message = "Tạo blog thành công", Data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // PUT
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] BlogUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _blogServices.UpdateAsync(id, dto);
                return Ok(new { Message = "Cập nhật blog thành công", Data = result });
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

        // DELETE → Soft Delete (Status = "Disabled")
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _blogServices.DeleteAsync(id);
                return Ok(new { Message = "Xóa blog thành công" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
