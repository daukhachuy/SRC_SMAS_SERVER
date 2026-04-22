using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_DataAccess.DAO
{
    public class ContractDAO
    {
        private readonly RestaurantDbContext _context;

        public ContractDAO(RestaurantDbContext context)
        {
            _context = context;
        }
        /// Lấy Contract theo BookingCode của BookEvent

        public async Task<Contract?> GetContractByBookEventCodeAsync(string bookingCode)
        {
            return await _context.Contracts
                .Include(c => c.Customer)
                .Include(c => c.BookEvent)
                    .ThenInclude(be => be!.Event)
                .Include(c => c.Payments)
                .FirstOrDefaultAsync(c =>
                    c.BookEvent != null &&
                    c.BookEvent.BookingCode == bookingCode);
        }

        public async Task<Contract?> GetByIdWithCustomerAsync(int contractId)
        {
            return await _context.Contracts
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(c => c.ContractId == contractId);
        }

        public async Task<Contract?> GetByIdWithCustomerAndBookEventAsync(int contractId)
        {
            return await _context.Contracts
                .Include(c => c.Customer)
                .Include(c => c.BookEvent)
                .FirstOrDefaultAsync(c => c.ContractId == contractId);
        }

        public async Task<Contract?> GetBySignTokenWithBookEventAsync(string token)
        {
            return await _context.Contracts
                .Include(c => c.BookEvent)
                .FirstOrDefaultAsync(c => c.SignMethod == token);
        }

        public async Task<Contract> CreateContractAndLinkBookEventAsync(Contract contract, BookEvent bookEvent)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Contracts.Add(contract);
                await _context.SaveChangesAsync();

                bookEvent.ContractId = contract.ContractId;
                bookEvent.IsContract = true;
                bookEvent.UpdatedAt = DateTime.UtcNow;
                _context.BookEvents.Update(bookEvent);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return contract;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Contract?> GetByIdForDepositAsync(int contractId)
        {
            return await _context.Contracts
                .Include(c => c.Payments)
                .FirstOrDefaultAsync(c => c.ContractId == contractId);
        }

        public async Task<Payment> AddDepositPaymentAndUpdateContractAsync(
            Contract contract,
            Payment payment,
            decimal totalAmount,
            decimal depositAmount,
            int depositDeadlineHoursAfterSign)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var now = DateTime.UtcNow;
                var fresh = await _context.Contracts
                    .FirstOrDefaultAsync(c => c.ContractId == contract.ContractId);
                if (fresh == null || fresh.Status != "Signed")
                    throw new InvalidOperationException("CONTRACT_NOT_ELIGIBLE_FOR_DEPOSIT");
                if (!fresh.SignedAt.HasValue)
                    throw new InvalidOperationException("CONTRACT_NOT_ELIGIBLE_FOR_DEPOSIT");
                if (now >= fresh.SignedAt.Value.AddHours(depositDeadlineHoursAfterSign))
                    throw new InvalidOperationException("DEPOSIT_DEADLINE_PASSED");

                _context.Payments.Add(payment);
                fresh.Status = "Deposited";
                fresh.RemainingAmount = totalAmount - depositAmount;
                fresh.UpdatedAt = now;
                _context.Contracts.Update(fresh);

                // Nghiệp vụ mới: cọc thành công → BookEvent tự động Active (không cần manager confirm thủ công).
                var be = await _context.BookEvents.FirstOrDefaultAsync(be => be.ContractId == fresh.ContractId);
                if (be != null && (be.Status == "Approved" || be.Status == "Confirmed"))
                {
                    be.Status = "Active";
                    be.UpdatedAt = now;
                    _context.BookEvents.Update(be);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return payment;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Hủy hợp đồng Signed đã quá hạn cọc (SignedAt + hours). Chỉ cập nhật hàng khớp điều kiện.
        /// </summary>
        public async Task<int> CancelSignedContractsPastDepositWindowAsync(int depositDeadlineHoursAfterSign)
        {
            var threshold = DateTime.UtcNow.AddHours(-depositDeadlineHoursAfterSign);
            var now = DateTime.UtcNow;

            await _context.BookEvents
                .Where(be => be.ContractId != null &&
                    _context.Contracts.Any(c =>
                        c.ContractId == be.ContractId &&
                        c.Status == "Signed" &&
                        c.SignedAt != null &&
                        c.SignedAt < threshold))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(be => be.Status, "Cancelled")
                    .SetProperty(be => be.UpdatedAt, now));

            return await _context.Contracts
                .Where(c => c.Status == "Signed"
                    && c.SignedAt != null
                    && c.SignedAt < threshold)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(c => c.Status, "Cancelled")
                    .SetProperty(c => c.UpdatedAt, now));
        }

        public async Task SignContractAndUpdateBookEventAsync(Contract contract)
        {
            if (contract.BookEventId == null)
                throw new InvalidOperationException("BookEventId is required.");

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var be = contract.BookEvent ?? await _context.BookEvents.FirstOrDefaultAsync(b => b.BookEventId == contract.BookEventId);
                if (be == null)
                    throw new InvalidOperationException("BookEvent not found.");

                contract.SignedAt = DateTime.UtcNow;
                contract.Status = "Signed";
                contract.UpdatedAt = DateTime.UtcNow;
                _context.Contracts.Update(contract);

                // Giữ trạng thái BookEvent (thường là Approved). Khi cọc thành công sẽ tự động chuyển Active.
                be.UpdatedAt = DateTime.UtcNow;
                _context.BookEvents.Update(be);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateContractAfterSendSignAsync(Contract contract, string token, DateTime utcNow)
        {
            contract.SignMethod = token;
            contract.Status = "Sent";
            contract.UpdatedAt = utcNow;
            _context.Contracts.Update(contract);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Lấy contract kèm BookEvent + Payments (+ Staff của ReceivedBy) để dùng cho
        /// xác nhận tất toán cuối và xem lịch sử giao dịch.
        /// </summary>
        public async Task<Contract?> GetContractWithBookEventAndPaymentsAsync(int contractId)
        {
            return await _context.Contracts
                .Include(c => c.BookEvent)
                .Include(c => c.Payments)
                    .ThenInclude(p => p.ReceivedByNavigation)
                        .ThenInclude(s => s!.User)
                .FirstOrDefaultAsync(c => c.ContractId == contractId);
        }

        public async Task<Contract?> GetContractWithBookEventAndPaymentsByCodeAsync(string contractCode)
        {
            return await _context.Contracts
                .Include(c => c.BookEvent)
                .Include(c => c.Payments)
                    .ThenInclude(p => p.ReceivedByNavigation)
                        .ThenInclude(s => s!.User)
                .FirstOrDefaultAsync(c => c.ContractCode == contractCode);
        }

        /// <summary>
        /// Tạo payment tiền mặt cho phần còn lại của hợp đồng và kết thúc sự kiện trong 1 transaction.
        /// - Re-check điều kiện bằng dữ liệu "fresh" để tránh race (double-click / double-confirm).
        /// - Không đụng cột computed Contract.RemainingAmount.
        /// - Tính outstanding = Contract.TotalAmount - Sum(Paid payments) -> tránh sai khi có payment khác ngoài deposit.
        /// </summary>
        public async Task<(Payment payment, decimal paidBefore, decimal paidTotal, decimal outstanding)>
            AddRemainingCashPaymentAndCompleteAsync(int contractId, string? extraNote, int managerUserId)
        {
            const decimal Tolerance = 0.01m;

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var contract = await _context.Contracts
                    .Include(c => c.Payments)
                    .FirstOrDefaultAsync(c => c.ContractId == contractId);

                if (contract == null)
                    throw new InvalidOperationException("CONTRACT_NOT_FOUND");

                if (contract.Status == "Cancelled")
                    throw new InvalidOperationException("CONTRACT_CANCELLED");

                if (contract.Status == "PaidInFull")
                    throw new InvalidOperationException("CONTRACT_ALREADY_PAID_IN_FULL");

                if (contract.Status != "Deposited")
                    throw new InvalidOperationException("CONTRACT_NOT_DEPOSITED");

                var bookEvent = contract.BookEventId == null
                    ? null
                    : await _context.BookEvents.FirstOrDefaultAsync(b => b.BookEventId == contract.BookEventId);

                if (bookEvent == null)
                    throw new InvalidOperationException("BOOKEVENT_NOT_FOUND");

                if (!string.Equals(bookEvent.Status, "AwaitingFinalPayment", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("BOOKEVENT_NOT_AWAITING_FINAL_PAYMENT");

                decimal totalAmount = contract.TotalAmount;
                decimal paidBefore = contract.Payments
                    .Where(p => p.PaymentStatus == "Paid")
                    .Sum(p => p.Amount);
                decimal outstanding = totalAmount - paidBefore;

                // Nghiệp vụ: tất toán 1 lần bằng outstanding đúng tại thời điểm confirm.
                // Nếu outstanding ~ 0 nghĩa là deposit đã đủ (bất thường với flow 30%), coi như đã trả đủ.
                if (outstanding <= Tolerance)
                    throw new InvalidOperationException("CONTRACT_ALREADY_PAID_IN_FULL");

                decimal amountToPay = outstanding;

                var now = DateTime.UtcNow;
                var note = string.IsNullOrWhiteSpace(extraNote) ? "remaining" : $"remaining: {extraNote.Trim()}";

                var payment = new Payment
                {
                    PaymentCode = "REM-" + now.ToString("yyyyMMddHHmm") + "-" + contractId,
                    ContractId = contractId,
                    OrderId = null,
                    Amount = amountToPay,
                    PaymentMethod = "Cash",
                    PaymentStatus = "Paid",
                    TransactionId = $"CASH-CON-{contractId}-{now.Ticks}",
                    Note = note,
                    ReceivedBy = managerUserId,
                    PaidAt = now,
                    CreatedAt = now
                };
                _context.Payments.Add(payment);

                contract.Status = "PaidInFull";
                contract.UpdatedAt = now;
                _context.Contracts.Update(contract);

                bookEvent.Status = "Completed";
                bookEvent.UpdatedAt = now;
                _context.BookEvents.Update(bookEvent);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                decimal paidTotal = paidBefore + amountToPay;
                decimal newOutstanding = totalAmount - paidTotal;
                return (payment, paidBefore, paidTotal, newOutstanding < 0 ? 0 : newOutstanding);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
