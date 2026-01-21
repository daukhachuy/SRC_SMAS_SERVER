using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class Buffet
{
    public int BuffetId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal MainPrice { get; set; }

    public decimal? ChildrenPrice { get; set; }

    public decimal? SidePrice { get; set; }

    public string? Image { get; set; }

    public bool? IsAvailable { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public virtual ICollection<BuffetFood> BuffetFoods { get; set; } = new List<BuffetFood>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
