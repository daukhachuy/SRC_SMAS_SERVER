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
    }
}
