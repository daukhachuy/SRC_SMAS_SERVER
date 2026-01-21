using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsProcessedGoods { get; set; }

    public string? Image { get; set; }

    public bool? IsAvailable { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Food> Foods { get; set; } = new List<Food>();
}
