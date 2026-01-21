using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public string? Image { get; set; }

    public string SupplierName { get; set; } = null!;

    public string? ContactName { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
