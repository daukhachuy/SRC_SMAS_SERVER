using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_DataAccess.DAO
{
    public class OrderDAO
    {
        private readonly RestaurantDbContext _context;

        public OrderDAO(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetOrdersByUserAndStatusAsync(int userId,string orderType,List<string> statuses)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId
                         && o.OrderType == orderType
                         && statuses.Contains(o.OrderStatus))
                .Include(o => o.User)
                .Include(o => o.ServedByNavigation)
                    .ThenInclude(s => s.User)
                .Include(o => o.Delivery)
                .Include(o => o.Payments)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.Food)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.Combo)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.Buffet)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order> CreateOrderDeliveryAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        /// <summary>
        /// Cập nhật trạng thái đơn hàng (dùng khi webhook PayOS báo thanh toán thành công).
        /// </summary>
        public async Task<bool> UpdateOrderStatusAsync(int orderId, string orderStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;
            order.OrderStatus = orderStatus;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
