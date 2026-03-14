using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.StaffRepository
{
    public class StaffProfileRepository : IStaffProfileRepository
    {
        private readonly StaffProfileDAO _staffProfileDAO;

        public StaffProfileRepository(StaffProfileDAO staffProfileDAO)
        {
            _staffProfileDAO = staffProfileDAO;
        }

        public async Task<StaffProfileDto?> GetProfileAsync(int userId)
        {
            var user = await _staffProfileDAO.GetProfileAsync(userId);
            if (user == null) return null;
            return MapToDto(user);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateProfileAsync(int userId, UpdateProfileStaffRequestDto dto)
        {
            var user = await _staffProfileDAO.GetForUpdateAsync(userId);
            if (user == null)
                return (false, "Không tìm thấy nhân viên.");

            // Cập nhật User
            if (!string.IsNullOrWhiteSpace(dto.Fullname)) user.Fullname = dto.Fullname;
            if (!string.IsNullOrWhiteSpace(dto.Phone)) user.Phone = dto.Phone;
            if (!string.IsNullOrWhiteSpace(dto.Email)) user.Email = dto.Email;
            if (!string.IsNullOrWhiteSpace(dto.Gender)) user.Gender = dto.Gender;
            if (dto.Dob != null) user.Dob = dto.Dob;
            if (!string.IsNullOrWhiteSpace(dto.Address)) user.Address = dto.Address;
            if (!string.IsNullOrWhiteSpace(dto.AvatarUrl)) user.Avatar = dto.AvatarUrl;
            user.UpdatedAt = DateTime.Now;

            // Cập nhật Staff (thông tin nhận lương)
            if (user.Staff != null)
            {
                if (!string.IsNullOrWhiteSpace(dto.BankAccountNumber)) user.Staff.BankAccountNumber = dto.BankAccountNumber;
                if (!string.IsNullOrWhiteSpace(dto.BankName)) user.Staff.BankName = dto.BankName;
            }

            await _staffProfileDAO.SaveAsync();
            return (true, null);
        }

        private static StaffProfileDto MapToDto(User u) =>
            new()
            {
                FullName = u.Fullname,
                AvatarUrl = u.Avatar,
                Phone = u.Phone,
                Email = u.Email,
                Gender = u.Gender,
                Dob = u.Dob,
                Address = u.Address,
                Position = u.Staff?.Position,
                ExperienceLevel = u.Staff?.ExperienceLevel,
                HireDate = u.Staff?.HireDate,
                BankAccountNumber = u.Staff?.BankAccountNumber,
                BankName = u.Staff?.BankName,

                // Thông tin do quản trị viên quản lý - chỉ đọc
                Role = u.Role,
                TaxId = u.Staff?.TaxId
            };
    }
}
