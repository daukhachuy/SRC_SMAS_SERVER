using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.OrderDTO
{
    public class CreateOrderDeliveryRequest
    {

        [StringLength(50, ErrorMessage = "Mã giảm giá không quá 50 ký tự.")]
        public string? DiscountCode { get; set; }

        [MaxLength(1000, ErrorMessage = "Ghi chú không quá 1000 ký tự.")]
        public string? Note { get; set; }

        [Required(ErrorMessage = "Đơn hàng phải có ít nhất một món.")]
        [MinLength(1, ErrorMessage = "Danh sách món ăn không được để trống.")]
        public List<OrderItemDto> Items { get; set; } = new();

        public OrderDeliveryRequestDto? DeliveryInfo { get; set; }
    }
    public class OrderItemDto
    {
        public int? FoodId { get; set; }
        public int? ComboId { get; set; }
        [Required(ErrorMessage = "Số lượng là bắt buộc.")]
        [Range(1, 30, ErrorMessage = "Số lượng phải từ 1 đến 30.")]
        public int Quantity { get; set; }
    }
    public class OrderDeliveryRequestDto
    {
        [Required(ErrorMessage = "Tên người nhận không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên người nhận không quá 100 ký tự.")]
        public string RecipientName { get; set; } = null!;

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Số điện thoại từ 10-15 số.")]
        public string RecipientPhone { get; set; } = null!;

        [Required(ErrorMessage = "Địa chỉ giao hàng là bắt buộc.")]
        [MaxLength(500, ErrorMessage = "Địa chỉ không quá 500 ký tự.")]
        public string Address { get; set; } = null!;

        [MaxLength(255, ErrorMessage = "Ghi chú giao hàng không quá 255 ký tự.")]
        public string? Note { get; set; }
        //[Required(ErrorMessage = "Địa chỉ giao hàng là bắt buộc.")]
        //public double Latitude { get; set; }
        //[Required(ErrorMessage = "Địa chỉ giao hàng là bắt buộc.")]
        //public double Longitude { get; set; }
    }
}
