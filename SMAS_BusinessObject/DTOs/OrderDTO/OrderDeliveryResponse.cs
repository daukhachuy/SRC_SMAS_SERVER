using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.OrderDTO
{
    public class OrderDeliveryResponse
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = "Thành công";
        /// <summary>
        /// Id đơn hàng vừa tạo (dùng để gọi API tạo link thanh toán PayOS).
        /// </summary>
        public int? OrderId { get; set; }
        public string? OrderCode { get; set; }
    }
}
