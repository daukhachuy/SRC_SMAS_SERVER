using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.DTOs.InventoryDTO;
using SMAS_Repositories.Inventoryrepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.InventoryServices
{
    public class InventoryService : IInventoryService
    {

        private readonly IInventoryrepository _inventoryrepository;

        public InventoryService(IInventoryrepository inventoryrepository)
        {
            _inventoryrepository = inventoryrepository;
        }


        public async Task<IEnumerable<InventoryResponseDTO>> GetAllInventoryAsync()
        {
            var data = await _inventoryrepository.GetAllInventoryAsync();
            return data.OrderByDescending(x => x.CreatedAt);
        }

        public async Task<IEnumerable<InventorylogResponseDTO>> GetAllInventoryLogsAsync()
        {
            var data =  await _inventoryrepository.GetAllInventoryLogsAsync();
            return data.OrderByDescending(x => x.Timestamp);
        }

        public async Task<string> GetNewBatchCodeAsync()
        {
            var inventories = await _inventoryrepository.GetAllInventoryAsync();

            var year = DateTime.Now.Year;

            var lastBatch = inventories
                .Where(i => i.BatchCode != null && i.BatchCode.StartsWith($"BCH-{year}"))
                .OrderByDescending(i => i.BatchCode)
                .FirstOrDefault();

            int nextNumber = 1;

            if (lastBatch != null)
            {
                var parts = lastBatch.BatchCode.Split('-');
                var numberPart = parts[2];
                nextNumber = int.Parse(numberPart) + 1;
            }

            return $"BCH-{year}-{nextNumber:D4}";
        }

        public async Task<bool> ExportInventoryAsync(ExImportInventoryRequestDTO dto, int staffId)
        {
            return await _inventoryrepository.ExportInventoryAsync(dto, staffId);
        }
        public async Task<bool> ImportInventoryAsync(ExImportInventoryRequestDTO dto, int staffId)
        {
            return await _inventoryrepository.ImportInventoryAsync(dto, staffId);
        }
        public async Task<bool> CreateInventoryAsync(CreateInventoryRequestDTO inventory)
        {
             return await _inventoryrepository.CreateInventoryAsync(inventory);
        }
    }
}
