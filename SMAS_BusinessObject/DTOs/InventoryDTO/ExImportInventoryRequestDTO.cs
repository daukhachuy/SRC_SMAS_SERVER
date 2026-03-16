using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.InventoryDTO
{
    public class ExImportInventoryRequestDTO
    {
        [Required(ErrorMessage = "InventoryId không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "InventoryId phải lớn hơn 0")]
        public int InventoryId { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống")]
        [Range(0.0001,2000, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public double Quantity { get; set; }

        [StringLength(255, ErrorMessage = "Reason tối đa 255 ký tự")]
        public string? Reason { get; set; }
    }
}
