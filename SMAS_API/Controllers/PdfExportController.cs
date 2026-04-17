using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.PDFDTO;
using SMAS_Services.PdfServices;

namespace SMAS_API.Controllers
{
    [Route("api/pdf-export")]
    [ApiController]
    public class PdfExportController : Controller
    {

        private readonly IPdfService _pdfService;

        public PdfExportController(IPdfService pdfService)
        {
            _pdfService = pdfService;
        }

        //  API xuất hóa đơn
        [HttpGet("invoice/{ordercode}")]
        public async Task<IActionResult> ExportInvoiceAsync(string ordercode)
        {
            var pdfBytes = await _pdfService.ExportInvoicePdfAsync(ordercode.Trim());
            if (pdfBytes == null || pdfBytes.Length == 0) return NotFound($"Không tìm thấy hóa đơn có mã là :  {ordercode} ");
            return File(pdfBytes, "application/pdf", $"invoice_{ordercode}.pdf");
        }

        //  API xuất hợp đồng
        [HttpGet("contract/{contraccode}")]
        public async Task<IActionResult> ExportContractAsync(string contraccode)
        {
            var pdfBytes = await _pdfService.ExportContractPdfAsync(contraccode.Trim());
            if (pdfBytes == null || pdfBytes.Length == 0) return NotFound($"Không tìm thấy hợp đồng có mã là : {contraccode}");
            return File(pdfBytes, "application/pdf", $"contract_{contraccode}.pdf");
        }
    }
}
