using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.Service;
using SMAS_Services.ServiceServices;
using Microsoft.AspNetCore.Authorization;
namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/services")]
    [Authorize(Roles = "Admin")]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _serviceService;

        public ServiceController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<ServiceListResponse>>> GetAll()
        //{
        //    var result = await _serviceService.GetAllServicesAsync();
        //    if (result == null || !result.Any())
        //    {
        //        return NotFound(new { MsgCode = "MSG_022", Message = "Không có dịch vụ nào !" });
        //    }
        //    return Ok(result);
        //}

        // GET: api/service?id=5  hoặc  api/service (all)
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] int? id)
        {
            if (id.HasValue)
            {
                var service = await _serviceService.GetServiceByIdAsync(id.Value);
                if (service == null)
                {
                    return NotFound(new { MsgCode = "MSG_013", Message = "Dịch vụ không tồn tại !" });
                }
                return Ok(new { Message = "Lấy dịch vụ thành công", Data = service });
            }

            var list = await _serviceService.GetAllServicesAsync();
            if (!list.Any())
            {
                return NotFound(new { MsgCode = "MSG_014", Message = "Không có dịch vụ nào !" });
            }
            return Ok(new { Message = "Lấy danh sách dịch vụ thành công", Data = list });
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ServiceCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _serviceService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetAsync), new { id = result.ServiceId },
                new { Message = "Tạo dịch vụ thành công", Data = result });
        }

        // PUT
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] ServiceUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _serviceService.UpdateAsync(id, dto);
                return Ok(new { Message = "Cập nhật dịch vụ thành công", Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        // DELETE: api/services/{id} -> xóa thật
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var success = await _serviceService.DeleteAsync(id);
            if (!success)
                return NotFound(new { message = $"Không tìm thấy dịch vụ với Id = {id}." });

            return Ok(new { message = $"Đã xóa dịch vụ Id = {id}." });
        }

        // PATCH: api/services/{id}/status?isAvailable=true|false
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatusAsync(int id, [FromQuery] bool isAvailable)
        {
            var success = await _serviceService.UpdateStatusAsync(id, isAvailable);
            if (!success)
                return NotFound(new { message = $"Không tìm thấy dịch vụ với Id = {id}." });

            return Ok(new { message = $"Đã cập nhật trạng thái dịch vụ Id = {id} thành {isAvailable}." });
        }
    }
}
