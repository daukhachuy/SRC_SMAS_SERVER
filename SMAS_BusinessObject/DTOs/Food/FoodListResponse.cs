using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.Food
{
    public class FoodListResponse
    {
        public int FoodId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public decimal? PromotionalPrice { get; set; }

        public string? Image { get; set; }

        public string? Unit { get; set; }

        public bool? IsAvailable { get; set; }

        public bool? IsDirectSale { get; set; }

        public bool? IsFeatured { get; set; }

        public int? PreparationTime { get; set; }

        public int? Calories { get; set; }

        public int? ViewCount { get; set; }

        public int? OrderCount { get; set; }

        public decimal? Rating { get; set; }

        public string? Note { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public List<CategoryFoodListResponse> Categories { get; set; } = new();
    }

    public class CategoryFoodListResponse
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool? IsProcessedGoods { get; set; }
        public string? Image { get; set; }
        public bool? IsAvailable { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
