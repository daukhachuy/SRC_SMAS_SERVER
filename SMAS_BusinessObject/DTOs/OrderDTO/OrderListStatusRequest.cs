using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.OrderDTO
{
    public class OrderListStatusRequest
    {
        [Required(ErrorMessage = "Loại đơn hàng không được trống !")]
        [RegularExpression("^(DineIn|TakeAway|Delivery|EventBooking)$",
            ErrorMessage = "Loại đơn hàng phải là  DineIn, TakeAway, Delivery or EventBooking")]
        public string orderType { get; set; } = null!;
        [Required(ErrorMessage = "Trạng thái đơn hàng không được trống !")]
        [MinLength(1, ErrorMessage = "Ít nhất một trạng thái là bắt buộc !")]
        public List<string> status { get; set; }
    }
}
