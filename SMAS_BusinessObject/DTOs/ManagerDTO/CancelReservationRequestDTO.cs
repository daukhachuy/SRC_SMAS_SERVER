using System.ComponentModel.DataAnnotations;

namespace SMAS_BusinessObject.DTOs.ManagerDTO
{
    /// <summary>
    /// Request khi Manager bấm Cancel đặt bàn.
    /// </summary>
    public class CancelReservationRequestDTO
    {
        /// <summary>
        /// Lý do hủy, bắt buộc.
        /// </summary>
        [Required]
        public string CancellationReason { get; set; } = null!;
    }
}

