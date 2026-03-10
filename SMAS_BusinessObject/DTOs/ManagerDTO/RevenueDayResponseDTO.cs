using System;

namespace SMAS_BusinessObject.DTOs.ManagerDTO;

/// <summary>
/// Doanh thu theo tuần (7 ngày gần nhất)
/// </summary>
public class RevenueWeekResponseDTO
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalRevenue { get; set; }
}
