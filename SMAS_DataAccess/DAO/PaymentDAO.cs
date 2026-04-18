using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.DTOs.PayOSDTO;
using SMAS_BusinessObject.Models;

namespace SMAS_DataAccess.DAO;

public class PaymentDAO
{
    private readonly RestaurantDbContext _context;

    public PaymentDAO(RestaurantDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsPaidDepositForContractAsync(int contractId)
    {
        return await _context.Payments.AnyAsync(p =>
            p.ContractId == contractId &&
            p.Note == "deposit" &&
            p.PaymentStatus == "Paid");
    }

    public async Task<bool> ExistsByTransactionIdAsync(string? transactionId)
    {
        if (string.IsNullOrEmpty(transactionId))
            return false;
        return await _context.Payments.AnyAsync(p => p.TransactionId == transactionId);
    }

    public async Task<bool> CreatePaymentCashAsync(Payment payment)
    {
        _context.Payments.Add(payment);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<(bool isEnough, string message)> CheckPaymentAsync(int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.Payments)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

        if (order == null)
        {
            return (false, "Không tìm thấy đơn hàng");
        }

        var orderAmount = order.TotalAmount;

        var paidAmount = order.Payments
            .Where(p => p.PaymentStatus == "Paid")
            .Sum(p => p.Amount);

        if (paidAmount >= orderAmount)
        {
            return (false, "Đã thanh toán đủ");
        }

        var remaining = orderAmount - paidAmount;

        return (true , $"Chưa đủ. Còn thiếu: {remaining}");
    }

    public async Task<bool> IsOrderDeliveryAsync(int orderid)
    {
        var order = await _context.Orders
           .Include(o => o.Payments)
           .Include(d => d.Delivery)
           .FirstOrDefaultAsync(o => o.OrderId == orderid);
        if (order == null || order.Delivery == null)
        {
            return false;
        }
        return true;
    }

    public async Task<Order?> GetOrderWithPaymentsByCodeAsync(string orderCode)
    {
        return await _context.Orders
            .Include(o => o.Payments)
            .FirstOrDefaultAsync(o => o.OrderCode == orderCode);
    }
    public async Task<List<TransactionHistoryItemDTO>> GetTransactionHistoryAsync(TransactionHistoryRequestDTO request)
    {
        var query = _context.Payments
            .Include(p => p.Order)
                .ThenInclude(o => o.User)                   // Order → User (khách hàng)
            .Include(p => p.ReceivedByNavigation)           // Payment → Staff
                .ThenInclude(s => s.User)                   // Staff → User (nhân viên)
            .AsNoTracking()
            .AsQueryable();

        // ── Lọc theo khoảng thời gian ──
        if (request.FromDate.HasValue)
        {
            var from = request.FromDate.Value.Date;
            query = query.Where(p => p.CreatedAt >= from);
        }

        if (request.ToDate.HasValue)
        {
            var to = request.ToDate.Value.Date.AddDays(1);
            query = query.Where(p => p.CreatedAt < to);
        }

        // ── Lọc theo phương thức thanh toán ──
        if (!string.IsNullOrWhiteSpace(request.PaymentMethod))
            query = query.Where(p => p.PaymentMethod == request.PaymentMethod);

        // ── Lọc theo mã đơn hàng ──
        if (!string.IsNullOrWhiteSpace(request.OrderCode))
            query = query.Where(p => p.Order != null
                                   && p.Order.OrderCode.Contains(request.OrderCode));

        // ── Lọc theo trạng thái thanh toán ──
        if (!string.IsNullOrWhiteSpace(request.PaymentStatus))
            query = query.Where(p => p.PaymentStatus == request.PaymentStatus);

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new TransactionHistoryItemDTO
            {
                // Thông tin giao dịch
                PaymentId = p.PaymentId,
                PaymentCode = p.PaymentCode ?? "",
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                PaymentStatus = p.PaymentStatus ?? "",
             
                PaidAt = p.PaidAt,                   // DateTime? → DateTime? khớp nhau
                CreatedAt = p.CreatedAt,                // DateTime? → DateTime? khớp nhau
                Note = p.Note,

                // Thông tin đơn hàng
                OrderId = p.OrderId,
                OrderCode = p.Order != null ? p.Order.OrderCode : null,
                OrderType = p.Order != null ? p.Order.OrderType : null,

                // Thông tin khách hàng (Order.User)
                // User.Fullname — chữ 'n' thường theo entity
                CustomerId = p.Order != null ? (int?)p.Order.UserId : null,
                CustomerName = p.Order != null && p.Order.User != null
                                ? p.Order.User.Fullname : null,
                CustomerPhone = p.Order != null && p.Order.User != null
                                ? p.Order.User.Phone : null,

                // Thông tin nhân viên (Payment.ReceivedByNavigation → Staff.User)
                StaffId = p.ReceivedBy,
                StaffName = p.ReceivedByNavigation != null && p.ReceivedByNavigation.User != null
                            ? p.ReceivedByNavigation.User.Fullname : null
            })
            .ToListAsync();
    }

}
