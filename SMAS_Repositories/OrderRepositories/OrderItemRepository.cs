using Microsoft.EntityFrameworkCore.Query;
using SMAS_BusinessObject.DTOs.Food;
using SMAS_BusinessObject.DTOs.OrderDTO;
using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SMAS_Services.TableService;
namespace SMAS_Repositories.OrderRepositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly OrderItemDAO _orderItemDAO;

        private readonly OrderDAO _orderDAO;
     
        public OrderItemRepository(OrderItemDAO orderItemDAO, OrderDAO orderDAO)
        {
            _orderItemDAO = orderItemDAO;
            _orderDAO = orderDAO;
        }

   

        public Task<List<Order>> GetActiveOrdersWithPendingItemsAsync()
            => _orderItemDAO.GetActiveOrdersWithPendingItemsAsync();

        public Task<OrderItem?> GetOrderItemWithOrderAndNamesAsync(int orderItemId)
            => _orderItemDAO.GetOrderItemWithOrderAndNamesAsync(orderItemId);

        public Task<Order?> GetOrderByIdAsync(int orderId)
            => _orderItemDAO.GetOrderByIdAsync(orderId);

        public Task<List<OrderItem>> GetOrderItemsByStatusWithNamesAsync(int orderId, string status)
            => _orderItemDAO.GetOrderItemsByStatusWithNamesAsync(orderId, status);

        public Task UpdatePreparingAsync(int orderItemId)
            => _orderItemDAO.UpdatePreparingAsync(orderItemId);

        public Task UpdateReadyAsync(int orderItemId, DateTime servedTimeUtc)
            => _orderItemDAO.UpdateReadyAsync(orderItemId, servedTimeUtc);

        public Task UpdateServedAsync(int orderItemId, DateTime servedTimeUtc)
            => _orderItemDAO.UpdateServedAsync(orderItemId, servedTimeUtc);

        public Task<int> UpdateAllPendingToPreparingAsync(int orderId)
            => _orderItemDAO.UpdateAllPendingToPreparingAsync(orderId);

        public Task<int> UpdateAllPreparingToReadyAsync(int orderId, DateTime servedTimeUtc)
            => _orderItemDAO.UpdateAllPreparingToReadyAsync(orderId, servedTimeUtc);

        public Task<decimal> CancelItemAndRecalculateOrderTotalsAsync(int orderItemId, string newNote)
            => _orderItemDAO.CancelItemAndRecalculateOrderTotalsAsync(orderItemId, newNote);

        public Task<List<OrderItem>> GetReadyOrderItemsHistoryTodayAsync(DateTime startOfDayUtc, DateTime endOfDayUtc, int? orderId)
            => _orderItemDAO.GetReadyOrderItemsHistoryTodayAsync(startOfDayUtc, endOfDayUtc, orderId);

        public async  Task<(bool status, string message)> AddOrderItemByOrderCodeAsync(string orderCode, List<AddOrderItemRequest> request)
            {
            var order = await _orderDAO.GetOrderByCodeNoTrackingAsync(orderCode);
            decimal unitPrice = 0;
            var currentBuffer = order.OrderItems.FirstOrDefault(oi => oi.BuffetId.HasValue)?.BuffetId;
            var bufferFoods = _orderItemDAO.GetFoodByBuffetIdAsync(currentBuffer).Result;
            foreach (var item in request) {
                var newItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    Quantity = item.Quantity,
                    Note = item.Note,
                    OpeningTime = DateTime.UtcNow,
                    Status = "Pending"
                };

                if (item.FoodId.HasValue)
                {
                    var food = await _orderDAO.GetFoodByIdAsync(item.FoodId.Value);
                    if (food == null)
                        return ( false,"Món ăn không tồn tại hoặc đã ngừng phục vụ." );
                    if (bufferFoods.Any(f => f.FoodId == food.FoodId))
                    {
                        unitPrice = food.PromotionalPrice ?? food.Price;
                        newItem.FoodId = food.FoodId;
                        newItem.UnitPrice = 0;
                        newItem.Subtotal = 0;
                    }
                    else
                    {
                        unitPrice = food.PromotionalPrice ?? food.Price;
                        newItem.FoodId = food.FoodId;
                        newItem.UnitPrice = unitPrice;
                        newItem.Subtotal = unitPrice * item.Quantity;
                    }
                        
                }
                else if (item.ComboId.HasValue)
                {
                    var combo = await _orderDAO.GetComboByIdAsync(item.ComboId.Value);
                    if (combo == null)
                        return (false, "Combo không tồn tại hoặc đã ngừng phục vụ.");

                    unitPrice = combo.Price;
                    newItem.ComboId = combo.ComboId;
                    newItem.UnitPrice = unitPrice;
                    newItem.Subtotal = unitPrice * item.Quantity;
                }
                else if (item.BuffetId.HasValue)
                {
                    var buffet = await _orderDAO.GetBuffetByIdAsync(item.BuffetId.Value);
                    if (buffet == null)
                        return (false, "Buffer không tồn tại hoặc đã ngừng phục vụ.");
                    if (currentBuffer.HasValue && bufferFoods !=  null)
                        return (false, "Không thể thêm buffet khác vào đơn hàng đã có buffet.");
                        unitPrice = buffet.MainPrice;
                        newItem.BuffetId = buffet.BuffetId;
                        newItem.Quantity = newItem.Quantity + (item.QuantityBufferChildent ?? 0);
                        newItem.UnitPrice = unitPrice;
                        newItem.Subtotal = unitPrice * item.Quantity + buffet.ChildrenPrice * item.QuantityBufferChildent;
                        newItem.Status = "Served";
                    //if (item.QuantityBufferChildent.HasValue)
                    //{
                    //    unitPrice = buffet.ChildrenPrice ?? buffet.MainPrice;
                    //    newItem.BuffetId = buffet.BuffetId;
                    //    newItem.UnitPrice = unitPrice;
                    //    newItem.Subtotal = unitPrice * item.Quantity;
                    //    await _orderDAO.AddOrderItemToOrderAsync(newItem);
                    //}

                }

                await _orderDAO.AddOrderItemToOrderAsync(newItem);
                await _orderDAO.UpdateOrderTotalAsync(order.OrderId, newItem.Subtotal ?? 0);
                var updatedOrder = await _orderDAO.GetOrderByCodeNoTrackingAsync(orderCode);
            }
            return (true, "Thêm món ăn thành công ");

        }

        public async Task<AddOrderItemResponse> AddOrderItemAsync(string orderCode, AddOrderItemRequest request)
        {
            var requestList = new List<AddOrderItemRequest> { request };
            var (status, message) = await AddOrderItemByOrderCodeAsync(orderCode, requestList);

            return new AddOrderItemResponse
            {
                Success = status,
                Message = message
            };
        }
        public async Task<IEnumerable<FoodFilterResponseDTO>> GetFoodForBufferAsync(string orderCode)
        {
            var order = await _orderDAO.GetOrderByCodeNoTrackingAsync(orderCode);

            if (order == null)
                return new List<FoodFilterResponseDTO>();

            var currentBuffer = order.OrderItems
                                     .FirstOrDefault(oi => oi.BuffetId.HasValue)
                                     ?.BuffetId;

            if (currentBuffer == null)
                return new List<FoodFilterResponseDTO>();

            var bufferFoods = await _orderItemDAO.GetFoodByBuffetIdAsync(currentBuffer);

            if (bufferFoods == null || !bufferFoods.Any())
                return new List<FoodFilterResponseDTO>();

            return bufferFoods.Select(f => new FoodFilterResponseDTO
            {
                FoodId = f.FoodId,
                Name = f.Name,
                Description = f.Description,
                Price = f.Price ,
                PromotionalPrice = f.PromotionalPrice,
                Image = f.Image,
                Unit = f.Unit,
                Rating = f.Rating,
                Note = f.Note
            }).ToList();
        }
    }
}

