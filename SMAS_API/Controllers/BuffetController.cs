using Microsoft.AspNetCore.Authorization;
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
            if (result == null || !result.Any())
            {
                return NotFound(new { MsgCode = "MSG_028", Message = "Không có buffet nào !" });
            }
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPatch("status-buffer/{id}")]
        public async Task<IActionResult> UpdateBufferStatus(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "BufferId phải lớn hơn 0" });
            }
            var result = await _buffetService.UpdateStatusByBuffetId(id);
            if (!result)
                return NotFound(new { MsgCode = "MSG_021", Message = "Không tìm thấy buffer !" });
            return Ok(new { MsgCode = "MSG_022", Message = "Cập nhật trạng thái món ăn thành công !" });
        }
    }
}
