using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class Inventory
{
    public int InventoryId { get; set; }

    public int IngredientId { get; set; }

    public string? BatchCode { get; set; }

    public double QuantityOnHand { get; set; }

    public decimal PricePerUnit { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public int? TransactionId { get; set; }

    public string? WarehouseLocation { get; set; }

    public string? Status { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ImExport> ImExports { get; set; } = new List<ImExport>();

    public virtual Ingredient Ingredient { get; set; } = null!;

    public virtual ICollection<InventoryLog> InventoryLogs { get; set; } = new List<InventoryLog>();

    public virtual Transaction? Transaction { get; set; }
}
