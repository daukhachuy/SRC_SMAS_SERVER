namespace SMAS_BusinessObject.DTOs.Workflow;

public class BookEventReviewRequestDTO
{
    public string Decision { get; set; } = null!;
    public string? Note { get; set; }
}

public class BookEventReviewResponseDTO
{
    public int BookEventId { get; set; }
    public string Status { get; set; } = null!;
    public int? ConfirmedBy { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public string? Note { get; set; }
}

public class CreateContractFromBookEventRequestDTO
{
    public int DepositPercent { get; set; }
    public string? TermsAndConditions { get; set; }
    public string? Note { get; set; }
}

public class CreateContractFromBookEventResponseDTO
{
    public int ContractId { get; set; }
    public string ContractCode { get; set; } = null!;
    public int BookEventId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

public class BookEventDetailResponseDTO
{
    public int BookEventId { get; set; }
    public string? BookingCode { get; set; }
    public string? Status { get; set; }
    public DateTime? CreatedAt { get; set; }

    public BookEventDetailCustomerDTO Customer { get; set; } = null!;
    public BookEventDetailEventInfoDTO EventInfo { get; set; } = null!;
    public BookEventDetailConfirmedByDTO ConfirmedBy { get; set; } = null!;
    public List<BookEventDetailFoodDTO> Foods { get; set; } = new();
    public List<BookEventDetailServiceDTO> Services { get; set; } = new();
    public BookEventDetailContractDTO Contract { get; set; } = null!;
    public BookEventDetailPaymentSectionDTO Payment { get; set; } = null!;
}

public class BookEventDetailCustomerDTO
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
}

public class BookEventDetailEventInfoDTO
{
    public string ReservationDate { get; set; } = null!;
    public string ReservationTime { get; set; } = null!;
    public int NumberOfGuests { get; set; }
    public string? Note { get; set; }
}

public class BookEventDetailConfirmedByDTO
{
    public int? StaffId { get; set; }
    public string? FullName { get; set; }
    public DateTime? ConfirmedAt { get; set; }
}

public class BookEventDetailFoodDTO
{
    public int FoodId { get; set; }
    public string Name { get; set; } = null!;
    public string? Image { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
    public string? Note { get; set; }
}

public class BookEventDetailServiceDTO
{
    public int ServiceId { get; set; }
    public string Name { get; set; } = null!;
    public int? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? Subtotal { get; set; }
    public string? Note { get; set; }
}

public class BookEventDetailContractDTO
{
    public int? ContractId { get; set; }
    public string? ContractCode { get; set; }
    public string? Status { get; set; }
    public DateTime? SignedAt { get; set; }
    /// <summary>Hạn cọc (UTC) = SignedAt + cấu hình giờ; chỉ có nghĩa khi đã ký.</summary>
    public DateTime? DepositDueUtc { get; set; }
    public string? ContractFileUrl { get; set; }
    public string? TermsAndConditions { get; set; }
}

public class BookEventDetailPaymentSectionDTO
{
    public decimal TotalAmount { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public List<BookEventDetailPaymentItemDTO> Payments { get; set; } = new();
}

public class BookEventDetailPaymentItemDTO
{
    public int PaymentId { get; set; }
    public string? PaymentCode { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = null!;
    public string? PaymentStatus { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? Note { get; set; }
}

public class SendContractSignResponseDTO
{
    public int ContractId { get; set; }
    public string ContractCode { get; set; } = null!;
    public string SentTo { get; set; } = null!;
    public DateTime Deadline { get; set; }
    public string Message { get; set; } = null!;
}

public class ContractSignRequestDTO
{
    public string Token { get; set; } = null!;
}

public class ContractSignResponseDTO
{
    public int ContractId { get; set; }
    public string ContractCode { get; set; } = null!;
    public DateTime SignedAt { get; set; }
    public DateTime DepositDueUtc { get; set; }
    public string Message { get; set; } = null!;
}

public class ContractDepositPayOSResponseDTO
{
    public int ContractId { get; set; }
    public decimal Amount { get; set; }
    public string CheckoutUrl { get; set; } = null!;
    public string Message { get; set; } = null!;
}

public class ConfirmBookEventResponseDTO
{
    public int BookEventId { get; set; }
    public string BookingCode { get; set; } = null!;
    public string Status { get; set; } = null!;
    public int ConfirmedBy { get; set; }
    public DateTime ConfirmedAt { get; set; }
    public bool EmailSent { get; set; }
    public string Message { get; set; } = null!;
}

/// <summary>
/// Request manager xác nhận thu phần còn lại của hợp đồng (sau checkout sự kiện).
/// Mặc định phương thức Cash; nếu Amount null sẽ auto dùng outstanding tính động.
/// </summary>
public class ConfirmContractFinalPaymentRequestDTO
{
    /// <summary>
    /// Ghi chú thêm (tuỳ chọn). Payment.Note sẽ luôn bắt đầu bằng "remaining".
    /// Số tiền thu được backend tự tính = TotalAmount - Σ(payment đã Paid), manager không cần nhập.
    /// </summary>
    public string? Note { get; set; }
}

public class ConfirmContractFinalPaymentResponseDTO
{
    public int ContractId { get; set; }
    public string? ContractCode { get; set; }
    public int? BookEventId { get; set; }
    public string? BookingCode { get; set; }

    public string ContractStatus { get; set; } = null!;
    public string BookEventStatus { get; set; } = null!;

    public decimal TotalAmount { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal PaidBefore { get; set; }
    public decimal PaidThisTime { get; set; }
    public decimal PaidTotal { get; set; }
    public decimal OutstandingAmount { get; set; }

    public int PaymentId { get; set; }
    public string? PaymentCode { get; set; }
    public DateTime PaidAt { get; set; }
    public string Message { get; set; } = null!;
}

public class ContractPaymentHistoryResponseDTO
{
    public int ContractId { get; set; }
    public string? ContractCode { get; set; }
    public string? ContractStatus { get; set; }
    public int? BookEventId { get; set; }
    public string? BookingCode { get; set; }

    public decimal TotalAmount { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal OutstandingAmount { get; set; }

    public List<ContractPaymentHistoryItemDTO> Payments { get; set; } = new();
}

public class ContractPaymentHistoryItemDTO
{
    public int PaymentId { get; set; }
    public string? PaymentCode { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = null!;
    public string? PaymentStatus { get; set; }
    public string? Note { get; set; }
    public string? TransactionId { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int? ReceivedBy { get; set; }
    public string? ReceivedByName { get; set; }
}
