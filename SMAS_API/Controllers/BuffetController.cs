using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.BlogDTo;
using SMAS_Services.BufferServices;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/Buffer")]
    public class BuffetController : Controller
    {
        private readonly IBufferServices _buffetService;

        public BuffetController(IBufferServices buffetService)
        {
            _buffetService = buffetService;
        }

        [HttpGet("lists")]
        public async Task<ActionResult<BlogResponse>> GetAllBlogs()
        {
            var result = await _buffetService.GetAllBuffetsAsync();
            return Ok(result);
        }
    }
}
