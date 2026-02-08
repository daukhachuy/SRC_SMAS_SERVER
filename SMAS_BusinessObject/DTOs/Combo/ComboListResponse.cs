using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.Combo
{
    public class ComboListResponse
    {
        public int ComboId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPercent { get; set; }
        public string? Image { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? ExpiryDate { get; set; }
        public int? NumberOfUsed { get; set; }
        public int? MaxUsage { get; set; }
        public bool? IsAvailable { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<ComboFoodItemDto> Foods { get; set; } = new List<ComboFoodItemDto>();
    }

    public class ComboFoodItemDto
    {
        public int FoodId { get; set; }
        public string FoodName { get; set; } = null!;
        public string? FoodImage { get; set; }
        public int Quantity { get; set; }
        public decimal? FoodPrice { get; set; }  
    }
}
