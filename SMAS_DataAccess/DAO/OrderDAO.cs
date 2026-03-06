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
    }
}
