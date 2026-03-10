using System;

namespace SMAS_BusinessObject.DTOs.ManagerDTO;

public class TableEmptyResponseDTO
{
    public int TableId { get; set; }
    public string TableName { get; set; } = null!;
    public string? TableType { get; set; }
    public int NumberOfPeople { get; set; }
    public string? Status { get; set; }
    public string? QrCode { get; set; }
    public bool? IsActive { get; set; }
}
