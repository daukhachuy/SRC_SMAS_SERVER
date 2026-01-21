using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class AdminLog
{
    public int LogId { get; set; }

    public int UserId { get; set; }

    public string? TableName { get; set; }

    public int? RecordId { get; set; }

    public string Action { get; set; } = null!;

    public string? Details { get; set; }

    public string? IpAddress { get; set; }

    public DateTime? Timestamp { get; set; }

    public virtual User User { get; set; } = null!;
}
