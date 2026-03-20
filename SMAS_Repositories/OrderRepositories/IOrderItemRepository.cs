using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMAS_Repositories.OrderRepositories
{
    public interface IOrderItemRepository
    {
        Task<List<Order>> GetActiveOrdersWithPendingItemsAsync();

        Task<OrderItem?> GetOrderItemWithOrderAndNamesAsync(int orderItemId);

        Task<Order?> GetOrderByIdAsync(int orderId);

        Task<List<OrderItem>> GetOrderItemsByStatusWithNamesAsync(int orderId, string status);

        Task UpdatePreparingAsync(int orderItemId);

        Task UpdateReadyAsync(int orderItemId, DateTime servedTimeUtc);

        Task<int> UpdateAllPendingToPreparingAsync(int orderId);

        Task<int> UpdateAllPreparingToReadyAsync(int orderId, DateTime servedTimeUtc);

        Task<decimal> CancelItemAndRecalculateOrderTotalsAsync(int orderItemId, string newNote);

        Task<List<OrderItem>> GetReadyOrderItemsHistoryTodayAsync(DateTime startOfDayUtc, DateTime endOfDayUtc, int? orderId);
    }
}

