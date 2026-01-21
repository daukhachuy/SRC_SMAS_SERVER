using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public string? OrderCode { get; set; }

    public int UserId { get; set; }

    public int? ReservationId { get; set; }

    public int? BookEventId { get; set; }

    public int? DiscountId { get; set; }

    public int? DeliveryId { get; set; }

    public string? OrderType { get; set; }

    public string? OrderStatus { get; set; }

    public int? NumberOfGuests { get; set; }

    public decimal? SubTotal { get; set; }

    public decimal? DiscountAmount { get; set; }

    public decimal? TaxAmount { get; set; }

    public decimal? DeliveryPrice { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public int? ServedBy { get; set; }

    public virtual BookEvent? BookEvent { get; set; }

    public virtual ICollection<CustomerFeedback> CustomerFeedbacks { get; set; } = new List<CustomerFeedback>();

    public virtual DeliveryDetail? Delivery { get; set; }

    public virtual ICollection<DeliveryDetail> DeliveryDetails { get; set; } = new List<DeliveryDetail>();

    public virtual Discount? Discount { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Reservation? Reservation { get; set; }

    public virtual Staff? ServedByNavigation { get; set; }

    public virtual ICollection<TableOrder> TableOrders { get; set; } = new List<TableOrder>();

    public virtual User User { get; set; } = null!;
}
