using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.Domain
{
    public class Blog
    {
        public int BlogId { get; set; }                  // PK

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int AuthorId { get; set; }

        public int RestaurantId { get; set; }            // FK
    }
}
