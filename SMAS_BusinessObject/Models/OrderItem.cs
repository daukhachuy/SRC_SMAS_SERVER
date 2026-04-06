using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    public int? FoodId { get; set; }

    public int? BuffetId { get; set; }

    public int? ComboId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? Subtotal { get; set; }

    public string? Status { get; set; }  // Pending/Preparing/Ready/Served/Cancelled

    public string? Note { get; set; }

    public DateTime? OpeningTime { get; set; }

    public DateTime? ServedTime { get; set; }

    public virtual Buffet? Buffet { get; set; }

    public virtual Combo? Combo { get; set; }

    public virtual Food? Food { get; set; }

    public virtual Order Order { get; set; } = null!;
}
