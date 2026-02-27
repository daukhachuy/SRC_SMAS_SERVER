using SMAS_BusinessObject.DTOs.BuffetDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.BufferServices
{
    public interface IBufferServices
    {
        Task<IEnumerable<BuffetListResponseDTO>> GetAllBuffetsAsync();
    }
}
