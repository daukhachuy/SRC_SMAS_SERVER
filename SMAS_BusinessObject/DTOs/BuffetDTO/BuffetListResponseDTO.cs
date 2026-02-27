using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.BuffetDTO
{
    public class BuffetListResponseDTO
    {
        public int BuffetId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal MainPrice { get; set; }
        public decimal? ChildrenPrice { get; set; }
        public decimal? SidePrice { get; set; }
        public string? Image { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
