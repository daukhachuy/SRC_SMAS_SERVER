using Microsoft.EntityFrameworkCore;
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
}
