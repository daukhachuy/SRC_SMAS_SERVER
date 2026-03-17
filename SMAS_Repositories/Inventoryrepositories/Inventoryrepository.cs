using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using SMAS_BusinessObject.DTOs.InventoryDTO;
using SMAS_BusinessObject.Models;
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

        public async Task<List<Inventory>> GetAllAsync()
        {
            return _inventoryDAO.GetAllAsync().Result;
        }

        public async Task<bool> ExportInventoryAsync(ExImportInventoryRequestDTO dto, int staffId)
        {
            var inventorys = await _inventoryDAO.GetAllAsync();
            var inventory = inventorys.FirstOrDefault(i => i.InventoryId == dto.InventoryId);
            if (inventory == null)
                return false;

            if (inventory.QuantityOnHand < dto.Quantity)
                throw new Exception("Not enough stock");

            double oldQty = inventory.QuantityOnHand;

            inventory.QuantityOnHand -= dto.Quantity;
            inventory.UpdatedAt = DateTime.Now;

            var imexport = new ImExport
            {
                Type = "EXPORT",
                InventoryId = inventory.InventoryId,
                IngredientId = inventory.IngredientId,
                Quantity = dto.Quantity,
                Reason = dto.Reason,
                CreatedByStaffId = staffId,
                CreatedAt = DateTime.Now
            };


            var log = new InventoryLog
            {
                InventoryId = inventory.InventoryId,
                IngredientId = inventory.IngredientId,
                Action = "EXPORT",
                OldQuantity = oldQty,
                NewQuantity = inventory.QuantityOnHand,
                UserId = staffId,
                Timestamp = DateTime.Now
            };

            var result = await _inventoryDAO.CreateImportInventoryAsync(inventory, imexport, log);

            return result;
        }

        public async Task<bool> ImportInventoryAsync(ExImportInventoryRequestDTO dto, int staffId)
        {
            var inventorys = await _inventoryDAO.GetAllAsync();
            var inventory = inventorys.FirstOrDefault(i => i.InventoryId == dto.InventoryId);

            if (inventory == null)
                return false;

            double oldQty = inventory.QuantityOnHand;

            inventory.QuantityOnHand += dto.Quantity;
            inventory.UpdatedAt = DateTime.Now;

            var imexport = new ImExport
            {
                Type = "RETURN",
                InventoryId = inventory.InventoryId,
                IngredientId = inventory.IngredientId,
                Quantity = dto.Quantity,
                Reason = dto.Reason,
                CreatedByStaffId = staffId,
                CreatedAt = DateTime.Now
            };


            var log = new InventoryLog
            {
                InventoryId = inventory.InventoryId,
                IngredientId = inventory.IngredientId,
                Action = "RETURN",
                OldQuantity = oldQty,
                NewQuantity = inventory.QuantityOnHand,
                UserId = staffId,
                Timestamp = DateTime.Now
            };


            var result = await _inventoryDAO.CreateImportInventoryAsync(inventory, imexport, log);

            return result;
        }

        public async Task<bool> CreateInventoryAsync(CreateInventoryRequestDTO inventory )
        {
            var newInventory = new Inventory
            {
                IngredientId = inventory.IngredientId,
                BatchCode = inventory.BatchCode,
                QuantityOnHand = inventory.Quantity,
                PricePerUnit = inventory.PricePerUnit,
                ExpiryDate = inventory.ExpiryDate,
                WarehouseLocation = inventory.WarehouseLocation,
                Status = "Active",
                Note = inventory.Note,
                CreatedAt = DateTime.Now
            };
            var result = await _inventoryDAO.CreateInventoryAsync(newInventory);
            return result;
        }
    }
}
