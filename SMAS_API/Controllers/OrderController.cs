using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.OrderDTO;
using SMAS_Services.ManagerServices;
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
        private readonly ITableService _tableSessionService;
        private readonly IManagerService _managerService;

        public OrderController(IOrderService orderService, ITableService tableSessionService, IManagerService managerService)
        {
            _orderService = orderService;
            _tableSessionService = tableSessionService;
            _managerService = managerService;
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
            return Ok(result);
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
                .GetOrdersByUserAndStatusAsync(request , userId);
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
        [Authorize(Roles = "Waiter,Manager")]
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

        [Authorize(Roles = "Waiter,Manager")]
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

        [Authorize(Roles = "Waiter,Manager")]
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

        [Authorize(Roles = "Waiter,Manager")]
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

    }
}
