using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class ComboFood
{
    public int FoodId { get; set; }

    public int ComboId { get; set; }

    public int Quantity { get; set; }

    public virtual Combo Combo { get; set; } = null!;

    public virtual Food Food { get; set; } = null!;
}
