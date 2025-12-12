using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.Domain
{
    public class Food
    {
        public int FoodId { get; set; }               // PK

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string? Image { get; set; }

        public bool IsAvailable { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? Note { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int Stock { get; set; }

    }
}
