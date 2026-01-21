using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class Combo
{
    public int ComboId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public decimal? DiscountPercent { get; set; }

    public string? Image { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public int? NumberOfUsed { get; set; }

    public int? MaxUsage { get; set; }

    public bool? IsAvailable { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public virtual ICollection<ComboFood> ComboFoods { get; set; } = new List<ComboFood>();

    public virtual Staff? CreatedByNavigation { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
