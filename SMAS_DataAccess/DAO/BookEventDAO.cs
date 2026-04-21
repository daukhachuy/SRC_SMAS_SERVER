using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SMAS_DataAccess.DAO
{
    public class BookEventDAO
    {
        private readonly RestaurantDbContext _context;
        private const string EventOrderType = "BookEvent";
        private const string EventSessionOrderStatus = "EventSession";
        private const string EventTableStatus = "EVENT";
        private const string AvailableTableStatus = "AVAILABLE";
        private const string ReminderNotificationType = "BookEventReminder3h";

        public BookEventDAO(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<List<BookEvent>> GetAllActiveBookEventAsync()
        {
            return await _context.BookEvents
                .Include(be => be.Customer)
                .Include(be => be.Event)
                .Include(be => be.ConfirmedByNavigation)
                    .ThenInclude(s => s!.User)
                .Include(be => be.Contract)
                .Include(be => be.BookEventServices)
                    .ThenInclude(s => s.Service)
                .Include(be => be.EventFoods)
                    .ThenInclude(ef => ef.Food)
                .Where(be => be.Status != "Cancelled" && be.Status != "Completed")
                .OrderByDescending(be => be.ReservationDate)
                .ToListAsync();
        }
        public async Task<BookEvent?> GetBookEventByIdAsync(int bookEventId)
        {
            return await _context.BookEvents
                .Include(be => be.Customer)
                .Include(be => be.Event)
                .Include(be => be.ConfirmedByNavigation)
                    .ThenInclude(s => s!.User)
                .Include(be => be.Contract)
                .Include(be => be.BookEventServices)
                    .ThenInclude(s => s.Service)
                .Include(be => be.EventFoods)
                    .ThenInclude(ef => ef.Food)
                .FirstOrDefaultAsync(be => be.BookEventId == bookEventId);
        }
        public async Task<List<BookEvent>> GetAllBookEventCompleteAndCancelAsync()
        {
            return await _context.BookEvents
                .Include(be => be.Customer)
                .Include(be => be.Event)
                .Include(be => be.ConfirmedByNavigation)
                    .ThenInclude(s => s!.User)
                .Include(be => be.Contract)
                .Include(be => be.BookEventServices)
                    .ThenInclude(s => s.Service)
                .Include(be => be.EventFoods)
                    .ThenInclude(ef => ef.Food)
                .Where(be => be.Status == "Cancelled" || be.Status == "Completed")
                .OrderByDescending(be => be.ReservationDate)
                .ToListAsync();
        }

        /// <summary>
        /// Lịch sử đặt sự kiện của một khách hàng (mọi trạng thái), mới nhất trước.
        /// </summary>
        public async Task<List<BookEvent>> GetBookEventsByCustomerIdAsync(int customerId)
        {
            return await _context.BookEvents
                .Include(be => be.Customer)
                .Include(be => be.Event)
                .Include(be => be.ConfirmedByNavigation)
                    .ThenInclude(s => s!.User)
                .Include(be => be.Contract)
                .Include(be => be.BookEventServices)
                    .ThenInclude(s => s.Service)
                .Include(be => be.EventFoods)
                    .ThenInclude(ef => ef.Food)
                .Where(be => be.CustomerId == customerId)
                .OrderByDescending(be => be.CreatedAt)
                .ThenByDescending(be => be.BookEventId)
                .ToListAsync();
        }

        /// <summary>
        /// Tạo đặt sự kiện kèm dịch vụ và món ăn trong một transaction.
        /// Chỉ lưu DB khi tất cả thành công; nếu lỗi thì rollback.
        /// </summary>
        public async Task<BookEvent> CreateBookEventWithDetailsAsync(
            BookEvent bookEvent,
            List<BookEventService> bookEventServices,
            List<EventFood> eventFoods)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.BookEvents.Add(bookEvent);
                await _context.SaveChangesAsync();

                foreach (var s in bookEventServices)
                {
                    s.BookEventId = bookEvent.BookEventId;
                    _context.BookEventServices.Add(s);
                }
                foreach (var f in eventFoods)
                {
                    f.BookEventId = bookEvent.BookEventId;
                    _context.EventFoods.Add(f);
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return bookEvent;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<BookEvent?> GetBookEventForReviewAsync(int bookEventId)
        {
            return await _context.BookEvents.FirstOrDefaultAsync(be => be.BookEventId == bookEventId);
        }

        public async Task<BookEvent?> GetBookEventForCreateContractAsync(int bookEventId)
        {
            return await _context.BookEvents
                .Include(be => be.Event)
                .Include(be => be.EventFoods).ThenInclude(ef => ef.Food)
                .Include(be => be.BookEventServices).ThenInclude(bs => bs.Service)
                .FirstOrDefaultAsync(be => be.BookEventId == bookEventId);
        }

        public async Task<BookEvent?> GetBookEventForDetailAsync(int bookEventId)
        {
            return await _context.BookEvents
                .AsSplitQuery()
                .Include(be => be.Customer)
                .Include(be => be.ConfirmedByNavigation).ThenInclude(s => s!.User)
                .Include(be => be.Event)
                .Include(be => be.EventFoods).ThenInclude(ef => ef.Food)
                .Include(be => be.BookEventServices).ThenInclude(bs => bs.Service)
                .Include(be => be.Contract)
                    .ThenInclude(c => c!.Payments)
                .FirstOrDefaultAsync(be => be.BookEventId == bookEventId);
        }

        public async Task<BookEvent?> GetBookEventWithContractAndCustomerAsync(int bookEventId)
        {
            return await _context.BookEvents
                .Include(be => be.Customer)
                .Include(be => be.Contract)
                .FirstOrDefaultAsync(be => be.BookEventId == bookEventId);
        }

        public async Task<(BookEvent bookEvent, string orderCode, List<int> tableIds, DateTime checkInAt)> CheckInBookEventAsync(
            int bookEventId,
            int managerUserId,
            List<int> tableIds)
        {
            if (tableIds == null || tableIds.Count == 0)
                throw new ArgumentException("Vui lòng chọn danh sách bàn để check-in sự kiện.");

            var normalizedTableIds = tableIds
                .Where(t => t > 0)
                .Distinct()
                .ToList();

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var bookEvent = await _context.BookEvents
                    .Include(be => be.Customer)
                    .FirstOrDefaultAsync(be => be.BookEventId == bookEventId);

                if (bookEvent == null)
                    throw new KeyNotFoundException($"Không tìm thấy BookEvent với id: {bookEventId}.");

                if (!string.Equals(bookEvent.Status, "Active", StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException("Chỉ cho phép check-in sự kiện ở trạng thái Active.");

                var eventStart = bookEvent.ReservationDate.ToDateTime(bookEvent.ReservationTime);
                var earliestCheckIn = eventStart.AddMinutes(-30);
                var localNow = DateTime.Now;
                if (localNow < earliestCheckIn)
                    throw new ArgumentException(
                        $"Chưa đến thời gian check-in. Bạn chỉ có thể check-in từ {earliestCheckIn:yyyy-MM-dd HH:mm}.");

                if (normalizedTableIds.Count != bookEvent.NumberOfGuests)
                    throw new ArgumentException($"Số bàn chọn phải đúng bằng số bàn đã đăng ký ({bookEvent.NumberOfGuests}).");

                var hasExistingSession = await _context.Orders.AnyAsync(o =>
                    o.BookEventId == bookEventId &&
                    o.OrderType == EventOrderType &&
                    o.OrderStatus == EventSessionOrderStatus);
                if (hasExistingSession)
                    throw new ArgumentException("Sự kiện này đã được check-in trước đó.");

                var tables = await _context.Tables
                    .Where(t => normalizedTableIds.Contains(t.TableId))
                    .ToListAsync();

                if (tables.Count != normalizedTableIds.Count)
                    throw new ArgumentException("Một hoặc nhiều bàn không tồn tại.");

                if (tables.Any(t => t.IsActive == false))
                    throw new ArgumentException("Chỉ được chọn bàn đang hoạt động.");

                var invalidStatusTable = tables
                    .FirstOrDefault(t => !string.Equals(t.Status, AvailableTableStatus, StringComparison.OrdinalIgnoreCase));
                if (invalidStatusTable != null)
                    throw new ArgumentException($"Bàn {invalidStatusTable.TableName} không ở trạng thái AVAILABLE.");

                var hasOccupiedTable = await _context.TableOrders.AnyAsync(to =>
                    normalizedTableIds.Contains(to.TableId) &&
                    to.LeftAt == null &&
                    to.Order.OrderStatus != "Cancelled" &&
                    to.Order.OrderStatus != "Completed");
                if (hasOccupiedTable)
                    throw new ArgumentException("Có bàn đang được sử dụng, vui lòng chọn bàn khác.");

                var now = DateTime.UtcNow;
                var order = new Order
                {
                    OrderCode = GenerateOrderCode(now, "OBE"),
                    UserId = bookEvent.CustomerId,
                    ReservationId = null,
                    BookEventId = bookEvent.BookEventId,
                    DiscountId = null,
                    DeliveryId = null,
                    OrderType = EventOrderType,
                    OrderStatus = EventSessionOrderStatus,
                    NumberOfGuests = bookEvent.NumberOfGuests,
                    Note = $"Giữ bàn cho sự kiện {bookEvent.BookingCode}",
                    ServedBy = managerUserId,
                    SubTotal = 0,
                    DiscountAmount = 0,
                    TaxAmount = 0,
                    DeliveryPrice = 0,
                    TotalAmount = 0,
                    CreatedAt = now,
                    ClosedAt = null
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                var tableOrders = normalizedTableIds.Select((tableId, index) => new TableOrder
                {
                    TableId = tableId,
                    OrderId = order.OrderId,
                    IsMainTable = index == 0,
                    JoinedAt = now,
                    LeftAt = null
                }).ToList();

                _context.TableOrders.AddRange(tableOrders);

                foreach (var table in tables)
                {
                    table.Status = EventTableStatus;
                    table.UpdatedAt = now;
                }

                bookEvent.Status = "InProgress";
                bookEvent.UpdatedAt = now;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (bookEvent, order.OrderCode ?? string.Empty, normalizedTableIds, now);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<(BookEvent bookEvent, List<int> releasedTableIds, DateTime checkOutAt)> CheckoutBookEventAsync(
            int bookEventId,
            int managerUserId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var bookEvent = await _context.BookEvents
                    .FirstOrDefaultAsync(be => be.BookEventId == bookEventId);
                if (bookEvent == null)
                    throw new KeyNotFoundException($"Không tìm thấy BookEvent với id: {bookEventId}.");

                if (!string.Equals(bookEvent.Status, "InProgress", StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException("Chỉ cho phép checkout sự kiện đang ở trạng thái InProgress.");

                var sessionOrder = await _context.Orders
                    .Include(o => o.TableOrders)
                        .ThenInclude(to => to.Table)
                    .FirstOrDefaultAsync(o =>
                        o.BookEventId == bookEventId &&
                        o.OrderType == EventOrderType &&
                        o.OrderStatus == EventSessionOrderStatus);

                if (sessionOrder == null)
                    throw new ArgumentException("Không tìm thấy phiên giữ bàn của sự kiện để checkout.");

                var now = DateTime.UtcNow;
                var releasedTableIds = new List<int>();

                foreach (var tableOrder in sessionOrder.TableOrders.Where(to => to.LeftAt == null))
                {
                    tableOrder.LeftAt = now;
                    releasedTableIds.Add(tableOrder.TableId);

                    if (tableOrder.Table != null)
                    {
                        tableOrder.Table.Status = AvailableTableStatus;
                        tableOrder.Table.UpdatedAt = now;
                    }
                }

                sessionOrder.OrderStatus = "Completed";
                sessionOrder.ClosedAt = now;
                sessionOrder.ServedBy ??= managerUserId;

                bookEvent.Status = "Completed";
                bookEvent.UpdatedAt = now;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (bookEvent, releasedTableIds.Distinct().ToList(), now);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<int> NotifyManagersBeforeUpcomingEventsAsync(int hoursBeforeStart)
        {
            var hours = Math.Max(1, hoursBeforeStart);
            var now = DateTime.Now;
            var maxTime = now.AddHours(hours);

            var activeEvents = await _context.BookEvents
                .Include(be => be.Event)
                .Where(be => be.Status == "Active")
                .ToListAsync();

            var upcoming = activeEvents
                .Select(be => new
                {
                    BookEvent = be,
                    EventDateTime = be.ReservationDate.ToDateTime(be.ReservationTime)
                })
                .Where(x => x.EventDateTime > now && x.EventDateTime <= maxTime)
                .ToList();

            if (!upcoming.Any())
                return 0;

            var managerIds = await _context.Users
                .Include(u => u.Staff)
                .Where(u => u.Role == "Manager" || (u.Staff != null && u.Staff.Position == "Manager"))
                .Select(u => u.UserId)
                .Distinct()
                .ToListAsync();

            if (!managerIds.Any())
                return 0;

            var notificationsToAdd = new List<Notification>();
            foreach (var item in upcoming)
            {
                var be = item.BookEvent;
                var title = $"Sự kiện sắp diễn ra (còn {hours} giờ)";
                var content =
                    $"BookEvent {be.BookingCode} ({be.Event?.Title ?? "N/A"}) sẽ diễn ra lúc {item.EventDateTime:yyyy-MM-dd HH:mm}.";

                var existingUsers = await _context.Notifications
                    .Where(n => n.Type == ReminderNotificationType && n.Content == content)
                    .Select(n => n.UserId)
                    .Distinct()
                    .ToListAsync();

                foreach (var managerId in managerIds.Where(id => !existingUsers.Contains(id)))
                {
                    notificationsToAdd.Add(new Notification
                    {
                        UserId = managerId,
                        SenderId = null,
                        Title = title,
                        Content = content,
                        Type = ReminderNotificationType,
                        Severity = "Information",
                        IsRead = false,
                        CreatedAt = now
                    });
                }
            }

            if (!notificationsToAdd.Any())
                return 0;

            _context.Notifications.AddRange(notificationsToAdd);
            await _context.SaveChangesAsync();
            return notificationsToAdd.Count;
        }

        public async Task UpdateBookEventAsync(BookEvent bookEvent)
        {
            _context.BookEvents.Update(bookEvent);
            await _context.SaveChangesAsync();
        }

        private static string GenerateOrderCode(DateTime now, string prefix)
        {
            var random = Random.Shared.Next(1000, 10000);
            return $"{prefix}-{now:yyyyMMddHHmmss}-{random}";
        }
    }
}
