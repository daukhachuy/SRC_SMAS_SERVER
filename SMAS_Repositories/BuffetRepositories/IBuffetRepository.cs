using SMAS_BusinessObject.DTOs.BuffetDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SMAS_BusinessObject.DTOs.BuffetDTO.PriceLessThanMainPriceAttribute;

namespace SMAS_Repositories.BuffetRepositories
{
    public interface IBuffetRepository
    {
        Task<IEnumerable<BuffetListResponseDTO>> GetAllBuffetsAsync();

        Task<bool> UpdateStatusByBuffetId(int buffetId);
        Task<BuffetListResponseDTO?> GetByIdAsync(int id);
        Task<(BuffetListResponseDTO? Data, string? MsgCode, string? Message)> CreateAsync(BuffetCreateDto dto, int? createdBy);
        Task<(BuffetListResponseDTO? Data, string? MsgCode, string? Message)> UpdateAsync(int id, BuffetUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateStatusAsync(int id, bool isAvailable);
        Task<(bool Success, string? MsgCode, string? Message)> AddFoodToBuffetAsync(int buffetId, BuffetFoodInputDto dto);
        Task<(bool Success, string? MsgCode, string? Message)> RemoveFoodFromBuffetAsync(int buffetId, int foodId);
    }
}
