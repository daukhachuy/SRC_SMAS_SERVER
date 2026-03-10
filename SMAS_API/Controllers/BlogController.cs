using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.BlogDTo;
using SMAS_Services.BlogServices;

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
            var result = await _blogServices.GetAllBlogsAsync();
            if(result == null || !result.Any())
            {
                return NotFound(new { MsgCode = "MSG_029", Message = "Không có bài viết nào !" });
            }
            return Ok(result);
        }
    }
}
