using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.Domain
{
    public class Table
    {
        public int TableId { get; set; }              // PK

        public string TableName { get; set; } = string.Empty;

        public string TableType { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? QRCode { get; set; }

        public int NumberOfPeople { get; set; }
    }
}
