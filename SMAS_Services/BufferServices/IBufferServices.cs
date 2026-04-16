using SMAS_BusinessObject.DTOs.BuffetDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SMAS_BusinessObject.DTOs.BuffetDTO.PriceLessThanMainPriceAttribute;

namespace SMAS_Services.BufferServices
{
    public interface IBufferServices
    {
        Task<IEnumerable<BuffetListResponseDTO>> GetAllBuffetsAsync();
        Task<bool> UpdateStatusByBuffetId(int buffetId);
        Task<BuffetListResponseDTO?> GetByIdAsync(int id);
        public Task<(BuffetListResponseDTO? Data, string? MsgCode, string? Message)> CreateAsync( BuffetCreateDto dto, int? createdBy);

        public Task<(BuffetListResponseDTO? Data, string? MsgCode, string? Message)> UpdateAsync( int id, BuffetUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateStatusAsync(int id, bool isAvailable);

        public Task<(bool Success, string? MsgCode, string? Message)> AddFoodToBuffetAsync(int buffetId, BuffetFoodInputDto dto);

        public Task<(bool Success, string? MsgCode, string? Message)> RemoveFoodFromBuffetAsync(int buffetId, int foodId);
    }
}
