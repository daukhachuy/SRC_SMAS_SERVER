using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.BlogDTo
{
    public class BlogResponse
    {
        public int BlogId { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string? Image { get; set; }

        public int? ViewCount { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? PublishedAt { get; set; }

        public string Fullname { get; set; } = null!;
    }
}
