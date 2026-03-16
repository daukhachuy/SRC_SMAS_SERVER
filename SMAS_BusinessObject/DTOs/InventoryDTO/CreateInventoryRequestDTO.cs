using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.InventoryDTO
{
    public class CreateInventoryRequestDTO
    {
        [Required(ErrorMessage = "IngredientId không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "IngredientId phải lớn hơn 0")]
        public int IngredientId { get; set; }

        [Required(ErrorMessage = "Mã lô không được để trống")]
        [StringLength(50, ErrorMessage = "BatchCode tối đa 50 ký tự")]
        public string BatchCode { get; set; } = null!;

        [Required(ErrorMessage = "Số lượng không được để trống")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "Quantity phải lớn hơn 0")]
        public double Quantity { get; set; }

        [Required(ErrorMessage = "Giá không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "PricePerUnit phải lớn hơn 0")]
        public decimal PricePerUnit { get; set; }
        [Required(ErrorMessage = "Ngày hết hạn  không được để trống")]
        public DateOnly? ExpiryDate { get; set; }
        [Required(ErrorMessage = "Vị trí kho không được để trống")]
        [StringLength(100, ErrorMessage = "Vị trí kho tối đa 100 ký tự")]
        public string? WarehouseLocation { get; set; }

        [StringLength(500, ErrorMessage = "Note tối đa 500 ký tự")]
        public string? Note { get; set; }
    }
}
