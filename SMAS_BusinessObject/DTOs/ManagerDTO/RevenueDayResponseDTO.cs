using System;

namespace SMAS_BusinessObject.DTOs.ManagerDTO;

/// <summary>
/// Doanh thu theo tuần (7 ngày gần nhất)
/// </summary>
public class DailyRevenueDTO
{
    public string DayLabel { get; set; } = "";
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
}

public class RevenueWeekResponseDTO
{
    public List<DailyRevenueDTO> Days { get; set; } = new();
    public decimal TotalRevenue { get; set; }
}
