using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.StaffDTO;
using SMAS_Services.StaffServices;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/staff")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        /// <summary>
        /// Lấy thông tin staff theo id (UserId).
        /// GET api/staff/{id}
        /// </summary>
        /// <param name="id">UserId của staff</param>
        /// <returns>StaffResponse hoặc 404 nếu không tồn tại</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(StaffResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StaffResponse>> GetStaffById(int id)
        {
            var staff = await _staffService.GetStaffByIdAsync(id);
            if (staff == null)
                return NotFound();

            return Ok(staff);
        }
    }
}
