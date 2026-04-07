using SMAS_BusinessObject.DTOs.BuffetDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.BuffetRepositories
{
    public interface IBuffetRepository
    {
        Task<IEnumerable<BuffetListResponseDTO>> GetAllBuffetsAsync();

        Task<bool> UpdateStatusByBuffetId(int buffetId);
        Task<BuffetListResponseDTO?> GetByIdAsync(int id);
        Task<BuffetListResponseDTO> CreateAsync(BuffetCreateDto dto);
        Task<BuffetListResponseDTO?> UpdateAsync(int id, BuffetUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateStatusAsync(int id, bool isAvailable);
    }
}
