using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.Domain
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public string QRCode { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreateAt { get; set; }
        public string? Note { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
