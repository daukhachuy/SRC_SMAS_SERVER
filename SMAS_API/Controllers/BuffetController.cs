using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.BlogDTo;
using SMAS_BusinessObject.DTOs.BuffetDTO;
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

        // GET: api/buffets        -> lấy tất cả
        // GET: api/buffets?id=2   -> lấy theo id
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] int? id)
        {
            if (id.HasValue)
            {
                var buffet = await _buffetService.GetByIdAsync(id.Value);
                if (buffet == null)
                    return NotFound(new { message = $"Không tìm thấy buffet với Id = {id}." });
                return Ok(buffet);
            }

            return Ok(await _buffetService.GetAllBuffetsAsync());
        }

        // POST: api/buffets
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<BuffetListResponseDTO>> CreateAsync([FromBody] BuffetCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _buffetService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetAsync), new { id = created.BuffetId }, created);
        }

        // PUT: api/buffets/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<BuffetListResponseDTO>> UpdateAsync(int id, [FromBody] BuffetUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _buffetService.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound(new { message = $"Không tìm thấy buffet với Id = {id}." });

            return Ok(updated);
        }

        // DELETE: api/buffets/{id} 
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var success = await _buffetService.DeleteAsync(id);
            if (!success)
                return NotFound(new { message = $"Không tìm thấy buffet với Id = {id}." });

            return Ok(new { message = $"Đã xóa buffet Id = {id}." });
        }

        // PATCH: api/buffets/{id}/status?isAvailable=true|false
        [Authorize(Roles = "Admin,Manager")]
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatusAsync(int id, [FromQuery] bool isAvailable)
        {
            var success = await _buffetService.UpdateStatusAsync(id, isAvailable);
            if (!success)
                return NotFound(new { message = $"Không tìm thấy buffet với Id = {id}." });

            return Ok(new { message = $"Đã cập nhật trạng thái buffet Id = {id} thành {isAvailable}." });
        }
    }
}
