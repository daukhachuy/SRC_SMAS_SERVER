using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.CategoryDTO
{
    public class CategoryResponse
    {
        public int CategoryId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public bool? IsProcessedGoods { get; set; }

        public string? Image { get; set; }

        public bool? IsAvailable { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
    public class CategoryCreateDto
    {
        [Required(ErrorMessage = "Tên danh mục không được để trống.")]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }
        public bool? IsProcessedGoods { get; set; }
        public string? Image { get; set; }
        public bool? IsAvailable { get; set; } = true;
    }

    public class CategoryUpdateDto
    {
        [Required(ErrorMessage = "Tên danh mục không được để trống.")]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }
        public bool? IsProcessedGoods { get; set; }
        public string? Image { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
