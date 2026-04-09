using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SMAS_BusinessObject.Cache;
using SMAS_BusinessObject.DTOs.OrderDTO;
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
        private readonly IMemoryCache _cache;

        public OrderDAO(RestaurantDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }
        public async Task<List<Order>> GetAllActiveOrderAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.ServedByNavigation)
                    .ThenInclude(s => s!.User)
                .Include(o => o.Delivery)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Food)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Buffet)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Combo)
                .Include(o => o.Payments)
                .Where(o => o.OrderStatus != "Completed" && o.OrderStatus != "Cancelled")
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Order>> GetAllOrderCompleteAndCancelAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.ServedByNavigation)
                    .ThenInclude(s => s!.User)
                .Include(o => o.Delivery)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Food)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Buffet)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Combo)
                .Include(o => o.Payments)
                .Where(o => o.OrderStatus == "Completed" || o.OrderStatus == "Cancelled")
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
        public async Task<List<Order>> GetAllOrderCompleteAndCancelByCustomerIdAsync(int customerId)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(o => o.User)
                .Include(o => o.ServedByNavigation).ThenInclude(s => s.User)
                .Include(o => o.Delivery)
                .Include(o => o.Payments)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Food)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Combo)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Buffet)
                .Where(o => o.UserId == customerId
                         && (o.OrderStatus == "Completed" || o.OrderStatus == "Cancelled"))
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
        public async Task<List<Order>> GetAllActiveOrderByOrderTypeAsync(string orderType)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.ServedByNavigation)
                    .ThenInclude(s => s!.User)
                .Include(o => o.Delivery)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Food)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Buffet)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Combo)
                .Include(o => o.Payments)
                .Where(o => o.OrderStatus != "Completed"
                         && o.OrderStatus != "Cancelled"
                         && o.OrderType == orderType)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderDetailByOrderCodeAsync(string orderCode)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.ServedByNavigation)
                    .ThenInclude(s => s!.User)
                .Include(o => o.Delivery)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Food)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Buffet)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Combo)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.OrderCode == orderCode);
        }

        public async Task<List<Order>> GetAllOrderCompleteAndCancelByOrderTypeAsync(string orderType)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.ServedByNavigation)
                    .ThenInclude(s => s!.User)
                .Include(o => o.Delivery)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Food)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Buffet)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Combo)
                .Include(o => o.Payments)
                .Where(o => (o.OrderStatus == "Completed" || o.OrderStatus == "Cancelled")
                         && o.OrderType == orderType)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
        public async Task<Order?> GetOrderByCodeNoTrackingAsync(string orderCode)
        {
            return await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Buffet).ThenInclude(b => b.BuffetFoods).ThenInclude(bf => bf.Food) // Include chi tiết món buffet
                .Include(o => o.TableOrders)
                .Include(o => o.CustomerFeedbacks)
                .FirstOrDefaultAsync(o => o.OrderCode == orderCode);
        }

        public async Task<Food?> GetFoodByIdAsync(int foodId)
        {
            return await _context.Foods.FirstOrDefaultAsync(f => f.FoodId == foodId && f.IsAvailable == true);
        }

        public async Task<Combo?> GetComboByIdAsync(int comboId)
        {
            return await _context.Combos.FirstOrDefaultAsync(c => c.ComboId == comboId && c.IsAvailable == true);
        }

        public async Task<Buffet?> GetBuffetByIdAsync(int buffetId)
        {
            return await _context.Buffets.FirstOrDefaultAsync(b => b.BuffetId == buffetId && b.IsAvailable == true);
        }

        public async Task<Reservation?> GetReservationByCodeAsync(string reservationCode)
        {
            return await _context.Reservations
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.ReservationCode == reservationCode);
        }

        public async Task<bool> HasActiveOrderByReservationIdAsync(int reservationId)
        {
            return await _context.Orders.AnyAsync(o =>
                o.ReservationId == reservationId &&
                o.OrderStatus != "Cancelled" &&
                o.OrderStatus != "Completed");
        }

        public async Task<User?> GetUserByPhoneAsync(string phone)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Phone == phone);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> TableExistsAsync(int tableId)
        {
            return await _context.Tables.AnyAsync(t => t.TableId == tableId);
        }

        public async Task<bool> IsTableOccupiedAsync(int tableId)
        {
            return await _context.TableOrders.AnyAsync(to =>
                to.TableId == tableId &&
                to.LeftAt == null &&
                to.Order.OrderStatus != "Cancelled" &&
                to.Order.OrderStatus != "Completed");
        }

        public async Task<Food?> GetFoodByIdForOrderAsync(int foodId)
        {
            return await _context.Foods.FirstOrDefaultAsync(f => f.FoodId == foodId);
        }

        public async Task<Buffet?> GetBuffetByIdForOrderAsync(int buffetId)
        {
            return await _context.Buffets.FirstOrDefaultAsync(b => b.BuffetId == buffetId);
        }

        public async Task<Combo?> GetComboByIdForOrderAsync(int comboId)
        {
            return await _context.Combos.FirstOrDefaultAsync(c => c.ComboId == comboId);
        }
        private static string CacheKey(string tableCode)
       => $"table_session_{tableCode.ToUpper()}";
        public async Task CreateInHouseOrderAsync(Order order, List<OrderItem> items, List<TableOrder> tableOrders, Reservation? reservationToUpdate)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var item in items)
                {
                    item.OrderId = order.OrderId;
                }

                if (items.Any())
                {
                    _context.OrderItems.AddRange(items);
                }

                foreach (var tableOrder in tableOrders)
                {
                    tableOrder.OrderId = order.OrderId;
                }
                _context.TableOrders.AddRange(tableOrders);
                // Cập nhật status bàn + tự động tạo session cache
                var tableIds = tableOrders.Select(t => t.TableId).ToList();
                var tables = await _context.Tables
                    .Where(t => tableIds.Contains(t.TableId))
                    .ToListAsync();

                var now = DateTime.UtcNow;
                foreach (var table in tables)
                {
                    table.Status = "OPEN";
                    table.UpdatedAt = now;
                    var cacheKey = $"table_session_{table.TableId}";

                    // GUARD: Chỉ tạo session mới nếu chưa có session ACTIVE.
                    // Tránh overwrite session + vô hiệu hoá token khách đang giữ.
                    if (!_cache.TryGetValue(cacheKey, out TableSessionCache? existing)
                        || existing?.Status != "ACTIVE")
                    {
                        // Tự động kích hoạt session để khách quét QR được
                        var session = new TableSessionCache
                        {
                            TableCode = table.TableId.ToString(),
                            TableId = table.TableId,
                            SessionNonce = Guid.NewGuid().ToString("N"),
                            Status = "ACTIVE",
                            OpenedBy = order.ServedBy ?? 0,
                            OpenedAt = now,
                            ExpiresAt = now.AddHours(12)
                        };
                        _cache.Set(
                            CacheKey(table.TableName),
                            session,
                            session.ExpiresAt - now);
                    }
                }

                if (reservationToUpdate != null)
                {
                    reservationToUpdate.Status = "Seated";
                    reservationToUpdate.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<OrderItem> AddOrderItemAsync(OrderItem item)
        {
            _context.OrderItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<bool> AddOrderItemToOrderAsync(OrderItem item)
        {
            try
            {
                _context.OrderItems.Add(item);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                return false;
            }
        }

        public async Task UpdateOrderTotalAsync(int orderId, decimal subtotal)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return;

            order.SubTotal = (order.SubTotal ?? 0) + subtotal;
            order.TotalAmount = (order.SubTotal ?? 0)
                              - (order.DiscountAmount ?? 0)
                              + (order.TaxAmount ?? 0)
                              + (order.DeliveryPrice ?? 0);

            await _context.SaveChangesAsync();
        }

        public async Task<List<Order>> GetOrdersByUserAndStatusAsync(int userId, string orderType, List<string> statuses)
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
            var order = await _context.Orders
                                       .Include(o => o.Payments)
                                       .Include(d => d.Delivery)
                                       .Include(o => o.TableOrders).ThenInclude(to => to.Table)
                                       .FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null) return false;
            order.OrderStatus = orderStatus;
            if (orderStatus == "Completed")
            {
                var tableIds = order.TableOrders.Select(t => t.TableId).ToList();
                var tables = await _context.Tables
                    .Where(t => tableIds.Contains(t.TableId))
                    .ToListAsync();
                foreach (var table in tables)
                {
                    table.Status = "AVAILABLE";
                    table.UpdatedAt = DateTime.UtcNow;
                    _cache.Remove($"table_session_{table.TableName.ToUpper()}");
                }
                foreach (var to in order.TableOrders.Where(t => t.LeftAt == null))
                    to.LeftAt = DateTime.UtcNow;
            }
            if (order.Delivery != null)
            {
                if (orderStatus == "Completed")
                {
                    order.Delivery.DeliveryStatus = "Completed";
                    order.Delivery.UpdatedAt = DateTime.UtcNow;
                }
                else if (orderStatus == "Cancelled")
                {
                    order.Delivery.DeliveryStatus = "Failed";
                    order.Delivery.UpdatedAt = DateTime.UtcNow;
                }
            }
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Thêm bản ghi Payment và cập nhật Order status trong một transaction (khi PayOS webhook báo thanh toán thành công).
        /// </summary>
        public async Task<bool> AddPaymentAndUpdateOrderStatusAsync(int orderId, string orderStatus, Payment payment)
        {
            var order = await _context.Orders.FindAsync(orderId); if (order == null) return false;

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                payment.OrderId = orderId;
                _context.Payments.Add(payment);
                order.OrderStatus = orderStatus;
                if (orderStatus == "Completed" || orderStatus == "Completed")
                {
                    var tableIds = order.TableOrders.Select(t => t.TableId).ToList();
                    var tables = await _context.Tables
                        .Where(t => tableIds.Contains(t.TableId))
                        .ToListAsync();

                    foreach (var table in tables)
                    {
                        table.Status = "AVAILABLE";
                        table.UpdatedAt = DateTime.UtcNow;
                        // Xóa session cache — khách không quét QR được nữa
                        _cache.Remove($"table_session_{table.TableName.ToUpper()}");
                    }
                    foreach (var to in order.TableOrders.Where(t => t.LeftAt == null))
                        to.LeftAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync(); await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<bool> UpdateOrderDeliveryFailedAtAsync(int orderId, string note)
        {
            var order = await _context.Orders.Include(d => d.DeliveryDetails).FirstOrDefaultAsync(o => o.OrderId == orderId);
            var delivery = await _context.DeliveryDetails.FirstOrDefaultAsync(d => d.OrderId == orderId);
            var closedAT = DateTime.UtcNow;
            if (order == null) return false;
            if (delivery == null) return false;
            order.ClosedAt = closedAT;
            order.OrderStatus = "Cancelled";
            delivery.Note = note;
            delivery.DeliveryStatus = "Failed";
            await _context.SaveChangesAsync(); return true;
        }

        public async Task<List<Order>> GetAllOrderPreparingByWaiterIdAsync(int userId)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(o => o.User)
                .Include(o => o.ServedByNavigation).ThenInclude(s => s.User)
                .Include(o => o.Delivery)
                .Include(o => o.Payments)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Food)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Combo)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Buffet)
                .Include(o => o.TableOrders).ThenInclude(to => to.Table)
                .Where(o => (o.OrderType == "DineIn" || o.OrderType == "TakeAway")
                         && o.OrderStatus != "Completed"
                         && o.OrderStatus != "Cancelled"
                         && o.ServedByNavigation != null
                         && o.ServedByNavigation.UserId == userId)
                .OrderByDescending(o => o.CreatedAt).ToListAsync();
        }
        public async Task<List<Order>> GetAllOrderDeliveryByWaiterIdAsync(int userId)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(o => o.User)
                .Include(o => o.ServedByNavigation).ThenInclude(s => s.User)
                .Include(o => o.Delivery)
                 .ThenInclude(d => d.AssignedStaff)
                .ThenInclude(s => s.User)
                .Include(o => o.Payments)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Food)
                .Include(o => o.TableOrders).ThenInclude(to => to.Table)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Combo)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Buffet)
                .Where(o => o.OrderType == "Delivery"
                         && o.OrderStatus != "Completed"
                         && o.OrderStatus != "Cancelled"
                          && o.Delivery != null
                 && o.Delivery.AssignedStaff != null
                 && o.Delivery.AssignedStaff.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
        public async Task<List<Order>> GetAllOrderHistoryByWaiterIdInSevenDayAsync(int userId)
        {
            var sevenDaysAgo = DateTime.Now.AddDays(-7);

            return await _context.Orders
                .AsNoTracking()
                .Include(o => o.User)
                .Include(o => o.ServedByNavigation).ThenInclude(s => s.User)
                .Include(o => o.Delivery)
                .Include(o => o.Payments)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Food)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Combo)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Buffet)
                .Where(o => (o.OrderStatus == "Completed" || o.OrderStatus == "Cancelled")
                         && o.CreatedAt >= sevenDaysAgo
                         && o.ServedByNavigation != null
                         && o.ServedByNavigation.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<(bool status, string message)> ChooseAssignedStaffbyOrderAsync(ChooseAssignedStaffRequestDTO request)
        {
            var order = await GetOrderDeliveryByCodeAsync(request.OrderCode);
            if (order == null)
                return (false, $"Không tìm thấy đơn hàng với mã {request.OrderCode}.");
            if (order.OrderStatus != "Pending")
                return (false, "Chỉ đơn Pending mới được gán nhân viên.");
            if (order.Delivery == null)
                return (false, $"Đơn hàng {request.OrderCode} không có thông tin giao hàng.");

            var staff = await _context.Staff
                                      .FirstOrDefaultAsync(s => s.UserId == request.StaffId);
            if (staff == null)
                return (false, $"Không tìm thấy nhân viên với ID {request.StaffId}.");
            if (staff.Position != "Waiter")
                return (false, $"Nhân viên {staff.UserId} không có vai trò giao hàng.");
            if (order.Delivery.AssignedStaffId != null)
            {
                order.Delivery.AssignedStaffId = request.StaffId;
                await _context.SaveChangesAsync();
                return (true, "Cập nhật nhân viên thành công.");
            }
            order.Delivery.AssignedStaffId = request.StaffId;
            order.Delivery.DeliveryStatus = "Assigned";
            order.OrderStatus = "Confirmed";
            await _context.SaveChangesAsync();


            return (true, "Gán nhân viên giao hàng thành công.");
        }

        public async Task<Order?> GetOrderDeliveryByCodeAsync(string orderCode)
        {
            return await _context.Orders
                            .Include(o => o.Delivery)
                            .FirstOrDefaultAsync(o => o.Delivery != null
                                                   && o.OrderCode == orderCode);

        }

        public async Task<(bool status, string message)> UpdateStatusOrderDelivery(string orderCode)
        {
            var order = await GetOrderDeliveryByCodeAsync(orderCode);
            if (order.Delivery.DeliveryStatus == "Pending")
            {
                order.Delivery.DeliveryStatus = "Assigned";
            }
            else if (order.Delivery.DeliveryStatus == "Assigned")
            {
                order.Delivery.DeliveryStatus = "PickingUp";
            }
            else if (order.Delivery.DeliveryStatus == "PickingUp")
            {
                order.Delivery.DeliveryStatus = "Delivering";
            }
            else if (order.Delivery.DeliveryStatus == "Delivering")
            {
                if (!await CheckPaymentAsync(order.OrderCode))
                {
                    return (false, $"Đơn hàng {order.OrderCode} chưa thanh toán, không thể cập nhật trạng thái giao hàng thành Completed.");
                }
                order.Delivery.DeliveryStatus = "Completed";
                order.OrderStatus = "Completed";
            }
            else
            {
                return (false, $"Trạng thái giao hàng hiện tại là {order.Delivery.DeliveryStatus}, không thể cập nhật tiếp.");
            }
            await _context.SaveChangesAsync();
            return (true, $"Cập nhật trạng thái giao hàng thành {order.Delivery.DeliveryStatus} thành công.");
        }

        public async Task<bool> CheckPaymentAsync(string orderCode)
        {
            var order = await _context.Orders
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.OrderCode == orderCode);

            if (order == null)
                return false;

            var totalPaid = order.Payments
                .Where(p => p.PaymentStatus == "Paid")
                .Sum(p => p.Amount);

            return totalPaid >= order.TotalAmount;
        }

        public async Task<(bool status, string message)> DeleteOrderDeliveryByDeliveryCodeAsync(string orderCode, string note)
        {
            var order = await GetOrderDeliveryByCodeAsync(orderCode);
            if (order.Delivery.DeliveryStatus != "Delivering" && order.Delivery.DeliveryStatus != "Pending")
            {
                return (false, $"Đơn hàng {orderCode} không thể hủy ở trạng thái hiện tại.");
            }
            order.Delivery.DeliveryStatus = "Failed";
            order.Delivery.Note = note;
            order.OrderStatus = "Cancelled";
            await _context.SaveChangesAsync();
            return (true, $"Hủy đơn hàng {orderCode} thành công.");
        }
        /// Lấy danh sách Food đang phục vụ cho khách order trong session bàn.
        /// Filter theo CategoryId (many-to-many qua Food.Categories) và keyword tên.
        public async Task<List<Food>> GetFoodsForSessionAsync(int? categoryId, string? keyword)
        {
            var query = _context.Foods
                .Include(f => f.Categories)
                .Where(f => f.IsAvailable == true)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(f => f.Categories.Any(c => c.CategoryId == categoryId.Value));

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim();
                query = query.Where(f => f.Name.Contains(kw));
            }

            return await query
                .OrderByDescending(f => f.IsFeatured)
                .ThenBy(f => f.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        /// Lấy danh sách Combo còn phục vụ cho session bàn.
        public async Task<List<Combo>> GetCombosForSessionAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            return await _context.Combos
                .Where(c => c.IsAvailable == true
                         && (c.StartDate == null || c.StartDate <= today)
                         && (c.ExpiryDate == null || c.ExpiryDate >= today))
                .OrderBy(c => c.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        /// Lấy danh sách Buffet còn phục vụ cho session bàn.
        public async Task<List<Buffet>> GetBuffetsForSessionAsync()
        {
            return await _context.Buffets
                .Where(b => b.IsAvailable == true)
                .OrderBy(b => b.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        /// Lấy danh sách Category đang active (để FE render tab filter).
        public async Task<List<Category>> GetCategoriesForSessionAsync()
        {
            return await _context.Categories
                .Where(c => c.IsAvailable == true)
                .OrderBy(c => c.Name)
                .AsNoTracking()
                .ToListAsync();
        }
        /// Lấy OrderCode active của một bàn (dùng cho session QR khách order tại bàn).
        public async Task<string?> GetActiveOrderCodeByTableIdAsync(int tableId)
        {
            return await _context.TableOrders
                .Where(to => to.TableId == tableId
                          && to.LeftAt == null
                          && to.Order.OrderStatus != "Cancelled"
                          && to.Order.OrderStatus != "Closed"
                          && to.Order.OrderStatus != "Completed")
                .Select(to => to.Order.OrderCode)
                .FirstOrDefaultAsync();
        }
    }
}