using SMAS_BusinessObject.DTOs.BuffetDTO;
using SMAS_Repositories.BuffetRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SMAS_BusinessObject.DTOs.BuffetDTO.PriceLessThanMainPriceAttribute;

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

        public Task<(BuffetListResponseDTO? Data, string? MsgCode, string? Message)> CreateAsync(
         BuffetCreateDto dto, int? createdBy)
         => _repo.CreateAsync(dto, createdBy);

        public Task<(BuffetListResponseDTO? Data, string? MsgCode, string? Message)> UpdateAsync(
            int id, BuffetUpdateDto dto)
            => _repo.UpdateAsync(id, dto);


        public Task<bool> DeleteAsync(int id)
            => _repo.DeleteAsync(id);

        public Task<bool> UpdateStatusAsync(int id, bool isAvailable)
            => _repo.UpdateStatusAsync(id, isAvailable);

        public Task<(bool Success, string? MsgCode, string? Message)> AddFoodToBuffetAsync(
       int buffetId, BuffetFoodInputDto dto)
       => _repo.AddFoodToBuffetAsync(buffetId, dto);

        public Task<(bool Success, string? MsgCode, string? Message)> RemoveFoodFromBuffetAsync(
            int buffetId, int foodId)
            => _repo.RemoveFoodFromBuffetAsync(buffetId, foodId);
    }
}
