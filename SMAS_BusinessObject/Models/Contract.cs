using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class Contract
{
    public int ContractId { get; set; }

    public string? ContractCode { get; set; }

    public int CustomerId { get; set; }

    public int? BookEventId { get; set; }

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

    public virtual BookEvent? BookEvent { get; set; }

    public virtual ICollection<BookEvent> BookEvents { get; set; } = new List<BookEvent>();

    public virtual User Customer { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
