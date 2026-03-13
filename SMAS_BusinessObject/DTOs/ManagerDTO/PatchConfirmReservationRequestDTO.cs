using System.ComponentModel.DataAnnotations;

namespace SMAS_BusinessObject.DTOs.ManagerDTO
{
    /// <summary>
    /// Request cho PATCH confirm/cancel đặt bàn (Manager).
    /// Confirm → status Confirmed. Cancel → status Cancelled và ghi CancellationReason.
    /// </summary>
    public class PatchConfirmReservationRequestDTO
    {
        /// <summary>
        /// Confirm hoặc Cancel
        /// </summary>
        [Required]
        public string Action { get; set; } = null!; // "Confirm" | "Cancel"

        /// <summary>
        /// Bắt buộc khi Action = Cancel
        /// </summary>
        public string? CancellationReason { get; set; }
    }
}
