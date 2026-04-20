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
    public async Task<PagedResult<TransactionHistoryItemDTO>> GetTransactionHistoryAsync(
    TransactionHistoryRequestDTO request)
    {
        var query = _context.Payments
            .Include(p => p.Order).ThenInclude(o => o.User)
            .Include(p => p.ReceivedByNavigation).ThenInclude(s => s.User)
            .AsNoTracking()
            .AsQueryable();

        // ── Filters (giữ nguyên) ──
        if (request.FromDate.HasValue)
            query = query.Where(p => p.CreatedAt >= request.FromDate.Value.Date);

        if (request.ToDate.HasValue)
            query = query.Where(p => p.CreatedAt < request.ToDate.Value.Date.AddDays(1));

        if (!string.IsNullOrWhiteSpace(request.PaymentMethod))
            query = query.Where(p => p.PaymentMethod == request.PaymentMethod);

        if (!string.IsNullOrWhiteSpace(request.OrderCode))
            query = query.Where(p => p.Order != null && p.Order.OrderCode.Contains(request.OrderCode));

        if (!string.IsNullOrWhiteSpace(request.PaymentStatus))
            query = query.Where(p => p.PaymentStatus == request.PaymentStatus);

        // ── Đếm tổng trước khi phân trang ──
        int totalCount = await query.CountAsync();

        // ── Paging ──
        int page = Math.Max(request.Page, 1);
        int pageSize = Math.Clamp(request.PageSize, 1, 100);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new TransactionHistoryItemDTO
            {
                PaymentId = p.PaymentId,
                PaymentCode = p.PaymentCode ?? "",
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                PaymentStatus = p.PaymentStatus ?? "",
                PaidAt = p.PaidAt,
                CreatedAt = p.CreatedAt,
                Note = p.Note,
                OrderId = p.OrderId,
                OrderCode = p.Order != null ? p.Order.OrderCode : null,
                OrderType = p.Order != null ? p.Order.OrderType : null,
                CustomerId = p.Order != null ? (int?)p.Order.UserId : null,
                CustomerName = p.Order != null && p.Order.User != null ? p.Order.User.Fullname : null,
                CustomerPhone = p.Order != null && p.Order.User != null ? p.Order.User.Phone : null,
                StaffId = p.ReceivedBy,
                StaffName = p.ReceivedByNavigation != null && p.ReceivedByNavigation.User != null
                                  ? p.ReceivedByNavigation.User.Fullname : null
            })
            .ToListAsync();

        return new PagedResult<TransactionHistoryItemDTO>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

}
