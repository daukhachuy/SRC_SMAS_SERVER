using SMAS_BusinessObject.DTOs.StaffDTO;
using SMAS_DataAccess.DAO;

namespace SMAS_Repositories.StaffRepositories
{
    public class StaffRepository : IStaffRepository
    {
        private readonly StaffDAO _staffDAO;

        public StaffRepository(StaffDAO staffDAO)
        {
            _staffDAO = staffDAO;
        }

        public async Task<StaffResponse?> GetStaffByIdAsync(int userId)
        {
            var staff = await _staffDAO.GetStaffByIdAsync(userId);
            if (staff == null) return null;

            return new StaffResponse
            {
                UserId = staff.UserId,
                Salary = staff.Salary,
                ExperienceLevel = staff.ExperienceLevel,
                HireDate = staff.HireDate,
                Position = staff.Position,
                BankAccountNumber = staff.BankAccountNumber,
                BankName = staff.BankName,
                Rating = staff.Rating,
                IsWorking = staff.IsWorking,
                TaxId = staff.TaxId,
                User = staff.User == null ? null : new StaffUserInfo
                {
                    UserId = staff.User.UserId,
                    Fullname = staff.User.Fullname,
                    Gender = staff.User.Gender,
                    Dob = staff.User.Dob,
                    Phone = staff.User.Phone,
                    Email = staff.User.Email,
                    Address = staff.User.Address,
                    Avatar = staff.User.Avatar,
                    CreatedAt = staff.User.CreatedAt,
                    UpdatedAt = staff.User.UpdatedAt,
                    Role = staff.User.Role,
                    IsActive = staff.User.IsActive,
                    IsDeleted = staff.User.IsDeleted,
                    DeletedAt = staff.User.DeletedAt
                }
            };
        }
    }
}
