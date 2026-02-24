using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.Service;
using SMAS_Services.ServiceServices;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/services")]
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
            return Ok(result);
        }
    }
}
