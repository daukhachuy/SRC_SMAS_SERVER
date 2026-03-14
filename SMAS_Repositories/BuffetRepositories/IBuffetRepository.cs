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
    }
}
