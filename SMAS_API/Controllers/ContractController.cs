using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SMAS_BusinessObject.Configurations;
using SMAS_BusinessObject.DTOs.Workflow;
using SMAS_Services.ContractService;
using SMAS_Services.ContractWorkflow;
using SMAS_Services.ManagerServices;
using System.Security.Claims;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/contract")]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;
        private readonly IManagerService _managerService;
        private readonly IContractWorkflowService _contractWorkflowService;
        private readonly IConfiguration _configuration;
        private readonly AppSettings _appSettings;

        public ContractController(
            IContractService contractService,
            IManagerService managerService,
            IContractWorkflowService contractWorkflowService,
            IOptions<AppSettings> appSettings,
            IConfiguration configuration)
        {
            _contractService = contractService;
            _managerService = managerService;
            _contractWorkflowService = contractWorkflowService;
            _configuration = configuration;
            _appSettings = appSettings.Value;
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

        [Authorize(Roles = "Manager")]
        [HttpPost("{id:int}/send-sign")]
        public async Task<IActionResult> SendSign([FromRoute] int id)
        {
            var publicBase = _configuration["App:PublicBaseUrl"] ?? _appSettings.PublicBaseUrl;
            var (dto, status, error) = await _contractWorkflowService.SendContractSignEmailAsync(id, publicBase);
            return status switch
            {
                200 => Ok(dto),
                400 => BadRequest(new { message = error }),
                404 => NotFound(new { message = error }),
                500 => StatusCode(500, new { message = error }),
                _ => StatusCode(status, new { message = error })
            };
        }

        [AllowAnonymous]
        [HttpPost("sign")]
        public async Task<IActionResult> Sign([FromBody] ContractSignRequestDTO request)
        {
            var (dto, status, error) = await _contractWorkflowService.SignContractByTokenAsync(request);
            return status switch
            {
                200 => Ok(dto),
                400 => BadRequest(new { message = error }),
                404 => NotFound(new { message = error }),
                _ => StatusCode(status, new { message = error })
            };
        }

        [Authorize(Roles = "Manager,Cashier")]
        [HttpPost("{id:int}/deposit")]
        public async Task<IActionResult> Deposit([FromRoute] int id, [FromBody] ContractDepositRequestDTO request)
        {
            var staffId = GetUserId();
            if (staffId == null)
                return Unauthorized();

            var apiBase = _configuration["App:PublicBaseUrl"] ?? _appSettings.PublicBaseUrl;
            var (dto, status, error) = await _contractWorkflowService.DepositAsync(id, request, staffId.Value, apiBase);
            return status switch
            {
                200 => Ok(dto),
                400 => BadRequest(new { message = error }),
                404 => NotFound(new { message = error }),
                500 => StatusCode(500, new { message = error }),
                _ => StatusCode(status, new { message = error })
            };
        }

        [AllowAnonymous]
        [HttpGet("{id:int}/deposit/callback")]
        public async Task<IActionResult> DepositCallback(
            [FromRoute] int id,
            [FromQuery] string? status,
            [FromQuery] long? orderCode,
            [FromQuery] string? transactionId)
        {
            var fe = _configuration["App:FrontendBaseUrl"] ?? _appSettings.FrontendBaseUrl;
            var url = await _contractWorkflowService.DepositCallbackRedirectAsync(
                id,
                status ?? "",
                orderCode ?? 0,
                transactionId,
                fe);
            return Redirect(url);
        }

        [AllowAnonymous]
        [HttpPost("{id:int}/deposit/webhook")]
        public async Task<IActionResult> DepositWebhook([FromRoute] int id)
        {
            string rawBody;
            using (var reader = new StreamReader(Request.Body, leaveOpen: true))
            {
                rawBody = await reader.ReadToEndAsync();
                Request.Body.Position = 0;
            }

            var sig = Request.Headers["x-payos-signature"].FirstOrDefault();
            await _contractWorkflowService.DepositWebhookAsync(id, rawBody, sig);
            return Ok();
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
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống.", detail = ex.Message });
            }
        }

        private int? GetUserId()
        {
            var v = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(v, out var uid) ? uid : null;
        }
    }
}
