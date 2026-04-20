using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class Discount
{
    public int DiscountId { get; set; }

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public string DiscountType { get; set; } = null!; //Percentage/Fixed

    public decimal Value { get; set; }

    public decimal? MinOrderAmount { get; set; }

    public decimal? MaxDiscountAmount { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public int? UsageLimit { get; set; }

    public int? UsedCount { get; set; }

    public string? ApplicableFor { get; set; } //All/DineIn/Takeaway/Delivery

    public string? Status { get; set; }    // Active/Inactive/Expired
        
    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }    

    public virtual Staff? CreatedByNavigation { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
