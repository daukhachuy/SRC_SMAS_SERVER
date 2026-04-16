using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class Service
{
    public int ServiceId { get; set; }

    public string Title { get; set; } = null!;

    public decimal ServicePrice { get; set; }

    public string? Description { get; set; }

    public string? Unit { get; set; } //theo buổi hoặc theo giờ. sự kiện như pháo giấy thì tính theo buổi, còn mc hay band nhạc là phải tính theo giờ.

    public string? Image { get; set; }

    public bool? IsAvailable { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<BookEventService> BookEventServices { get; set; } = new List<BookEventService>();
}
