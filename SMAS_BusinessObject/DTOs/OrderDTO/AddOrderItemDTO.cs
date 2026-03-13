using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.OrderDTO
{
    public class AddOrderItemRequest
    {
        [Required(ErrorMessage = "Phải chọn ít nhất một loại item (FoodId, ComboId hoặc BuffetId).")]
        public int? FoodId { get; set; }

        public int? ComboId { get; set; }

        public int? BuffetId { get; set; }

        [Required(ErrorMessage = "Số lượng là bắt buộc.")]
        [Range(1, 30, ErrorMessage = "Số lượng phải từ 1 đến 30.")]
        public int Quantity { get; set; }

        [MaxLength(500, ErrorMessage = "Ghi chú không quá 500 ký tự.")]
        public string? Note { get; set; }
    }

    public class AddOrderItemResponse
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = "Thêm món thành công.";
        public int? OrderItemId { get; set; }
        public string? OrderCode { get; set; }
        public decimal? NewTotalAmount { get; set; }
    }
}
