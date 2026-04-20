using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SMAS_BusinessObject.DTOs.ManagerDTO;
using SMAS_BusinessObject.DTOs.OrderDTO;
using SMAS_Services.ManagerServices;
using SMAS_Services.OrderServices;
using SMAS_Services.TableService;
using System.Security.Claims;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ITableService _tableSessionService;
        private readonly IManagerService _managerService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ITableService tableSessionService, IManagerService managerService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _tableSessionService = tableSessionService;
            _managerService = managerService;
            _logger = logger;
        }

        private IActionResult HandleOrderExceptions(Exception ex)
        {
            return ex switch
            {
                ArgumentException arg => BadRequest(new { message = arg.Message }),
                KeyNotFoundException knf => NotFound(new { message = knf.Message }),
                UnauthorizedAccessException _ => Unauthorized(new { message = "Unauthorized" }),
                _ => StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống.", detail = ex.Message })
            };
        }

        /// <summary>
        /// Lấy danh sách đơn hàng được tạo trong ngày hôm nay
        /// </summary>
        [Authorize(Roles = "Manager")]
        [HttpGet("orders-today")]
        public async Task<IActionResult> GetOrderToday()
        {
            var result = await _managerService.GetOrdersTodayAsync();

            if (result == null || !result.Any())
                return Ok(new
                {
                    data = Array.Empty<object>(),
                    message = "Hôm nay chưa có đơn hàng nào."
                });

            return Ok(new
            {
                data = result,
                message = $"Có {result.Count()} đơn hàng hôm nay."
            });
        }

        /// <summary>
        /// Lấy tổng doanh thu 7 ngày gần nhất (theo tuần)
        /// </summary>
        [Authorize(Roles = "Manager")]
        [HttpGet("revenue-previous-seven-days")]
        public async Task<IActionResult> GetRevenuePreviousSevenDay()
        {
            var result = await _managerService.GetRevenuePreviousSevenDaysAsync();
            return Ok(result);
        }

        /// <summary>
        /// Lấy 4 đơn hàng mới tạo gần nhất
        /// </summary>
        [Authorize(Roles = "Manager")]
        [HttpGet("four-newest-orders")]
        public async Task<IActionResult> GetFourNewCreateOrder()
        {
            var result = await _managerService.GetFourNewestOrdersAsync();
            return Ok(result);
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
                .GetOrdersByUserAndStatusAsync(request, userId);
            if (result == null || !result.Any())
            {
                return NotFound(new { MsgCode = "MSG_021", Message = "Không có đơn hàng nào !" });
            }

            return Ok(result);
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("create/delivery")]

        public async Task<ActionResult<OrderDeliveryResponse>> CreateOrder([FromBody] CreateOrderDeliveryRequest request)
        {

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

        [Authorize(Roles = "Admin,Manager")]
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

        [Authorize(Roles = "Customer")]
        [HttpGet("history/my")]
        public async Task<IActionResult> GetAllOrderCompleteAndCancelByCustomerId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            try
            {
                var orders = await _orderService.GetAllOrderCompleteAndCancelByCustomerIdAsync(userId);

                if (orders == null || !orders.Any())
                    return Ok(new { MsgCode = "MSG_021", Message = "Bạn chưa có đơn hàng nào hoàn thành hoặc đã huỷ.", Data = orders });

                return Ok(new { MsgCode = "MSG_000", Message = "Lấy lịch sử đơn hàng thành công.", Data = orders });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { MsgCode = "MSG_500", Message = "Đã xảy ra lỗi hệ thống.", Detail = ex.Message });
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


        [HttpPost("{orderCode}/items")]
        [AllowAnonymous]
        public async Task<IActionResult> PostAddOrderItemByOrderCode(
        [FromRoute] string orderCode,
        [FromBody] AddOrderItemRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var authHeader = Request.Headers.Authorization.ToString();

                if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new
                    {
                        errorCode = "MISSING_TABLE_TOKEN",
                        message = "Vui lòng quét mã QR bàn trước khi đặt món."
                    });
                }

                var accessToken = authHeader.Substring("Bearer ".Length).Trim();

                var result = await _orderService.AddOrderItemByTableTokenAsync(orderCode, request, accessToken);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new
                {
                    success = true,
                    message = result.Message,
                    data = new { newTotalAmount = result.NewTotalAmount }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi thêm món vào order {OrderCode}", orderCode);   // Nếu _logger chưa có, inject nó
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống." });
            }
        }

        [Authorize(Roles = "Admin,Waiter,Kitchen,Manager")]
        [HttpPost("delivery/fail")]
        public async Task<IActionResult> UpdateStatusDeliveryAsync([FromBody] FailDeliveryRequestDTO request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _orderService.UpdateOrderDeliveryFailedAtAsync(request);
            return Ok(result);
        }

        [Authorize(Roles = "Waiter")]
        [HttpGet("preparing/my")]
        public async Task<IActionResult> GetAllOrderPreparingByJwtWaiterIdAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            try
            {
                var orders = await _orderService.GetAllOrderPreparingByWaiterIdAsync(userId);

                if (orders == null || !orders.Any())
                    return Ok(new { MsgCode = "MSG_021", Message = "Không có đơn hàng nào đang xử lý.", Data = orders });

                return Ok(new { MsgCode = "MSG_000", Message = "Lấy danh sách đơn hàng thành công.", Data = orders });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { MsgCode = "MSG_500", Message = "Đã xảy ra lỗi hệ thống.", Detail = ex.Message });
            }
        }
        [Authorize(Roles = "Waiter")]
        [HttpGet("delivery/my")]
        public async Task<IActionResult> GetAllOrderDeliveryByJwtWaiterIdAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            try
            {
                var orders = await _orderService.GetAllOrderDeliveryByWaiterIdAsync(userId);

                if (orders == null || !orders.Any())
                    return Ok(new { MsgCode = "MSG_021", Message = "Không có đơn giao hàng nào đang xử lý.", Data = orders });

                return Ok(new { MsgCode = "MSG_000", Message = "Lấy danh sách đơn giao hàng thành công.", Data = orders });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { MsgCode = "MSG_500", Message = "Đã xảy ra lỗi hệ thống.", Detail = ex.Message });
            }
        }

        [Authorize(Roles = "Waiter")]
        [HttpGet("history/my/seven-days")]
        public async Task<IActionResult> GetAllOrderHistoryByJwtWaiterIdInSevenDayAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            try
            {
                var orders = await _orderService.GetAllOrderHistoryByWaiterIdInSevenDayAsync(userId);

                if (orders == null || !orders.Any())
                    return Ok(new { MsgCode = "MSG_021", Message = "Không có lịch sử đơn hàng nào trong 7 ngày qua.", Data = orders });

                return Ok(new { MsgCode = "MSG_000", Message = "Lấy lịch sử đơn hàng thành công.", Data = orders });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { MsgCode = "MSG_500", Message = "Đã xảy ra lỗi hệ thống.", Detail = ex.Message });
            }
        }
        [Authorize(Roles = "Waiter,Manager,Admin")]
        [HttpPost("/api/orders/lookup")]
        public async Task<IActionResult> Lookup([FromBody] OrderLookupRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _orderService.LookupOrderAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleOrderExceptions(ex);
            }
        }

        [Authorize(Roles = "Waiter,Manager,Admin")]
        [HttpPost("/api/orders/by-reservation")]
        public async Task<IActionResult> CreateOrderByReservation([FromBody] CreateOrderByReservationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int waiterUserId))
                    throw new UnauthorizedAccessException("Unauthorized");

                var result = await _orderService.CreateOrderByReservationAsync(request, waiterUserId);
                return StatusCode(StatusCodes.Status201Created, result);
            }
            catch (Exception ex)
            {
                return HandleOrderExceptions(ex);
            }
        }

        [Authorize(Roles = "Waiter,Manager,Admin")]
        [HttpPost("/api/orders/by-contact")]
        public async Task<IActionResult> CreateOrderByContact([FromBody] CreateOrderByContactRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int waiterUserId))
                    throw new UnauthorizedAccessException("Unauthorized");

                var result = await _orderService.CreateOrderByContactAsync(request, waiterUserId);
                return StatusCode(StatusCodes.Status201Created, result);
            }
            catch (Exception ex)
            {
                return HandleOrderExceptions(ex);
            }
        }

        [Authorize(Roles = "Waiter,Manager,Admin")]
        [HttpPost("/api/orders/guest")]
        public async Task<IActionResult> CreateGuestOrder([FromBody] CreateGuestOrderRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int waiterUserId))
                    throw new UnauthorizedAccessException("Unauthorized");

                var result = await _orderService.CreateGuestOrderAsync(request, waiterUserId);
                return StatusCode(StatusCodes.Status201Created, result);
            }
            catch (Exception ex)
            {
                return HandleOrderExceptions(ex);
            }
        }


        [Authorize(Roles = "Waiter,Manager,Kitchen,Admin")]
        [HttpPost("choose-staffid-delivery")]
        public async Task<IActionResult> ChooseAssignedStaffbyOrderAsync([FromBody] ChooseAssignedStaffRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var result = await _orderService.ChooseAssignedStaffbyOrderAsync(request);
                if (!result.status)
                {
                    return BadRequest(result.message);
                }
                return Ok(result.message);
            }
            catch (Exception ex)
            {
                return HandleOrderExceptions(ex);
            }
        }
        [Authorize(Roles = "Waiter,Manager,Kitchen,Admin")]
        [HttpPatch("change-status/{OrderCode}")]
        public async Task<IActionResult> ChangeStatusDeliveryAsync([FromRoute] string OrderCode)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var result = await _orderService.ChangeStatusDeliveryAsync(OrderCode);
                if (!result.status)
                {
                    return BadRequest(result.message);
                }
                return Ok(result.message);
            }
            catch (Exception ex)
            {
                return HandleOrderExceptions(ex);
            }
        }
        [Authorize(Roles = "Waiter,Manager,Kitchen,Admin")]
        [HttpPost("delete-orderdelivery/{OrderCode}")]
        public async Task<IActionResult> DeleteOrderDeliveryByDeliveryCodeAsync(string OrderCode, [FromBody] CancelReservationRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(OrderCode))
                return BadRequest("DeliveryCode không hợp lệ.");
            if (dto == null || string.IsNullOrWhiteSpace(dto.CancellationReason))
                return BadRequest("Lý do hủy là bắt buộc.");
            var result = await _orderService.DeleteOrderDeliveryByDeliveryCodeAsync(OrderCode, dto.CancellationReason);
            if (!result.status)
            {
                return BadRequest(result.message);
            }
            return Ok(result.message);
        }


        /// Khách quét QR → lấy menu (food/combo/buffet) trong session bàn.
        /// Header yêu cầu: Authorization: Bearer {tableAccessToken}
        /// Query: type=food|combo|buffet|all, categoryId, keyword
        [HttpGet("session/menu")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSessionMenu(
            [FromQuery] string? type,
            [FromQuery] int? categoryId,
            [FromQuery] string? keyword)
        {
            try
            {
                var authHeader = Request.Headers.Authorization.ToString();
                if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new
                    {
                        errorCode = "MISSING_TABLE_TOKEN",
                        message = "Vui lòng quét mã QR bàn trước khi xem thực đơn."
                    });
                }

                var accessToken = authHeader.Substring("Bearer ".Length).Trim();

                var (success, errorCode, data) = await _orderService
                    .GetMenuForSessionAsync(accessToken, type, categoryId, keyword);

                if (!success)
                {
                    return errorCode switch
                    {
                        "INVALID_QR_TOKEN" => Unauthorized(new { errorCode, message = "Token bàn không hợp lệ." }),
                        "SESSION_NOT_ACTIVE" => BadRequest(new { errorCode, message = "Phiên bàn không hoạt động." }),
                        "SESSION_EXPIRED" => BadRequest(new { errorCode, message = "Phiên bàn đã hết hạn." }),
                        "TABLE_CLOSED" => BadRequest(new { errorCode, message = "Bàn đã đóng." }),
                        _ => BadRequest(new { errorCode })
                    };
                }

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lấy session menu.");
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống." });
            }
        }
        /// Khách quét QR → lấy chi tiết order hiện tại của bàn (giỏ hàng bên phải).
        /// Header: Authorization: Bearer {tableAccessToken}
        /// </summary>
        [HttpGet("session/current")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSessionCurrentOrder()
        {
            try
            {
                var authHeader = Request.Headers.Authorization.ToString();
                if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new
                    {
                        errorCode = "MISSING_TABLE_TOKEN",
                        message = "Vui lòng quét mã QR bàn trước."
                    });
                }

                var accessToken = authHeader.Substring("Bearer ".Length).Trim();

                var (success, errorCode, data) = await _orderService
                    .GetCurrentOrderBySessionAsync(accessToken);

                if (!success)
                {
                    return errorCode switch
                    {
                        "INVALID_QR_TOKEN" => Unauthorized(new { errorCode, message = "Token bàn không hợp lệ." }),
                        "SESSION_NOT_ACTIVE" => BadRequest(new { errorCode, message = "Phiên bàn không hoạt động." }),
                        "SESSION_EXPIRED" => BadRequest(new { errorCode, message = "Phiên bàn đã hết hạn." }),
                        "TABLE_CLOSED" => BadRequest(new { errorCode, message = "Bàn đã đóng." }),
                        "NO_ACTIVE_ORDER" => Ok(new { success = true, data = (object?)null, message = "Chưa có đơn hàng cho bàn này." }),
                        _ => BadRequest(new { errorCode })
                    };
                }

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lấy session current order.");
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống." });
            }
        }
        [Authorize(Roles = "Waiter,Manager,Admin,Customer")]
        [HttpPost("{ordercode}/apply-discount/{discountcode}")]
        public async Task<IActionResult> AddDiscountToOrderAsync(string ordercode, string discountcode)
        {
            var success = await _orderService.AddDiscountToOrderAsync(ordercode, discountcode);
            if (!success.status)
                return NotFound(success.message);
            return Ok(success.message);
        }
    }
}
