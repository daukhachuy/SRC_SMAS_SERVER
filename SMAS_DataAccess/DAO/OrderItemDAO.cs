using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMAS_DataAccess.DAO
{
    public class OrderItemDAO
    {
        private readonly RestaurantDbContext _context;

        public OrderItemDAO(RestaurantDbContext context)
        {
            _context = context;
        }

        // Kitchen: GET /api/order-items/pending
        public async Task<List<Order>> GetActiveOrdersWithPendingItemsAsync()
        {
            return await _context.Orders
                .Where(o => o.OrderStatus == "Pending" || o.OrderStatus == "Processing")
                .Include(o => o.TableOrders.Where(to => to.IsMainTable == true))
                .Include(o => o.OrderItems.Where(oi => oi.Status == "Pending" || oi.Status == "Preparing" ))
                    .ThenInclude(oi => oi.Food)
                .Include(o => o.OrderItems.Where(oi => oi.Status == "Pending" || oi.Status == "Preparing" ))
                    .ThenInclude(oi => oi.Combo)
                .AsNoTracking()
                .ToListAsync();
        }

        // Kitchen: load a single item with Order + optional item names
        public async Task<OrderItem?> GetOrderItemWithOrderAndNamesAsync(int orderItemId)
        {
            return await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Food)
                .Include(oi => oi.Buffet)
                .Include(oi => oi.Combo)
                .FirstOrDefaultAsync(oi => oi.OrderItemId == orderItemId);
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<List<OrderItem>> GetOrderItemsByStatusWithNamesAsync(int orderId, string status)
        {
            return await _context.OrderItems
                .Where(oi => oi.OrderId == orderId && oi.Status == status)
                .Include(oi => oi.Food)
                .Include(oi => oi.Buffet)
                .Include(oi => oi.Combo)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdatePreparingAsync(int orderItemId)
        {
            var orderItem = await _context.OrderItems.FirstOrDefaultAsync(oi => oi.OrderItemId == orderItemId);
            if (orderItem == null) return;

            orderItem.Status = "Preparing";
            await _context.SaveChangesAsync();
        }

        public async Task UpdateReadyAsync(int orderItemId, DateTime servedTimeUtc)
        {
            var orderItem = await _context.OrderItems.FirstOrDefaultAsync(oi => oi.OrderItemId == orderItemId);
            if (orderItem == null) return;

            orderItem.Status = "Ready";
            orderItem.ServedTime = servedTimeUtc;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateServedAsync(int orderItemId, DateTime servedTimeUtc)
        {
            var orderItem = await _context.OrderItems.FirstOrDefaultAsync(oi => oi.OrderItemId == orderItemId);
            if (orderItem == null) return;

            orderItem.Status = "Served";
            orderItem.ServedTime = servedTimeUtc;
            await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAllPendingToPreparingAsync(int orderId)
        {
            var items = await _context.OrderItems
                .Where(oi => oi.OrderId == orderId && oi.Status == "Pending")
                .ToListAsync();

            foreach (var item in items)
                item.Status = "Preparing";

            await _context.SaveChangesAsync();
            return items.Count;
        }

        public async Task<int> UpdateAllPreparingToReadyAsync(int orderId, DateTime servedTimeUtc)
        {
            var items = await _context.OrderItems
                .Where(oi => oi.OrderId == orderId && oi.Status == "Preparing")
                .ToListAsync();

            foreach (var item in items)
            {
                item.Status = "Ready";
                item.ServedTime = servedTimeUtc;
            }

            await _context.SaveChangesAsync();
            return items.Count;
        }

        // Kitchen: cancel + recalc order totals in a transaction
        public async Task<decimal> CancelItemAndRecalculateOrderTotalsAsync(int orderItemId, string newNote)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var orderItem = await _context.OrderItems
                    .Include(oi => oi.Order)
                    .FirstOrDefaultAsync(oi => oi.OrderItemId == orderItemId);

                if (orderItem == null)
                    throw new KeyNotFoundException("Order item not found");

                orderItem.Status = "Cancelled";
                orderItem.Subtotal = 0m;
                orderItem.Note = newNote;

                var orderItems = await _context.OrderItems
                    .Where(oi => oi.OrderId == orderItem.OrderId && oi.Status != "Cancelled")
                    .ToListAsync();

                var newSubTotal = orderItems.Sum(oi => oi.Subtotal ?? 0m);
                orderItem.Order.SubTotal = newSubTotal;
                orderItem.Order.TotalAmount = newSubTotal;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return newSubTotal;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Kitchen: GET /api/order-items/history/today
        public async Task<List<OrderItem>> GetReadyOrderItemsHistoryTodayAsync(DateTime startOfDayUtc, DateTime endOfDayUtc, int? orderId)
        {
            var query = _context.OrderItems
                .Where(oi => oi.Status == "Ready")
                .Where(oi => oi.Order.CreatedAt >= startOfDayUtc && oi.Order.CreatedAt < endOfDayUtc)
                .Include(oi => oi.Order)
                    .ThenInclude(o => o.TableOrders.Where(to => to.IsMainTable == true))
                .Include(oi => oi.Food)
                .Include(oi => oi.Buffet)
                .Include(oi => oi.Combo)
                .AsNoTracking();

            if (orderId.HasValue)
            {
                query = query.Where(oi => oi.OrderId == orderId.Value);
            }

            return await query
                .OrderByDescending(oi => oi.ServedTime ?? DateTime.MinValue)
                .ToListAsync();
        }

        public async Task<List<Food>> GetFoodByBuffetIdAsync(int? buffetId)
        {
            if (!buffetId.HasValue) return new List<Food>();
            return await _context.Foods
                .Where(f => f.BuffetFoods.Any(bf => bf.BuffetId == buffetId.Value))
                .AsNoTracking()
                .ToListAsync();
        }
    }
}

