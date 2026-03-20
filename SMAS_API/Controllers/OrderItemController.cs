using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.OrderDTO;
using SMAS_Services.OrderItemServices;
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

        public OrderItemController(IOrderItemService orderItemService)
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
    }
}

