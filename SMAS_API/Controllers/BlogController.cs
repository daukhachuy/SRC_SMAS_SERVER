using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.BlogDTo;
using SMAS_Services.BlogServices;
using Microsoft.AspNetCore.Authorization;
namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/blogs")]
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
            var result = await _blogServices.GetAllAsync();
            if (result == null || !result.Any())
            {
                return NotFound(new { MsgCode = "MSG_029", Message = "Không có bài viết nào !" });
            }
            return Ok(result);
        }

        // GET: api/blogs        -> lấy tất cả
        // GET: api/blogs?id=2   -> lấy theo id
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAsync([FromQuery] int? id)
        {
            if (id.HasValue)
            {
                var blog = await _blogServices.GetByIdAsync(id.Value);
                if (blog == null)
                    return NotFound(new { message = $"Không tìm thấy blog với Id = {id}." });
                return Ok(blog);
            }

            return Ok(await _blogServices.GetAllAsync());
        }

        // POST: api/blogs
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<BlogResponse>> CreateAsync([FromBody] BlogCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _blogServices.CreateAsync(dto);
            return CreatedAtAction(nameof(GetAsync), new { id = created.BlogId }, created);
        }

        // PUT: api/blogs/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<BlogResponse>> UpdateAsync(int id, [FromBody] BlogUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var updated = await _blogServices.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound(new { message = $"Không tìm thấy blog với Id = {id}." });

            return Ok(updated);
        }

        // DELETE: api/blogs/{id} 
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var success = await _blogServices.DeleteAsync(id);
            if (!success)
                return NotFound(new { message = $"Không tìm thấy blog với Id = {id}." });

            return Ok(new { message = $"Đã xóa blog Id = {id}." });
        }

        // PATCH: api/blogs/{id}/status?status=Published
        [Authorize(Roles = "Admin,Manager")]
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatusAsync(int id, [FromQuery] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return BadRequest(new { message = "Status không được để trống." });

            var success = await _blogServices.UpdateStatusAsync(id, status);
            if (!success)
                return NotFound(new { message = $"Không tìm thấy blog với Id = {id}." });

            return Ok(new { message = $"Đã cập nhật trạng thái blog Id = {id} thành '{status}'." });
        }
    }
}
