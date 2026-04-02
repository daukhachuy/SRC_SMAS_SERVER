using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class CustomerFeedback
{
    public int FeedbackId { get; set; }

    public int UserId { get; set; }

    public int? OrderId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public string? FeedbackType { get; set; } //-- Service/Food/Ambience/Overall

    public string? ResponseStatus { get; set; } // -- Pending/Responded

    public string? ResponseText { get; set; }

    public int? RespondedBy { get; set; }

    public DateTime? RespondedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Staff? RespondedByNavigation { get; set; }

    public virtual User User { get; set; } = null!;
}
