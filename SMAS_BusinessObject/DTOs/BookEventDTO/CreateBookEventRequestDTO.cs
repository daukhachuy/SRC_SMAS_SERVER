using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.DTOs.BookEventDTO;

/// <summary>
/// Request từ client khi gọi API đặt sự kiện. Không có CustomerId (lấy từ JWT).
/// </summary>
public class CreateBookEventApiRequestDTO
{
    /// <summary>Số bàn (lưu vào NumberOfGuests trong DB).</summary>
    public int NumberOfGuests { get; set; }

    public DateOnly ReservationDate { get; set; }
    public TimeOnly ReservationTime { get; set; }

    /// <summary>Ghi chú thêm về sự kiện.</summary>
    public string? Note { get; set; }

    /// <summary>Khu vực (vd: Trong nhà máy lạnh).</summary>
    public string? Area { get; set; }

    /// <summary>Loại sự kiện (EventId).</summary>
    public int EventId { get; set; }

    /// <summary>Dịch vụ đi kèm (MC, đèn, ...).</summary>
    public List<BookEventServiceItemDTO> Services { get; set; } = new();

    /// <summary>Món ăn cho mỗi bàn.</summary>
    public List<EventFoodItemDTO> Foods { get; set; } = new();
}

/// <summary>
/// Request nội bộ (có CustomerId từ JWT) dùng cho Service.
/// </summary>
public class CreateBookEventRequestDTO
{
    public int CustomerId { get; set; }

    /// <summary>Số bàn (lưu vào NumberOfGuests trong DB).</summary>
    public int NumberOfGuests { get; set; }

    public DateOnly ReservationDate { get; set; }
    public TimeOnly ReservationTime { get; set; }

    /// <summary>Ghi chú chung. Có thể gộp thêm khu vực (Area) vào đây nếu cần.</summary>
    public string? Note { get; set; }

    /// <summary>Khu vực (vd: Trong nhà máy lạnh). Có thể lưu vào Note khi gửi API.</summary>
    public string? Area { get; set; }

    /// <summary>Bước 2: Loại sự kiện.</summary>
    public int EventId { get; set; }

    /// <summary>Bước 2: Dịch vụ đi kèm (MC, đèn, ...).</summary>
    public List<BookEventServiceItemDTO> Services { get; set; } = new();

    /// <summary>Bước 3: Món ăn cho mỗi bàn (từ EventFood/Food).</summary>
    public List<EventFoodItemDTO> Foods { get; set; } = new();
}

public class BookEventServiceItemDTO
{
    public int ServiceId { get; set; }
    public int Quantity { get; set; } = 1;
    public string? Note { get; set; }
}

public class EventFoodItemDTO
{
    public int FoodId { get; set; }
    public int Quantity { get; set; }
    public string? Note { get; set; }
}
