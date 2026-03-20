using SMAS_BusinessObject.DTOs.StaffDTO;
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

        private readonly UserDAO _userDAO;

        public StaffProfileRepository(StaffProfileDAO staffProfileDAO, UserDAO userDAO)
        {
            _staffProfileDAO = staffProfileDAO;
            _userDAO = userDAO;
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


        public async Task<IEnumerable<CustomerResponseDTO>> GetAllAcountCustomerAsync()
        {
            var users = await _userDAO.GetAllAcountCustomerAsync();
            return users.Select(u => new CustomerResponseDTO
            {
                UserId = u.UserId,
                Fullname = u.Fullname,
                Phone = u.Phone,
                Email = u.Email,
                Avatar = u.Avatar,
                IsDeleted = u.IsDeleted,
                CreatedAt = u.CreatedAt
            }).ToList(); 
        }

        public async Task<IEnumerable<StaffResponseDTO>> GetAllAcountStaffAsync()
        {
            var users = await _userDAO.GetAllAcountStaffAsync();
            return users.Select(u => new StaffResponseDTO
            {
                UserId = u.UserId,
                Fullname = u.Fullname,
                Phone = u.Phone,
                Email = u.Email,
                Avatar = u.Avatar,
                IsDeleted = u.IsDeleted,
                HireDate = u.Staff?.HireDate ?? default,
                Position = u.Staff?.Position
            }).ToList();
        }



        public async Task<bool> CreateStaffAsync(CreateNewStaffByUseridResquestDTO request)
        {
            var staff = new Staff
            {
                UserId = request.UserId,
                Salary = request.Salary,
                Position = request.Position,
                BankAccountNumber = request.BankAccountNumber,
                BankName = request.BankName,
                TaxId = request.TaxId
            };
            return await _staffProfileDAO.CreateStaffAsync(staff);
        }

        public async Task<bool> CreateStaffWithUserAsync(CreateNewStaffRequestDTO request)
        {
            var datetime = DateTime.Now;
            var user = new User
            {
                Fullname = request.Fullname,
                Role = "Staff",
                Gender = request.Gender,
                Phone = request.Phone,
                Email = request.Email,
                Address = request.Address,
                PasswordHash = request.PasswordHash,
                PasswordSalt = request.PasswordHash,
                CreatedAt = datetime
            };
            var staff = new Staff
            {
                Salary = request.Salary,
                Position = request.Position,
                BankAccountNumber = request.BankAccountNumber,
                BankName = request.BankName,
                TaxId = request.TaxId
            };

            var result = await _staffProfileDAO.CreateStaffWithUserAsync(user, staff);
            return result;
        }
    }
}
