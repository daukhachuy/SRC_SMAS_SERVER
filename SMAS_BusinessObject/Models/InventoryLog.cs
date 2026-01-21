using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class InventoryLog
{
    public int InventoryLogId { get; set; }

    public int? InventoryId { get; set; }

    public int? IngredientId { get; set; }

    public string Action { get; set; } = null!;

    public double? OldQuantity { get; set; }

    public double? NewQuantity { get; set; }

    public string? Details { get; set; }

    public int UserId { get; set; }

    public DateTime? Timestamp { get; set; }

    public virtual Ingredient? Ingredient { get; set; }

    public virtual Inventory? Inventory { get; set; }

    public virtual User User { get; set; } = null!;
}
