using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class TableOrder
{
    public int TableId { get; set; }

    public int OrderId { get; set; }

    public bool? IsMainTable { get; set; }

    public DateTime? JoinedAt { get; set; }

    public DateTime? LeftAt { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Table Table { get; set; } = null!;
}
