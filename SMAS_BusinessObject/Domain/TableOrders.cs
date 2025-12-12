using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.Domain
{
    public class TableOrders
    {
        public int TableId { get; set; }          // FK + FK

        public int OrderId { get; set; }          // + FK

        public string? IsMainLink { get; set; }

        public string? QROrder { get; set; }
    }
}
