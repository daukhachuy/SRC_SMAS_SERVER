using System;

namespace SMAS_BusinessObject.DTOs.ManagerDTO;

public class StaffWorkTodayResponseDTO
{
    public int UserId { get; set; }
    public string Fullname { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public string? Position { get; set; }
    public int ShiftId { get; set; }
    public string? ShiftName { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
}
