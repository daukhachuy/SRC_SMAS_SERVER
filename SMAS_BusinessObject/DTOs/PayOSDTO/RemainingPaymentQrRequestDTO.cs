using System.ComponentModel.DataAnnotations;

namespace SMAS_BusinessObject.DTOs.PayOSDTO;

public class RemainingPaymentQrRequestDTO
{
    [Required(ErrorMessage = "OrderCode không được để trống.")]
    public string OrderCode { get; set; } = null!;

    [Required(ErrorMessage = "ReturnUrl là bắt buộc.")]
    [Url(ErrorMessage = "ReturnUrl phải là URL hợp lệ.")]
    public string ReturnUrl { get; set; } = null!;

    [Required(ErrorMessage = "CancelUrl là bắt buộc.")]
    [Url(ErrorMessage = "CancelUrl phải là URL hợp lệ.")]
    public string CancelUrl { get; set; } = null!;
}
