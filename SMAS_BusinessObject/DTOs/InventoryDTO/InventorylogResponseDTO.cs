using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.InventoryDTO
{
    public class InventorylogResponseDTO
    {
        public int InventoryLogId { get; set; }

        public string IngredientName { get; set; } = null!;

        public string UnitOfMeasurement { get; set; } = null!;

        public string? BatchCode { get; set; }

        public string Action { get; set; } = null!;

        public double? OldQuantity { get; set; }

        public double? NewQuantity { get; set; }

        public string? Details { get; set; }

        public DateTime? Timestamp { get; set; }

        public string Fullname { get; set; } = null!;
    }
}
