using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_Services.AdminServices;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/admin/dashboard")]
    //[Authorize(Roles = "Admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminDashboardController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        /// Tổng quan hệ thống: doanh thu, chi phí nhập kho, hợp đồng mới, khách hàng mới.
        /// Mặc định trả về tháng hiện tại nếu không truyền tham số.
        [HttpGet("summary")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSummary([FromQuery] int? month, [FromQuery] int? year)
        {
            try
            {
                var result = await _adminService.GetSummaryAsync(month, year);
                return Ok(new { success = true, data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống.", detail = ex.Message });
            }
        }


        /// Biểu đồ doanh thu và chi phí theo tháng.
        /// Mặc định 6 tháng gần nhất của năm hiện tại.
        [HttpGet("revenue-chart")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRevenueChart([FromQuery] int? months, [FromQuery] int? year)
        {
            try
            {
                var result = await _adminService.GetRevenueChartAsync(months, year);
                return Ok(new { success = true, data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống.", detail = ex.Message });
            }
        }

        
        /// Cơ cấu đơn hàng theo loại: DineIn, TakeAway, Delivery.
        /// Mặc định trả về tháng hiện tại
        [HttpGet("order-structure")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrderStructure([FromQuery] int? month, [FromQuery] int? year)
        {
            try
            {
                var result = await _adminService.GetOrderStructureAsync(month, year);
                return Ok(new { success = true, data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống.", detail = ex.Message });
            }
        }

        /// Danh sách giao dịch nhập kho gần nhất.
        /// Mặc định trả về 5 giao dịch gần nhất nếu không truyền limit.
        [HttpGet("warehouse-transactions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWarehouseTransactions([FromQuery] int? limit)
        {
            try
            {
                var result = await _adminService.GetWarehouseTransactionsAsync(limit);
                return Ok(new { success = true, data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống.", detail = ex.Message });
            }
        }
    }
}
