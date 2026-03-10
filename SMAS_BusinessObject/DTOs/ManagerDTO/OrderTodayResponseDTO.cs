using SMAS_BusinessObject.DTOs.OrderDTO;
using System;

namespace SMAS_BusinessObject.DTOs.ManagerDTO;

public class OrderTodayResponseDTO
{
    public int OrderId { get; set; }
    public string? OrderCode { get; set; }
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
    public UserInfoDto Customer { get; set; } = null!;
    public StaffInfoDto? ServedBy { get; set; }
    public DeliveryDto? Delivery { get; set; }
    public List<OrderItemDetailDto> Items { get; set; } = new();
    public List<PaymentDto> Payments { get; set; } = new();
}
