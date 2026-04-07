using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class DeliveryDetail
{
    public int DeliveryId { get; set; }

    public int? OrderId { get; set; }

    public string? DeliveryCode { get; set; }

    public string RecipientName { get; set; } = null!;

    public string RecipientPhone { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? DeliveryStatus { get; set; } // Pending ,Assigned  ,PickingUp đang lấy món ,Delivering đang giao ,Completed hoàn thành ,Failed 

    public int? AssignedStaffId { get; set; }

    public string? Note { get; set; }

    public DateTime? EstimatedDeliveryTime { get; set; }

    public DateTime? ActualDeliveryTime { get; set; }

    public decimal? DeliveryFee { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Staff? AssignedStaff { get; set; }

    public virtual Order? Order { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
