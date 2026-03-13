using SMAS_BusinessObject.DTOs.ContractDTO;
using SMAS_Repositories.ContractRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.ContractService
{
    public class ContractService : IContractService
    {
        private readonly IContractRepository _contractRepository;

        public ContractService(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }

        public async Task<ContractResponseDTO?> GetContractByBookEventCodeAsync(string bookingCode)
        {
            if (string.IsNullOrWhiteSpace(bookingCode))
                throw new ArgumentException("Booking code không được để trống.", nameof(bookingCode));

            return await _contractRepository.GetContractByBookEventCodeAsync(bookingCode);
        }
    }
}
