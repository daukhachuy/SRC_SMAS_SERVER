using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public int? SenderId { get; set; }

    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public string Type { get; set; } = null!;

    public string? Severity { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ReadAt { get; set; }

    public virtual User? Sender { get; set; }

    public virtual User User { get; set; } = null!;
}
