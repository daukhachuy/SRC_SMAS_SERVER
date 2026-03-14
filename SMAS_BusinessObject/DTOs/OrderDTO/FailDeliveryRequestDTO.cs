using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.OrderDTO
{
    public class FailDeliveryRequestDTO
    {
        [Required(ErrorMessage = "Mã đơn hàng không được trống !")]
        public int orderId { get; set; }
        public string reason { get; set; } = null!;
    }
}
