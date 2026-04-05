using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public string? PaymentCode { get; set; }

    public int? OrderId { get; set; }

    public int? ContractId { get; set; }

    public string PaymentMethod { get; set; } = null!;  // Cash/Banking/Momo/ZaloPay

    public string? PaymentStatus { get; set; }   // Paid, Unpaid

    public decimal Amount { get; set; }

    public string? TransactionId { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? PaidAt { get; set; }

    public int? ReceivedBy { get; set; }

    public virtual Contract? Contract { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Staff? ReceivedByNavigation { get; set; }
}
