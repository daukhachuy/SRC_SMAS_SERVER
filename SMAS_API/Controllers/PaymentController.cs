using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.PayOSDTO;
using SMAS_Services.PaymentServices;
using System.Security.Claims;

namespace SMAS_API.Controllers;

[ApiController]
[Route("api/payment")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary>
    /// Tạo link thanh toán PayOS cho đơn hàng đã tạo (trạng thái Pending).
    /// Frontend gọi sau khi POST create order thành công, dùng OrderId trả về.
    /// </summary>
    [Authorize(Roles = "Customer")]
    [HttpPost("create-link")]
    public async Task<ActionResult<CreatePaymentLinkResponse>> CreatePaymentLink([FromBody] CreatePaymentLinkRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        var result = await _paymentService.CreatePaymentLinkAsync(request, userId);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Webhook PayOS gọi khi có kết quả thanh toán. Không dùng JWT.
    /// Đăng ký URL này trên https://my.payos.vn (Confirm webhook).
    /// </summary>
    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook()
    {
        using var reader = new StreamReader(Request.Body);
        var rawBody = await reader.ReadToEndAsync();
        var handled = await _paymentService.HandleWebhookAsync(rawBody);
        return handled ? Ok() : (IActionResult)BadRequest();
    }

    [Authorize(Roles = "Admin,Waiter,Kitchen,Manager")]
    [HttpPost("payment-order-cash")]
    public async Task<IActionResult> CreatePaymentOrderCashAsync([FromBody] PaymentOrderCashRequestDTO request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        var result = await _paymentService.CreatePaymentOrderCashAsync(request, userId);
        if (!result.status)
            return BadRequest(result.message);

        return Ok(result.message);
    }

}
