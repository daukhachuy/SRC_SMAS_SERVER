using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public int? AuthorId { get; set; } 
    }
    public class BlogCreateDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title must not exceed 200 characters")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; } = null!;

        public string? Image { get; set; }

        public int? ViewCount { get; set; }

        public string? Status { get; set; }

        public DateTime? PublishedAt { get; set; }

        [Required(ErrorMessage = "AuthorId is required")]
        public int AuthorId { get; set; }
    }

    public class BlogUpdateDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title must not exceed 200 characters")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; } = null!;

        public string? Image { get; set; }
        public int? ViewCount { get; set; }
        public string? Status { get; set; }
        public DateTime? PublishedAt { get; set; }
    }
}
