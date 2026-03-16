using SMAS_BusinessObject.DTOs.InventoryDTO;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.Inventoryrepositories
{
    public interface IInventoryrepository
    {
        Task<IEnumerable<InventoryResponseDTO>> GetAllInventoryAsync();
        Task<IEnumerable<InventorylogResponseDTO>> GetAllInventoryLogsAsync();
        Task<List<Inventory>> GetAllAsync();
        Task<bool> ExportInventoryAsync(ExImportInventoryRequestDTO dto, int staffId);
        Task<bool> ImportInventoryAsync(ExImportInventoryRequestDTO dto, int staffId);
        Task<bool> CreateInventoryAsync(CreateInventoryRequestDTO inventory);
    }
}
