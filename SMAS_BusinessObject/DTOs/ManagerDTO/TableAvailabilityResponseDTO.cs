using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.DTOs.ManagerDTO;

public class TableAvailabilityResponseDTO
{
    public DateOnly Date { get; set; }
    public List<TimeSlotAvailabilityDTO> TimeSlots { get; set; } = new();
}

public class TimeSlotAvailabilityDTO
{
    public string TimeSlotName { get; set; } = null!;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public List<ReservationSlotDTO> Reservations { get; set; } = new();
    public List<BookEventSlotDTO> BookEvents { get; set; } = new();

    public SlotSummaryDTO Summary { get; set; } = new();
}

public class SlotSummaryDTO
{
    public int TotalGuests { get; set; }
    public int TotalBookedTables { get; set; }
    public int ActiveTables { get; set; }
    public double CapacityPercentage { get; set; }
}

public class ReservationSlotDTO
{
    public string? ReservationCode { get; set; }
    public int NumberOfGuests { get; set; }
    public TimeOnly ReservationTime { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? OrderCode { get; set; }
}

public class BookEventSlotDTO
{
    public string? BookingCode { get; set; }
    public int NumberOfGuests { get; set; }
    public TimeOnly ReservationTime { get; set; }
    public string? CustomerName { get; set; }
    public string? EventTitle { get; set; }
    public string? OrderCode { get; set; }
}
