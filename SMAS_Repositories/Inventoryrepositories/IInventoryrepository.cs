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
    }
}
