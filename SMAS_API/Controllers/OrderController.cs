using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.OrderDTO;
using SMAS_Services.OrderServices;
using SMAS_Services.TableService;
using System.Security.Claims;

namespace SMAS_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/order")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ITableSessionService _tableSessionService;

        public OrderController(IOrderService orderService, ITableSessionService tableSessionService)
        {
            _orderService = orderService;
            _tableSessionService = tableSessionService;
        }

        [Authorize(Roles = "Admin,Manager,Customer")]
        [HttpPost("filter")]
        public async Task<IActionResult> GetOrdersByStatus([FromQuery] OrderListStatusRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var result = await _orderService
                .GetOrdersByUserAndStatusAsync(request , userId);
            if (result == null || !result.Any())
            {
                return NotFound(new { MsgCode = "MSG_021", Message = "Không có đơn hàng nào !" });
            }

            return Ok(result);
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("create/delivery")]

        //Cai nay bua sau bo vo truoc phan create order
        public async Task<ActionResult<OrderDeliveryResponse>> CreateOrder([FromBody] CreateOrderDeliveryRequest request)
        {

            var accessToken = Request.Headers["X-Table-Token"].FirstOrDefault();

            if (string.IsNullOrEmpty(accessToken))
                return Unauthorized(new { errorCode = "INVALID_QR_TOKEN" });

            (bool valid, string? errorCode, string? tableCode) = _tableSessionService.ValidateAccessToken(accessToken);
            if (!valid)
                return BadRequest(new { errorCode });
        //phan o giua nhe

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();
            var result = await _orderService.CreateOrderDeliveryAsync(request, userId);
            if (result.Success == false)
            {
                return BadRequest(new { MsgCode = 29, Message = result.Message });
            }
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("active")]
        public async Task<IActionResult> GetAllActiveOrder()
        {
            try
            {
                var orders = await _orderService.GetAllActiveOrderAsync();

                if (orders == null || orders.Count == 0)
                    return Ok(new { data = (object?)null, message = "Không có đơn hàng nào đang hoạt động." });

                return Ok(new { data = orders, message = "Lấy danh sách đơn hàng thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống.", detail = ex.Message });
            }
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("history")]
        public async Task<IActionResult> GetAllOrderCompleteAndCancel()
        {
            try
            {
                var orders = await _orderService.GetAllOrderCompleteAndCancelAsync();

                if (orders == null || orders.Count == 0)
                    return Ok(new { data = (object?)null, message = "Không có đơn hàng đã hoàn thành hoặc đã huỷ." });

                return Ok(new { data = orders, message = "Lấy danh sách đơn hàng thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống.", detail = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("active/type")]
        public async Task<IActionResult> GetAllActiveOrderByOrderType([FromQuery] string orderType)
        {
            if (string.IsNullOrWhiteSpace(orderType))
                return BadRequest(new { message = "OrderType không được để trống." });

            try
            {
                var orders = await _orderService.GetAllActiveOrderByOrderTypeAsync(orderType);

                if (orders == null || orders.Count == 0)
                    return Ok(new { data = (object?)null, message = $"Không có đơn hàng active với loại: {orderType}" });

                return Ok(new { data = orders, message = "Lấy danh sách đơn hàng thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống.", detail = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,Manager,Customer")]
        [HttpGet("{orderCode}")]
        public async Task<IActionResult> GetOrderDetailByOrderCode([FromRoute] string orderCode)
        {
            try
            {
                var order = await _orderService.GetOrderDetailByOrderCodeAsync(orderCode);

                if (order == null)
                    return Ok(new { data = (object?)null, message = $"Không tìm thấy đơn hàng với mã: {orderCode}" });

                return Ok(new { data = order, message = "Lấy thông tin đơn hàng thành công." });
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

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("history/type")]
        public async Task<IActionResult> GetAllOrderCompleteAndCancelByOrderType([FromQuery] string orderType)
        {
            if (string.IsNullOrWhiteSpace(orderType))
                return BadRequest(new { message = "OrderType không được để trống." });

            try
            {
                var orders = await _orderService.GetAllOrderCompleteAndCancelByOrderTypeAsync(orderType);

                if (orders == null || orders.Count == 0)
                    return Ok(new { data = (object?)null, message = $"Không có đơn hàng với loại: {orderType}" });

                return Ok(new { data = orders, message = "Lấy danh sách đơn hàng thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống.", detail = ex.Message });
            }
        }

    [Authorize(Roles = "Admin,Manager,Staff")]
    [HttpPost("{orderCode}/items")]
     public async Task<IActionResult> PostAddOrderItemByOrderCode(
    [FromRoute] string orderCode,
    [FromBody] AddOrderItemRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _orderService.AddOrderItemByOrderCodeAsync(orderCode, request);

                if (!result.Success)
                    return result.Message.Contains("Không tìm thấy")
                        ? NotFound(new { message = result.Message })
                        : BadRequest(new { message = result.Message });

                return Ok(new { data = result, message = result.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống.", detail = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,Waiter,Kitchen,Manager")]
        [HttpPost("delivery/fail")]
        public async Task<IActionResult> UpdateStatusDeliveryAsync([FromBody] FailDeliveryRequestDTO request)
        {
            if (ModelState.IsValid) return BadRequest(ModelState);
            var result = await _orderService.UpdateOrderDeliveryFailedAtAsync(request);
            return Ok(result);
        }

    }
}
