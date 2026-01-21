using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class BuffetFood
{
    public int FoodId { get; set; }

    public int BuffetId { get; set; }

    public int? Quantity { get; set; }

    public bool? IsUnlimited { get; set; }

    public virtual Buffet Buffet { get; set; } = null!;

    public virtual Food Food { get; set; } = null!;
}
