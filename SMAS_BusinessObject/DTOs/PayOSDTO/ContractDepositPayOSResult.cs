namespace SMAS_BusinessObject.DTOs.PayOSDTO;

public class ContractDepositPayOSResult
{
    public bool Success { get; set; }
    public string? CheckoutUrl { get; set; }
    public string? Message { get; set; }
}
