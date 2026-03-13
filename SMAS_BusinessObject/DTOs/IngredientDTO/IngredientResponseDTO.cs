using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.IngredientDTO
{
    public class IngredientResponseDTO
    {
        public int IngredientId { get; set; }

        public string IngredientName { get; set; } = null!;

        public string UnitOfMeasurement { get; set; } = null!;

        public double? WarningLevel { get; set; }

        public double? CurrentStock { get; set; }

        public string? Description { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool? IsActive { get; set; }
    }
}
