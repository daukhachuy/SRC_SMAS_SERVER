using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.Domain
{
    public class Category
    {
        public int CategoryId { get; set; }           // PK

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsProcessedGoods { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsAvailable { get; set; }
    }
}
