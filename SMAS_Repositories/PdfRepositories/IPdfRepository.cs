using SMAS_BusinessObject.DTOs.BookEventDTO;
using SMAS_BusinessObject.DTOs.ContractDTO;
using SMAS_BusinessObject.DTOs.PDFDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.PdfRepositories
{
    public interface IPdfRepository
    {
        Task<PdfInvoiceDTO> GetInvoiceByIdAsync(string invoiceId);
        Task<ContractResponseDTO> GetContractByIdAsync(string contractId);
        Task<BookEventListResponseDTO> GetBookEventdetailAsync(string contractId);
    }
}
