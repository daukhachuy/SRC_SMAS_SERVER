using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMAS_BusinessObject.DTOs.ContractDTO;
using SMAS_BusinessObject.Models;

namespace SMAS_Repositories.ContractRepository
{
    public interface IContractRepository
    {
        Task<ContractResponseDTO?> GetContractByBookEventCodeAsync(string bookingCode);
    }
}
