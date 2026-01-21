using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class EventFood
{
    public int BookEventId { get; set; }

    public int FoodId { get; set; }

    public int Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public string? Note { get; set; }

    public virtual BookEvent BookEvent { get; set; } = null!;

    public virtual Food Food { get; set; } = null!;
}
