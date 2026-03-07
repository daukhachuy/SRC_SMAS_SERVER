using System;

namespace SMAS_BusinessObject.DTOs.StaffDTO
{

    public class StaffResponse
    {
        public int UserId { get; set; }
        public decimal? Salary { get; set; }
        public string? ExperienceLevel { get; set; }
        public DateOnly HireDate { get; set; }
        public string? Position { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankName { get; set; }
        public decimal? Rating { get; set; }
        public bool? IsWorking { get; set; }
        public string? TaxId { get; set; }

        /// <summary>
        /// Thông tin User liên quan (không chứa PasswordHash, PasswordSalt).
        /// </summary>
        public StaffUserInfo? User { get; set; }
    }

    public class StaffUserInfo
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
        public string? Role { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
