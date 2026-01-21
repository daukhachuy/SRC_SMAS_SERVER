using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class Ingredient
{
    public int IngredientId { get; set; }

    public string IngredientName { get; set; } = null!;

    public string UnitOfMeasurement { get; set; } = null!;

    public double? WarningLevel { get; set; }

    public double? CurrentStock { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<FoodRecipe> FoodRecipes { get; set; } = new List<FoodRecipe>();

    public virtual ICollection<ImExport> ImExports { get; set; } = new List<ImExport>();

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<InventoryLog> InventoryLogs { get; set; } = new List<InventoryLog>();
}
