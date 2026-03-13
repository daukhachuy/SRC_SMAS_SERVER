using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.InventoryDTO
{
    public class InventoryResponseDTO
    {
        public int InventoryId { get; set; }

        public int IngredientId { get; set; }

        public string? BatchCode { get; set; }

        public double QuantityOnHand { get; set; }

        public decimal PricePerUnit { get; set; }

        public DateOnly? ExpiryDate { get; set; }

        public string? WarehouseLocation { get; set; }

        public string? Status { get; set; }

        public string? Note { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string IngredientName { get; set; } = null!;

        public string UnitOfMeasurement { get; set; } = null!;

        public double? WarningLevel { get; set; }
    }
}
