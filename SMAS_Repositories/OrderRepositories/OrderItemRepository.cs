using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMAS_Repositories.OrderRepositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly OrderItemDAO _orderItemDAO;

        public OrderItemRepository(OrderItemDAO orderItemDAO)
        {
            _orderItemDAO = orderItemDAO;
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

        public Task<int> UpdateAllPendingToPreparingAsync(int orderId)
            => _orderItemDAO.UpdateAllPendingToPreparingAsync(orderId);

        public Task<int> UpdateAllPreparingToReadyAsync(int orderId, DateTime servedTimeUtc)
            => _orderItemDAO.UpdateAllPreparingToReadyAsync(orderId, servedTimeUtc);

        public Task<decimal> CancelItemAndRecalculateOrderTotalsAsync(int orderItemId, string newNote)
            => _orderItemDAO.CancelItemAndRecalculateOrderTotalsAsync(orderItemId, newNote);

        public Task<List<OrderItem>> GetReadyOrderItemsHistoryTodayAsync(DateTime startOfDayUtc, DateTime endOfDayUtc, int? orderId)
            => _orderItemDAO.GetReadyOrderItemsHistoryTodayAsync(startOfDayUtc, endOfDayUtc, orderId);
    }
}

