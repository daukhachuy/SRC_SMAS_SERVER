using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.Domain
{
    public class Discount
    {
        public int DiscountId { get; set; }                  // PK

        public string Code { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string DiscountType { get; set; } = string.Empty;   // Percent / Fixed

        public decimal Value { get; set; }                

        public decimal MinOrderAmount { get; set; }         

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int UsageLimit { get; set; }                  

        public int UsedCount { get; set; }                   

        public string Status { get; set; } = string.Empty;   // Active, Expired, Disabled...
    }
}
