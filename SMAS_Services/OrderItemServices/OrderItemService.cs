using SMAS_BusinessObject.DTOs.Food;
using SMAS_BusinessObject.DTOs.OrderDTO;
using SMAS_BusinessObject.Models;
using SMAS_Repositories.OrderRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMAS_Services.OrderItemServices
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IOrderItemRepository _orderItemRepository;

        private readonly IOrderRepository _orderRepository;

        public OrderItemService(IOrderItemRepository orderItemRepository, IOrderRepository orderRepository)
        {
            _orderItemRepository = orderItemRepository;
            _orderRepository = orderRepository;
        }

        // 1) ValidateOrderActive(int orderId)
        //    - throw KeyNotFoundException -> controller maps to 404
        //    - throw InvalidOperationException -> controller maps to 400
        private async Task<Order> ValidateOrderActive(int orderId)
        {
            var order = await _orderItemRepository.GetOrderByIdAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException("Order not found");

            if (order.OrderStatus == "Cancelled" || order.OrderStatus == "Closed")
                throw new InvalidOperationException("Cannot update item of a closed or cancelled order");

            return order;
        }

        // 2) GetItemName(OrderItem item)
        private static string GetItemName(OrderItem item)
        {
            if (item.Food != null)
                return item.Food.Name;

            if (item.Buffet != null)
                return item.Buffet.Name;

            if (item.Combo != null)
                return item.Combo.Name;

            return string.Empty;
        }

        public async Task<List<KitchenPendingOrderDTO>> GetActiveOrdersWithPendingItemsAsync()
        {
            var orders = await _orderItemRepository.GetActiveOrdersWithPendingItemsAsync();

            // Lọc ra chỉ những Order có ít nhất 1 OrderItem Pending
            orders = orders
                .Where(o => o.OrderItems != null && o.OrderItems.Any(oi => oi.Status == "Pending" || oi.Status == "Preparing" || oi.Status == "Ready"))
                .ToList();

            var result = orders
                .Select(o =>
                {
                    var pendingItems = o.OrderItems
                        .OrderBy(oi => oi.OpeningTime ?? DateTime.MaxValue)
                        .Select(oi => new KitchenPendingItemDTO
                        {
                            OrderItemId = oi.OrderItemId,
                            FoodId = oi.FoodId,
                            BuffetId = oi.BuffetId,
                            ComboId = oi.ComboId,
                            ItemName = GetItemName(oi),
                            Quantity = oi.Quantity,
                            Note = oi.Note,
                            OpeningTime = oi.OpeningTime,
                            Status = oi.Status ?? string.Empty
                        })
                        .ToList();

                    var oldestOpening = pendingItems.FirstOrDefault()?.OpeningTime ?? DateTime.MaxValue;

                    return new { Order = o, PendingItems = pendingItems, OldestOpening = oldestOpening };
                })
                .OrderBy(x => x.OldestOpening)
                .Select(x => new KitchenPendingOrderDTO
                {
                    OrderId = x.Order.OrderId,
                    OrderCode = x.Order.OrderCode ?? string.Empty,
                    TableId = x.Order.TableOrders?.FirstOrDefault(to => to.IsMainTable == true)?.TableId ?? 0,
                    OrderCreatedAt = x.Order.CreatedAt,
                    PendingItems = x.PendingItems
                })
                .ToList();

            return result;
        }

        public async Task<KitchenOrderItemPreparingResponseDTO> PatchUpdateStatusOrderItemPreparingAsync(int orderItemId)
        {
            try
            {
                var orderItem = await _orderItemRepository.GetOrderItemWithOrderAndNamesAsync(orderItemId);
                if (orderItem == null)
                    throw new KeyNotFoundException("Order item not found");

                // gọi GetItemName để dùng chung helper (theo requirement)
                _ = GetItemName(orderItem);

                await ValidateOrderActive(orderItem.OrderId);

                var currentStatus = orderItem.Status;
                if (currentStatus != "Pending")
                    throw new InvalidOperationException($"Order item is not in Pending status. Current status: {currentStatus}");

                var updatedAt = DateTime.UtcNow;
                await _orderItemRepository.UpdatePreparingAsync(orderItemId);

                return new KitchenOrderItemPreparingResponseDTO
                {
                    OrderItemId = orderItem.OrderItemId,
                    OrderId = orderItem.OrderId,
                    Status = "Preparing",
                    UpdatedAt = updatedAt
                };
            }
            catch
            {
                throw;
            }
        }

        public async Task<KitchenOrderItemReadyResponseDTO> PatchUpdateStatusOrderItemReadyAsync(int orderItemId)
        {
            try
            {
                var orderItem = await _orderItemRepository.GetOrderItemWithOrderAndNamesAsync(orderItemId);
                if (orderItem == null)
                    throw new KeyNotFoundException("Order item not found");

                _ = GetItemName(orderItem);

                await ValidateOrderActive(orderItem.OrderId);

                var currentStatus = orderItem.Status;
                if (currentStatus != "Preparing")
                    throw new InvalidOperationException($"Order item is not in Preparing status. Current status: {currentStatus}");

                var servedTime = DateTime.UtcNow;
                await _orderItemRepository.UpdateReadyAsync(orderItemId, servedTime);

                return new KitchenOrderItemReadyResponseDTO
                {
                    OrderItemId = orderItem.OrderItemId,
                    OrderId = orderItem.OrderId,
                    Status = "Ready",
                    ServedTime = servedTime
                };
            }
            catch
            {
                throw;
            }
        }

        public async Task<KitchenOrderItemReadyResponseDTO> PatchUpdateStatusOrderItemServedAsync(int orderItemId)
        {
            try
            {
                var orderItem = await _orderItemRepository.GetOrderItemWithOrderAndNamesAsync(orderItemId);
                if (orderItem == null)
                    throw new KeyNotFoundException("Order item not found");

                _ = GetItemName(orderItem);

                await ValidateOrderActive(orderItem.OrderId);

                var currentStatus = orderItem.Status;
                if (currentStatus != "Ready")
                    throw new InvalidOperationException($"Order item is not in Served status. Current status: {currentStatus}");

                var servedTime = DateTime.UtcNow;
                await _orderItemRepository.UpdateServedAsync(orderItemId, servedTime);

                return new KitchenOrderItemReadyResponseDTO
                {
                    OrderItemId = orderItem.OrderItemId,
                    OrderId = orderItem.OrderId,
                    Status = "Served",
                    ServedTime = servedTime
                };
            }
            catch
            {
                throw;
            }
        }

        public async Task<KitchenOrderItemCancelledResponseDTO> PostUpdateStatusOrderItemCancelledAsync(int orderItemId, KitchenCancelOrderItemRequestDTO request)
        {
            try
            {
                var reason = request?.Reason;
                if (string.IsNullOrWhiteSpace(reason))
                    throw new InvalidOperationException("Cancellation reason is required");

                var orderItem = await _orderItemRepository.GetOrderItemWithOrderAndNamesAsync(orderItemId);
                if (orderItem == null)
                    throw new KeyNotFoundException("Order item not found");

                _ = GetItemName(orderItem);

                try
                {
                    await ValidateOrderActive(orderItem.OrderId);
                }
                catch (InvalidOperationException)
                {
                    // message theo đúng Prompt 4
                    throw new InvalidOperationException("Cannot cancel item of a closed or cancelled order");
                }

                var currentStatus = orderItem.Status;
                if (currentStatus != "Pending")
                    throw new InvalidOperationException($"Only Pending items can be cancelled. Current status: {currentStatus}");

                var newNote = string.IsNullOrEmpty(orderItem.Note)
                    ? $"[Kitchen cancelled] {reason}"
                    : $"{orderItem.Note} | [Kitchen cancelled] {reason}";

                var newSubTotal = await _orderItemRepository.CancelItemAndRecalculateOrderTotalsAsync(orderItemId, newNote);

                return new KitchenOrderItemCancelledResponseDTO
                {
                    OrderItemId = orderItem.OrderItemId,
                    OrderId = orderItem.OrderId,
                    Status = "Cancelled",
                    Subtotal = 0m,
                    Note = newNote,
                    NewOrderSubTotal = newSubTotal,
                    NewOrderTotalAmount = newSubTotal
                };
            }
            catch
            {
                throw;
            }
        }

        public async Task<KitchenUpdateAllPreparingResponseDTO> PatchUpdateStatusAllOrderItemPreparingAsync(int orderId)
        {
            try
            {
                var order = default(Order);
                try
                {
                    order = await ValidateOrderActive(orderId);
                }
                catch (InvalidOperationException)
                {
                    // message theo đúng Prompt 5
                    throw new InvalidOperationException("Cannot update items of a closed or cancelled order");
                }

                var pendingItems = await _orderItemRepository.GetOrderItemsByStatusWithNamesAsync(orderId, "Pending");

                if (!pendingItems.Any())
                    throw new InvalidOperationException("No pending items found for this order");

                _ = pendingItems.Select(GetItemName).FirstOrDefault();

                await _orderItemRepository.UpdateAllPendingToPreparingAsync(orderId);

                return new KitchenUpdateAllPreparingResponseDTO
                {
                    OrderId = orderId,
                    OrderCode = order.OrderCode ?? string.Empty,
                    UpdatedCount = pendingItems.Count,
                    UpdatedItems = pendingItems
                        .Select(i => new KitchenUpdatedPreparingItemDTO
                        {
                            OrderItemId = i.OrderItemId,
                            Status = "Preparing"
                        })
                        .ToList()
                };
            }
            catch
            {
                throw;
            }
        }

        public async Task<KitchenUpdateAllReadyResponseDTO> PatchUpdateStatusAllOrderItemReadyAsync(int orderId)
        {
            try
            {
                var order = default(Order);
                try
                {
                    order = await ValidateOrderActive(orderId);
                }
                catch (InvalidOperationException)
                {
                    // message theo đúng Prompt 6
                    throw new InvalidOperationException("Cannot update items of a closed or cancelled order");
                }

                var preparingItems = await _orderItemRepository.GetOrderItemsByStatusWithNamesAsync(orderId, "Preparing");

                if (!preparingItems.Any())
                    throw new InvalidOperationException("No preparing items found for this order");

                var servedTime = DateTime.UtcNow;
                _ = preparingItems.Select(GetItemName).FirstOrDefault();

                await _orderItemRepository.UpdateAllPreparingToReadyAsync(orderId, servedTime);

                return new KitchenUpdateAllReadyResponseDTO
                {
                    OrderId = orderId,
                    OrderCode = order.OrderCode ?? string.Empty,
                    UpdatedCount = preparingItems.Count,
                    UpdatedItems = preparingItems
                        .Select(i => new KitchenUpdatedReadyItemDTO
                        {
                            OrderItemId = i.OrderItemId,
                            Status = "Ready",
                            ServedTime = servedTime
                        })
                        .ToList()
                };
            }
            catch
            {
                throw;
            }
        }

        public async Task<KitchenTodayHistoryResponseDTO> GetAllOrderItemsHistoryTodayAsync(int? orderId)
        {
            var startOfDayUtc = DateTime.UtcNow.Date;
            var endOfDayUtc = startOfDayUtc.AddDays(1);

            var items = await _orderItemRepository.GetReadyOrderItemsHistoryTodayAsync(startOfDayUtc, endOfDayUtc, orderId);

            var mappedItems = items.Select(oi => new KitchenTodayHistoryItemDTO
            {
                OrderItemId = oi.OrderItemId,
                OrderId = oi.OrderId,
                OrderCode = oi.Order.OrderCode ?? string.Empty,
                TableId = oi.Order.TableOrders?.FirstOrDefault(to => to.IsMainTable == true)?.TableId,
                FoodId = oi.FoodId,
                BuffetId = oi.BuffetId,
                ComboId = oi.ComboId,
                ItemName = GetItemName(oi),
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                Subtotal = oi.Subtotal,
                Note = oi.Note,
                OpeningTime = oi.OpeningTime,
                ServedTime = oi.ServedTime
            }).ToList();

            return new KitchenTodayHistoryResponseDTO
            {
                Date = startOfDayUtc.ToString("yyyy-MM-dd"),
                TotalItems = mappedItems.Count,
                Items = mappedItems
            };
        }

        public async Task<(bool status, string message)> AddOrderItemByOrderCodeAsync(string orderCode, List<AddOrderItemRequest> request)
        {
            var order = await _orderRepository.GetOrderByIdNoTrackingAsync(orderCode);
            if (order == null)
                return (false, "Không tìm thấy dơn hàng");
            if (order.OrderStatus == "Cancelled" || order.OrderStatus == "Closed")
                return (false, "Không thể thêm món khi đơn hàng đã hoàn thành hoặc đã hủy ");
            foreach (var item in request)
            {
                if (item.BuffetId != null)
                {
                    var currentBuffer = item.Quantity + item.QuantityBufferChildent;
                    if (currentBuffer != order.NumberOfGuests)
                        return (false, "Số lượng bufer đang khác với số lượng khách cho mỗi hóa đơn !");
                }

            }              
            return await _orderItemRepository.AddOrderItemByOrderCodeAsync(orderCode, request);
        }

        public async Task<IEnumerable<FoodFilterResponseDTO>> GetFoodForBufferAsync(string orderCode)
        {
            return await _orderItemRepository.GetFoodForBufferAsync(orderCode);
        }
    }
}

