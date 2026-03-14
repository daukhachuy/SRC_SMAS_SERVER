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
    }
}
