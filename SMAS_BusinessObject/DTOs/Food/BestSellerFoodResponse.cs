using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.Food
{
    public class BestSellerFoodResponse
    {
        public int FoodId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? PromotionalPrice { get; set; }
        public string? Image { get; set; }
        public string? Unit { get; set; }
        public int? PreparationTime { get; set; }
        public int? Calories { get; set; }
        public int OrderCount { get; set; }   // số lượng đã bán
        public decimal? Rating { get; set; }
        public bool IsAvailable { get; set; }
    }
}
