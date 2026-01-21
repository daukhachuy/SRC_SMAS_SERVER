using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class Food
{
    public int FoodId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public decimal? PromotionalPrice { get; set; }

    public string? Image { get; set; }

    public string? Unit { get; set; }

    public bool? IsAvailable { get; set; }

    public bool? IsDirectSale { get; set; }

    public bool? IsFeatured { get; set; }

    public int? PreparationTime { get; set; }

    public int? Calories { get; set; }

    public int? ViewCount { get; set; }

    public int? OrderCount { get; set; }

    public decimal? Rating { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BuffetFood> BuffetFoods { get; set; } = new List<BuffetFood>();

    public virtual ICollection<ComboFood> ComboFoods { get; set; } = new List<ComboFood>();

    public virtual ICollection<EventFood> EventFoods { get; set; } = new List<EventFood>();

    public virtual ICollection<FoodRecipe> FoodRecipes { get; set; } = new List<FoodRecipe>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
