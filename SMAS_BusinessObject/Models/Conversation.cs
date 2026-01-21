using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class Conversation
{
    public int ConversationId { get; set; }

    public int UserId { get; set; }

    public DateTime? LastMessageAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual User User { get; set; } = null!;
}
