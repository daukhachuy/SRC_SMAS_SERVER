using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.Service;
using SMAS_Services.ServiceServices;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/services")]
    //[Authorize(Roles = "Manager")]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _serviceService;

        public ServiceController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceListResponse>>> GetAll()
        {
            var result = await _serviceService.GetAllServicesAsync();
            if (result == null || !result.Any())
            {
                return NotFound(new { MsgCode = "MSG_022", Message = "Không có dịch vụ nào !" });
            }
            return Ok(result);
        }

        // GET: api/service?id=5  hoặc  api/service (all)
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int? id)
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
        public async Task<IActionResult> Create([FromBody] ServiceCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _serviceService.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = result.ServiceId },
                new { Message = "Tạo dịch vụ thành công", Data = result });
        }

        // PUT
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ServiceUpdateDto dto)
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

        // DELETE → Soft Delete (IsAvailable = false)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _serviceService.DeleteAsync(id);
                return Ok(new { Message = "Xóa dịch vụ thành công" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
