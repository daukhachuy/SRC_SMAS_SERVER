using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.PdfServices
{
    public interface IPdfService 
    {
        Task<byte[]> ExportInvoicePdfAsync(string invoiceId);
        Task<byte[]> ExportContractPdfAsync(string contractId);
    }
}
