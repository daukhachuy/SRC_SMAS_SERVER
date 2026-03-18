using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.NotificationDTO
{
    public class ChangeWorkstaffRequestDTO
    {
        public int? SenderId { get; set; }
        public string Title { get; set; } = null!;
        public string? Content { get; set; }

    }
}
