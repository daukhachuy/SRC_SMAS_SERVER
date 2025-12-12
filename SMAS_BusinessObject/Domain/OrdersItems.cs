using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.Domain
{
    public class OrdersItems
    {
        public int OrdersItemId { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
        public int ProductId { get; set; }
        public TimeSpan OpeningTime { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
