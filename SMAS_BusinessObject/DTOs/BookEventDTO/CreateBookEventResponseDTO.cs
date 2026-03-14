namespace SMAS_BusinessObject.DTOs.BookEventDTO;

public class CreateBookEventResponseDTO
{
    public int BookEventId { get; set; }
    public string BookingCode { get; set; } = null!;
    public string? Message { get; set; }
}
