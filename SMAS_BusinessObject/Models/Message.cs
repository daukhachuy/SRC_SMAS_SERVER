using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class Message
{
    public int MessageId { get; set; }

    public int ConversationId { get; set; }

    public int SenderId { get; set; }

    public string? MessageType { get; set; }

    public string? Content { get; set; }

    public string? AttachmentUrl { get; set; }

    public DateTime? SentAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsRead { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
