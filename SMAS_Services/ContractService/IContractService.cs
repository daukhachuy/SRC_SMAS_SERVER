using SMAS_BusinessObject.DTOs.ContractDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.ContractService
{
    public interface IContractService
    {
        Task<ContractResponseDTO?> GetContractByBookEventCodeAsync(string bookingCode);
    }
}
