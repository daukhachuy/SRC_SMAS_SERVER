using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.DTOs.BookEventDTO
{
    public class BookEventCheckInRequestDTO
    {
        /// <summary>
        /// Danh sách bàn manager chọn để giữ chỗ cho sự kiện.
        /// Phải đúng bằng NumberOfGuests (đang lưu như số bàn).
        /// </summary>
        public List<int> TableIds { get; set; } = new();
    }

    public class BookEventCheckInResponseDTO
    {
        public int BookEventId { get; set; }
        public string? BookingCode { get; set; }
        public string? OrderCode { get; set; }
        public string? Status { get; set; }
        public DateTime? CheckInAt { get; set; }
        public List<int> TableIds { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }

    public class BookEventCheckoutResponseDTO
    {
        public int BookEventId { get; set; }
        public string? BookingCode { get; set; }
        public string? Status { get; set; }
        public DateTime? CheckOutAt { get; set; }
        public List<int> ReleasedTableIds { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }
}
