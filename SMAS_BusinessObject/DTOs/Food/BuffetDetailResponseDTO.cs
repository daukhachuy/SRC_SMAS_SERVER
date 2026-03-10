using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.Food
{
    public class BuffetDetailResponseDTO
    {
        public int BuffetId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal MainPrice { get; set; }
        public decimal? ChildrenPrice { get; set; }
        public decimal? SidePrice { get; set; }
        public string? Image { get; set; }

        public List<BuffetFoodResponseDTO> Foods { get; set; } = new();
    }

    public class BuffetFoodResponseDTO
    {
        public int FoodId { get; set; }
        public string FoodName { get; set; } = null!;
        public decimal Price { get; set; }
        public string? Image { get; set; }

        public int? Quantity { get; set; }
        public bool? IsUnlimited { get; set; }
    }
}
