using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class Table
{
    public int TableId { get; set; }

    public string TableName { get; set; } = null!;

    public string? TableType { get; set; }

    public int NumberOfPeople { get; set; }

    public string? Status { get; set; } // AVAILABLE, OPEN

    public string? QrCode { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<TableOrder> TableOrders { get; set; } = new List<TableOrder>();
}
