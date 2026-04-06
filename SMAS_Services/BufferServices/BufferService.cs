using SMAS_BusinessObject.DTOs.BuffetDTO;
using SMAS_Repositories.BuffetRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.BufferServices
{
    public class BufferService : IBufferServices
    {
        private readonly IBuffetRepository _repo;

        public BufferService(IBuffetRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<BuffetListResponseDTO>> GetAllBuffetsAsync()
        {
            return await _repo.GetAllBuffetsAsync();
        }
        public async Task<bool> UpdateStatusByBuffetId(int buffetId)
        {
            return await _repo.UpdateStatusByBuffetId(buffetId);
        }

        public Task<BuffetListResponseDTO?> GetByIdAsync(int id)
            => _repo.GetByIdAsync(id);

        public Task<BuffetListResponseDTO> CreateAsync(BuffetCreateDto dto)
            => _repo.CreateAsync(dto);

        public Task<BuffetListResponseDTO?> UpdateAsync(int id, BuffetUpdateDto dto)
            => _repo.UpdateAsync(id, dto);

        public Task<bool> DeleteAsync(int id)
            => _repo.DeleteAsync(id);

        public Task<bool> UpdateStatusAsync(int id, bool isAvailable)
            => _repo.UpdateStatusAsync(id, isAvailable);
    }
}
