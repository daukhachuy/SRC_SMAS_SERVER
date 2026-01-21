using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class ImExport
{
    public int ImExportId { get; set; }

    public string Type { get; set; } = null!;

    public int InventoryId { get; set; }

    public int IngredientId { get; set; }

    public double Quantity { get; set; }

    public decimal? PricePerUnit { get; set; }

    public string? UnitOfMeasurement { get; set; }

    public string? FromWarehouse { get; set; }

    public string? ToWarehouse { get; set; }

    public string? Reason { get; set; }

    public string? Note { get; set; }

    public int? CreatedByStaffId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Staff? CreatedByStaff { get; set; }

    public virtual Ingredient Ingredient { get; set; } = null!;

    public virtual Inventory Inventory { get; set; } = null!;
}
