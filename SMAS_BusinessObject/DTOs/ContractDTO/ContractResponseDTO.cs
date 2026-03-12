using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.ContractDTO
{
    public class ContractResponseDTO
    {
        public int ContractId { get; set; }
        public string? ContractCode { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }

        // BookEvent info
        public int? BookEventId { get; set; }
        public string? BookingCode { get; set; }
        public string? EventName { get; set; }

        // Contract details
        public string? EventType { get; set; }
        public DateOnly EventDate { get; set; }
        public int? NumberOfGuests { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? DepositAmount { get; set; }
        public decimal? RemainingAmount { get; set; }
        public string? SignMethod { get; set; }
        public DateTime? SignedAt { get; set; }
        public string? ContractFileUrl { get; set; }
        public string? ServiceDetails { get; set; }
        public string? TermsAndConditions { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Payments
        public List<PaymentSummaryDTO> Payments { get; set; } = new();
    }

    public class PaymentSummaryDTO
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
