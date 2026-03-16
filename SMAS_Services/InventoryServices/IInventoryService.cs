using SMAS_BusinessObject.DTOs.InventoryDTO;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.InventoryServices
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryResponseDTO>> GetAllInventoryAsync();

        Task<IEnumerable<InventorylogResponseDTO>> GetAllInventoryLogsAsync();

        Task<string> GetNewBatchCodeAsync();

        Task<bool> ExportInventoryAsync(ExImportInventoryRequestDTO dto, int staffId);
        Task<bool> ImportInventoryAsync(ExImportInventoryRequestDTO dto, int staffId);
        Task<bool> CreateInventoryAsync(CreateInventoryRequestDTO inventory);
    }
}
