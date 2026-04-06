using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.CustomerDTO
{
    public class CustomerDetailResponseDTO
    {
        public int UserId { get; set; }

        public string Fullname { get; set; } = null!;

        public string? Gender { get; set; }

        public DateOnly? Dob { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public string? Avatar { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int totalOrders { get; set; }

        public int totalOrderCancel { get; set; }

        public int totalOrderNoShow { get; set; }

        public decimal totalSpending { get; set; }
    }
}
