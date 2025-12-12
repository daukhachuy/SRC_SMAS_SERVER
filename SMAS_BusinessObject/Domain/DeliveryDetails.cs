using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.Domain
{
    public class DeliveryDetails
    {
        public int DeliveryId { get; set; }
        public string Address { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string DeliveryStatus { get; set; } = string.Empty;
        public string? Note { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
