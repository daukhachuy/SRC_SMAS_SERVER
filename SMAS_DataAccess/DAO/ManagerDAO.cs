using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.DTOs.ManagerDTO;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMAS_DataAccess.DAO
{
    public class ManagerDAO
    {
        private readonly RestaurantDbContext _context;

        public ManagerDAO(RestaurantDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách đơn hàng được tạo trong ngày hôm nay
        /// </summary>
        public async Task<List<Order>> GetOrdersTodayAsync()
        {
            var todayStart = DateTime.Today;
            var todayEnd = todayStart.AddDays(1);

            var orders = await _context.Orders
                .Where(o => o.CreatedAt >= todayStart && o.CreatedAt < todayEnd)
                .Include(o => o.User)
                .Include(o => o.ServedByNavigation)
                    .ThenInclude(s => s!.User)
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

            return orders;
        }

        /// Lấy danh sách bàn trống (Status = ACTIVE)
        public async Task<List<Table>> GetEmptyTablesAsync()
        {
            return await _context.Tables
                .Where(t => t.Status == "AVAILABLE"
                         && t.IsActive != false)
                .OrderBy(t => t.TableName)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy tổng doanh thu 7 ngày gần nhất (theo tuần)
        /// </summary>
        // ManagerDAO.cs - đổi return type từ tuple sang RevenueWeekResponseDTO
        public async Task<RevenueWeekResponseDTO> GetRevenuePreviousSevenDaysAsync()
        {
            var today = DateTime.Today;
            var sevenDaysAgo = today.AddDays(-6);
            var tomorrow = today.AddDays(1);

            var rawData = await _context.Orders
                .Where(o => o.OrderStatus == "Completed"
                         && o.CreatedAt >= sevenDaysAgo
                         && o.CreatedAt < tomorrow)   // thay .Date bằng range
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Revenue = g.Sum(o => o.TotalAmount) })
                .ToListAsync(); 
            var days = new List<DailyRevenueDTO>();
            for (int i = 6; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                var found = rawData.FirstOrDefault(x => x.Date == date);
                days.Add(new DailyRevenueDTO
                {
                    DayLabel = GetVietnameseDayLabel(date),
                    Date = date,
                    Revenue = found?.Revenue ?? 0
                });
            }

            return new RevenueWeekResponseDTO
            {
                Days = days,
                TotalRevenue = days.Sum(d => d.Revenue)
            };
        }

        private static string GetVietnameseDayLabel(DateTime d) => d.DayOfWeek switch
        {
            DayOfWeek.Monday => "T2",
            DayOfWeek.Tuesday => "T3",
            DayOfWeek.Wednesday => "T4",
            DayOfWeek.Thursday => "T5",
            DayOfWeek.Friday => "T6",
            DayOfWeek.Saturday => "T7",
            DayOfWeek.Sunday => "CN",
            _ => ""
        };

        /// <summary>
        /// Lấy 4 đơn hàng mới tạo gần nhất
        /// </summary>
        public async Task<List<Order>> GetFourNewestOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.ServedByNavigation)
                    .ThenInclude(s => s!.User)
                .Include(o => o.Delivery)
                .Include(o => o.Payments)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.Food)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.Combo)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.Buffet)
                .OrderByDescending(o => o.CreatedAt)
                .Take(4)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy danh sách nhân viên làm việc hôm nay (WorkStaff có WorkDay = today, IsWorking = true)
        /// </summary>
        public async Task<List<WorkStaff>> GetStaffWorkTodayAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return await _context.WorkStaffs
                .Where(ws => ws.WorkDay == today && ws.IsWorking == true)
                .Include(ws => ws.User)
                .Include(ws => ws.User!.Staff)
                .Include(ws => ws.Shift)
                .OrderBy(ws => ws.Shift!.StartTime)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy thông báo theo UserId
        /// </summary>
        public async Task<List<Notification>> GetNotificationsByUserIdAsync(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Tổng số lượng đặt bàn trong ngày hôm nay (theo ReservationDate)
        /// </summary>
        public async Task<int> GetSumReservationTodayAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return await _context.Reservations
                .CountAsync(r => r.ReservationDate == today);
        }

        /// <summary>
        /// Đặt bàn chờ Manager xác nhận (Status = Pending)
        /// </summary>
        public async Task<List<Reservation>> GetReservationsWaitConfirmAsync()
        {
            return await _context.Reservations
                .Where(r => r.Status == "Pending")
                .Include(r => r.User)
                .Include(r => r.ConfirmedByNavigation)
                    .ThenInclude(s => s!.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Tất cả đặt bàn sắp xếp theo CreatedAt giảm dần
        /// </summary>
        public async Task<List<Reservation>> GetAllReservationsDescCreatedAtAsync()
        {
            return await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.ConfirmedByNavigation)
                    .ThenInclude(s => s!.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Các BookEvent sắp xếp theo CreatedAt tăng dần
        /// </summary>
        public async Task<List<BookEvent>> GetBookEventsAscCreatedAtAsync()
        {
            return await _context.BookEvents
                .Include(be => be.Event)
                .Include(be => be.Customer)
                .Include(be => be.ConfirmedByNavigation)
                    .ThenInclude(s => s!.User)
                .OrderBy(be => be.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Danh sách sự kiện sắp tới (ReservationDate >= hôm nay, loại trừ Cancelled)
        /// </summary>
        public async Task<List<BookEvent>> GetUpcomingBookEventsAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return await _context.BookEvents
                .Where(be => be.ReservationDate >= today && be.Status != "Cancelled")
                .Include(be => be.Event)
                .Include(be => be.Customer)
                .OrderBy(be => be.ReservationDate)
                .ThenBy(be => be.ReservationTime)
                .ToListAsync();
        }

        /// <summary>
        /// Số hợp đồng chưa ký: Draft (đã tạo, chưa gửi / chưa ký) hoặc Sent (đã gửi mail, chờ khách ký).
        /// Luồng BookEvent không dùng Status Pending — trước đây đếm Pending nên luôn 0.
        /// </summary>
        public async Task<int> GetNumberContractNeedSignedAsync()
        {
            return await _context.Contracts
                .CountAsync(c => c.Status == "Draft" || c.Status == "Sent");
        }

        /// <summary>
        /// Lấy danh sách Reservation đã Confirmed theo ngày, kèm User + Orders (để lấy OrderCode nếu cần)
        /// </summary>
        public async Task<List<Reservation>> GetConfirmedReservationsByDateAsync(DateOnly date)
        {
            return await _context.Reservations
                .Where(r => r.ReservationDate == date && r.Status == "Confirmed")
                .Include(r => r.User)
                .Include(r => r.Orders)
                .OrderBy(r => r.ReservationTime)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy danh sách BookEvent đã Confirmed theo ngày, kèm Customer + Event + Orders (để lấy OrderCode nếu cần)
        /// </summary>
        public async Task<List<BookEvent>> GetConfirmedBookEventsByDateAsync(DateOnly date)
        {
            return await _context.BookEvents
                .Where(be => be.ReservationDate == date && be.Status == "Confirmed")
                .Include(be => be.Customer)
                .Include(be => be.Event)
                .Include(be => be.Orders)
                .OrderBy(be => be.ReservationTime)
                .ToListAsync();
        }

        /// <summary>
        /// Đếm tổng số bàn đang active
        /// </summary>
        public async Task<int> CountActiveTablesAsync()
        {
            return await _context.Tables
                .CountAsync(t => t.IsActive != false);
        }

        /// <summary>
        /// Manager bấm Cancel đặt bàn theo ReservationCode:
        /// - Status → Cancelled
        /// - Ghi CancellationReason (bắt buộc)
        /// </summary>
        /// <returns>True nếu đã cập nhật, false nếu không tìm thấy.</returns>
        public async Task<bool> DeleteReservationByCodeAsync(string reservationCode, string cancellationReason, int? managerUserId)
        {
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.ReservationCode == reservationCode);
            if (reservation == null)
                return false;

            var now = DateTime.UtcNow;

            reservation.Status = "Cancelled";
            reservation.CancelledAt = now;
            reservation.CancellationReason = cancellationReason;
            reservation.UpdatedAt = now;
            reservation.ConfirmedAt = null;
            reservation.ConfirmedBy = null;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Confirm reservation đang ở trạng thái Pending:
        /// - Status → Confirmed
        /// </summary>
        public async Task<Reservation?> UpdateReservationConfirmAsync(string reservationCode, int? confirmedByStaffId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.ConfirmedByNavigation)
                    .ThenInclude(s => s!.User)
                .FirstOrDefaultAsync(r => r.ReservationCode == reservationCode);
            if (reservation == null)
                return null;

            // Chỉ cho confirm khi đang Pending
            if (!string.Equals(reservation.Status, "Pending", StringComparison.OrdinalIgnoreCase))
                return null;

            var now = DateTime.UtcNow;

            reservation.Status = "Confirmed";
            reservation.ConfirmedAt = now;
            reservation.ConfirmedBy = confirmedByStaffId;
            reservation.CancelledAt = null;
            reservation.CancellationReason = null;
            reservation.UpdatedAt = now;

            await _context.SaveChangesAsync();
            return reservation;
        }
    }
}
