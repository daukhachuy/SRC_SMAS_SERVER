using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.Food
{
    public class FoodFilterResponseDTO
    {
        public int FoodId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? PromotionalPrice { get; set; }
        public string? Image { get; set; }
        public string? Unit { get; set; }
        public decimal? Rating { get; set; }
        public string? Note { get; set; }
    }
}
