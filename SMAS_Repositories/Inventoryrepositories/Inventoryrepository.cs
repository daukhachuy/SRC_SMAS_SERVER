using SMAS_BusinessObject.DTOs.InventoryDTO;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.Inventoryrepositories
{
    public class Inventoryrepository : IInventoryrepository
    {
        private readonly InventoryDAO _inventoryDAO;

        public Inventoryrepository(InventoryDAO inventoryDAO)
        {
            _inventoryDAO = inventoryDAO;
        }


        public async Task<IEnumerable<InventoryResponseDTO>> GetAllInventoryAsync()
        {
            var inventories = await _inventoryDAO.GetAllInventoryAsync();
            return inventories.Select(i => new InventoryResponseDTO
            {
                InventoryId = i.InventoryId,
                IngredientId = i.IngredientId,
                BatchCode = i.BatchCode,
                QuantityOnHand = i.QuantityOnHand,
                PricePerUnit = i.PricePerUnit,
                ExpiryDate = i.ExpiryDate,
                WarehouseLocation = i.WarehouseLocation,
                Status = i.Status,
                Note = i.Note,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt,
                IngredientName = i.Ingredient.IngredientName,
                UnitOfMeasurement = i.Ingredient.UnitOfMeasurement,
                WarningLevel = i.Ingredient.WarningLevel
            }).ToList();
        }

        public async Task<IEnumerable<InventorylogResponseDTO>> GetAllInventoryLogsAsync()
        {
            var inventoryLogs = await _inventoryDAO.GetAllInventoryLogsAsync();
            return inventoryLogs.Select(log => new InventorylogResponseDTO
            {
                InventoryLogId = log.InventoryLogId,
                IngredientName = log.Inventory.Ingredient.IngredientName,
                UnitOfMeasurement = log.Inventory.Ingredient.UnitOfMeasurement,
                BatchCode = log.Inventory.BatchCode,
                Action = log.Action,
                OldQuantity = log.OldQuantity,
                NewQuantity = log.NewQuantity,
                Details = log.Details,
                Timestamp = log.Timestamp,
                Fullname = log.User.Fullname


            }).ToList();
        }
    }
}
