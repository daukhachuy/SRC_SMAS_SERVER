using System.ComponentModel.DataAnnotations;

namespace SMAS_BusinessObject.DTOs.PayOSDTO;

/// <summary>
/// Request từ Frontend khi tạo link thanh toán PayOS (sau khi đã tạo đơn hàng).
/// </summary>
public class CreatePaymentLinkRequest
{
    [Required(ErrorMessage = "OrderId là bắt buộc.")]
    public int OrderId { get; set; }

    [Required(ErrorMessage = "ReturnUrl là bắt buộc.")]
    [Url(ErrorMessage = "ReturnUrl phải là URL hợp lệ.")]
    public string ReturnUrl { get; set; } = null!;

    [Required(ErrorMessage = "CancelUrl là bắt buộc.")]
    [Url(ErrorMessage = "CancelUrl phải là URL hợp lệ.")]
    public string CancelUrl { get; set; } = null!;
}
