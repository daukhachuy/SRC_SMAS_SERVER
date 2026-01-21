using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public string? TransactionCode { get; set; }

    public string TransactionType { get; set; } = null!;

    public int? SupplierId { get; set; }

    public string? Image { get; set; }

    public DateTime TransactionDate { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal? PaidAmount { get; set; }

    public string? PaymentStatus { get; set; }

    public string? Note { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Staff? CreatedByNavigation { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual Supplier? Supplier { get; set; }
}
