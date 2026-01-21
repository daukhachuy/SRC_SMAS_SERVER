using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class StaffLog
{
    public int LogId { get; set; }

    public int UserId { get; set; }

    public string Action { get; set; } = null!;

    public string? Details { get; set; }

    public DateTime? Timestamp { get; set; }

    public virtual Staff User { get; set; } = null!;
}
