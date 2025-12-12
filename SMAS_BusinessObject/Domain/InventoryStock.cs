using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.Domain
{
    public class InventoryStock
    {
        public int StockId { get; set; }
        public int QuantityOnHand { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public string? Note { get; set; }
        public decimal PricePerUnit { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
