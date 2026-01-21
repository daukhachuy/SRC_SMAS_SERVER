using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class FoodRecipe
{
    public int RecipeId { get; set; }

    public int FoodId { get; set; }

    public int IngredientId { get; set; }

    public double QuantityNeeded { get; set; }

    public string? Note { get; set; }

    public int? CreatedByStaffId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Staff? CreatedByStaff { get; set; }

    public virtual Food Food { get; set; } = null!;

    public virtual Ingredient Ingredient { get; set; } = null!;
}
