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
    public string Message { get; set; } = null!;
}

public class ContractDepositRequestDTO
{
    public string PaymentMethod { get; set; } = null!;
    public string? TransactionId { get; set; }
    public string? Note { get; set; }
}

public class ContractDepositDirectResponseDTO
{
    public int PaymentId { get; set; }
    public string PaymentCode { get; set; } = null!;
    public int ContractId { get; set; }
    public decimal Amount { get; set; }
    public decimal RemainingAmount { get; set; }
    public string PaymentMethod { get; set; } = null!;
    public DateTime PaidAt { get; set; }
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
