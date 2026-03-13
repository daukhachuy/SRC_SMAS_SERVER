using SMAS_BusinessObject.DTOs.InventoryDTO;
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
    }
}
