using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.OrderDTO;
using SMAS_Services.OrderItemServices;
using SMAS_Services.OrderServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api")]
    public class OrderItemController : Controller
    {
        private readonly IOrderItemService _orderItemService;

        public OrderItemController(IOrderItemService orderItemService )
        {
            _orderItemService = orderItemService;
        }

        [Authorize(Roles = "Kitchen")]
        [HttpGet("order-items/pending")]
        public async Task<IActionResult> GetActiveOrdersWithPendingItemsAsync()
        {
            var result = await _orderItemService.GetActiveOrdersWithPendingItemsAsync();
            return Ok(result);
        }

        [Authorize(Roles = "Kitchen")]
        [HttpPatch("order-items/{orderItemId}/preparing")]
        public async Task<IActionResult> PatchUpdateStatusOrderItemPreparingAsync([FromRoute] int orderItemId)
        {
            try
            {
                var result = await _orderItemService.PatchUpdateStatusOrderItemPreparingAsync(orderItemId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return StatusCode(404, new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(400, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống.", detail = ex.Message });
            }
        }

        [Authorize(Roles = "Kitchen")]
        [HttpPatch("order-items/{orderItemId}/ready")]
        public async Task<IActionResult> PatchUpdateStatusOrderItemReadyAsync([FromRoute] int orderItemId)
        {
            try
            {
                var result = await _orderItemService.PatchUpdateStatusOrderItemReadyAsync(orderItemId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return StatusCode(404, new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(400, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống.", detail = ex.Message });
            }
        }

        [Authorize(Roles = "Kitchen")]
        [HttpPost("order-items/{orderItemId}/cancel")]
        public async Task<IActionResult> PostUpdateStatusOrderItemCancelledAsync(
            [FromRoute] int orderItemId,
            [FromBody] KitchenCancelOrderItemRequestDTO request)
        {
            try
            {
                var result = await _orderItemService.PostUpdateStatusOrderItemCancelledAsync(orderItemId, request);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return StatusCode(404, new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(400, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống.", detail = ex.Message });
            }
        }

        [Authorize(Roles = "Kitchen")]
        [HttpPatch("orders/{orderId}/items/all-preparing")]
        public async Task<IActionResult> PatchUpdateStatusAllOrderItemPreparingAsync([FromRoute] int orderId)
        {
            try
            {
                var result = await _orderItemService.PatchUpdateStatusAllOrderItemPreparingAsync(orderId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return StatusCode(404, new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(400, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống.", detail = ex.Message });
            }
        }

        [Authorize(Roles = "Kitchen")]
        [HttpPatch("orders/{orderId}/items/all-ready")]
        public async Task<IActionResult> PatchUpdateStatusAllOrderItemReadyAsync([FromRoute] int orderId)
        {
            try
            {
                var result = await _orderItemService.PatchUpdateStatusAllOrderItemReadyAsync(orderId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return StatusCode(404, new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(400, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống.", detail = ex.Message });
            }
        }

        [Authorize(Roles = "Kitchen")]
        [HttpGet("order-items/history/today")]
        public async Task<IActionResult> GetAllOrderItemsHistoryTodayAsync([FromQuery] int? orderId)
        {
            try
            {
                var result = await _orderItemService.GetAllOrderItemsHistoryTodayAsync(orderId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return StatusCode(404, new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(400, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống.", detail = ex.Message });
            }
        }

        [Authorize(Roles = "Waiter")]
        [HttpPatch("order-items/{orderItemId}/Served")]
        public async Task<IActionResult> PatchUpdateStatusOrderItemServedAsync([FromRoute] int orderItemId)
        {
            try
            {
                var result = await _orderItemService.PatchUpdateStatusOrderItemServedAsync(orderItemId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return StatusCode(404, new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(400, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống.", detail = ex.Message });
            }
        }

        //[Authorize(Roles = "Manager,Waiter,Admin")]
        [HttpGet("getfoods-buffer-{orderCode}")]
        public async Task<IActionResult> GetFoodBufferByOrderCodeAsync([FromRoute] string orderCode)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var result = await _orderItemService.GetFoodForBufferAsync(orderCode);

                if (result == null)
                    return NotFound("Không tìm thấy bufer nào đã mua ");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống.", detail = ex.Message });
            }
        }

        //[Authorize(Roles = "Manager,Waiter,Admin")]
        [HttpPost("add-{orderCode}-items")]
        public async Task<IActionResult> PostAddOrderItemByOrderCodeAsync([FromRoute] string orderCode, [FromBody] List<AddOrderItemRequest> request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (request == null || request.Count == 0)
                return BadRequest(new { message = "Danh sách món ăn không được để trống." });
            var countbuffer = 0;
            foreach (var item in request)
            {
                if (item.FoodId == null && item.ComboId == null && item.BuffetId == null)
                    return BadRequest(new { message = "Phải chọn ít nhất một loại item (FoodId, ComboId hoặc BuffetId)." });
                if (item.Quantity < 1 || item.Quantity > 10)
                    return BadRequest(new { message = "Số lượng phải từ 1 đến 30." });
                if (!string.IsNullOrEmpty(item.Note) && item.Note.Length > 500)
                    return BadRequest(new { message = "Ghi chú không được quá 500 ký tự." });
                if (item.BuffetId.HasValue)
                    countbuffer++;
            }
            if (countbuffer > 1)
                return BadRequest(new { message = "Không thể thêm nhiều loại  buffet vào 1 đơn hàng ." });
            try
            {
                var result = await _orderItemService.AddOrderItemByOrderCodeAsync(orderCode, request);

                if (!result.status)
                    return BadRequest(result.message);

                return Ok(result.message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống.", detail = ex.Message });
            }
        }
    }
}

