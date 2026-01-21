using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class Event
{
    public int EventId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? EventType { get; set; }

    public string? Image { get; set; }

    public int? MinGuests { get; set; }

    public int? MaxGuests { get; set; }

    public decimal? BasePrice { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int CreatedBy { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<BookEvent> BookEvents { get; set; } = new List<BookEvent>();

    public virtual User CreatedByNavigation { get; set; } = null!;
}
