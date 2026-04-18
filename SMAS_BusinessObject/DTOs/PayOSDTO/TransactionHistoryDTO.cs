using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.PayOSDTO
{
    public class TransactionHistoryRequestDTO
    {
        /// Lọc từ ngày (nullable = không lọc)
        public DateTime? FromDate { get; set; }

        ///Lọc đến ngày (nullable = không lọc)
        public DateTime? ToDate { get; set; }

        /// Cash | PayOS (nullable = lấy tất cả)
        public string? PaymentMethod { get; set; }

        /// Tìm theo mã đơn hàng (chứa / contains)
        public string? OrderCode { get; set; }

        /// Paid | Failed | Pending (nullable = lấy tất cả)
        public string? PaymentStatus { get; set; }
    }
    /// Thông tin chi tiết của một giao dịch
    public class TransactionHistoryItemDTO
    {
        // ── Thông tin giao dịch ──
        public int PaymentId { get; set; }
        public string PaymentCode { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;    // Cash | PayOS
        public string PaymentStatus { get; set; } = string.Empty;    // Paid | Unpaid
   
        public DateTime? PaidAt { get; set; }       // nullable — khớp Payment.PaidAt
        public DateTime? CreatedAt { get; set; }    // nullable — khớp Payment.CreatedAt
        public string? Note { get; set; }

        // ── Thông tin đơn hàng ──
        public int? OrderId { get; set; }
        public string? OrderCode { get; set; }
        public string? OrderType { get; set; }

        // ── Thông tin khách hàng ──
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }

        // ── Thông tin nhân viên xử lý ──
        public int? StaffId { get; set; }
        public string? StaffName { get; set; }
    }
}
