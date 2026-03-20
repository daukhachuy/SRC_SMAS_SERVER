using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.NotificationDTO
{
    public class NotificationDto
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public int? SenderId { get; set; }
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
        public string Type { get; set; } = null!;
        public string? Severity { get; set; }
        public bool? IsRead { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
    }
    public class NotificationQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool? IsRead { get; set; }       // null = lấy hết, true/false = filter
        public string? Type { get; set; }       // Order/Reservation/System/Promotion/Request
    }
    public class PagedResultDto<T>
    {
        public IEnumerable<T> Data { get; set; } = [];
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
