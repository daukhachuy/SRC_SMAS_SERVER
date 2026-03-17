using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_Services.ContractService;
using SMAS_Services.ManagerServices;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/contract")]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;
        private readonly IManagerService _managerService;

        public ContractController(IContractService contractService, IManagerService managerService)
        {
            _contractService = contractService;
            _managerService = managerService;
        }

        /// <summary>
        /// Số lượng contract cần được ký (Status = Pending)
        /// </summary>
        [Authorize(Roles = "Manager")]
        [HttpGet("number-need-signed")]
        public async Task<IActionResult> GetNumberContractNeedSigned()
        {
            var result = await _managerService.GetNumberContractNeedSignedAsync();
            return Ok(result);
        }

        [HttpGet("{bookingCode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetContractByBookEventCode([FromRoute] string bookingCode)
        {
            if (string.IsNullOrWhiteSpace(bookingCode))
                return BadRequest(new { message = "Booking code không hợp lệ." });

            try
            {
                var contract = await _contractService.GetContractByBookEventCodeAsync(bookingCode);

                if (contract == null)
                    return NotFound(new { message = $"Không tìm thấy hợp đồng với booking code: {bookingCode}" });

                return Ok(contract);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log exception nếu cần
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống.", detail = ex.Message });
            }
        }
    }
}
