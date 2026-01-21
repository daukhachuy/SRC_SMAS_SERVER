using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class EmployeeAnnouncement
{
    public int EmployeeAnnouncementId { get; set; }

    public string Title { get; set; } = null!;

    public string? Details { get; set; }

    public string? Priority { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual User? CreatedByNavigation { get; set; }
}
