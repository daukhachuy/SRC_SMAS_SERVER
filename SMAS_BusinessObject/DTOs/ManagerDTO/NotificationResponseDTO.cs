using System;

namespace SMAS_BusinessObject.DTOs.ManagerDTO;

public class NotificationResponseDTO
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
