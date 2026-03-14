using SMAS_Repositories.StaffRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.StaffService
{
    public class StaffProfileService : IStaffProfileService
    {
        private readonly IStaffProfileRepository _staffProfileRepository;

        public StaffProfileService(IStaffProfileRepository staffProfileRepository)
        {
            _staffProfileRepository = staffProfileRepository;
        }

        public async Task<StaffProfileDto?> GetProfileStaffAsync(int userId)
            => await _staffProfileRepository.GetProfileAsync(userId);

        public async Task<(bool Success, string? ErrorMessage)> UpdateProfileStaffAsync(int userId, UpdateProfileStaffRequestDto dto)
        {
            // Validate
            if (!string.IsNullOrWhiteSpace(dto.Phone) && dto.Phone.Length < 9)
                return (false, "Số điện thoại không hợp lệ.");

            if (!string.IsNullOrWhiteSpace(dto.Email) && !dto.Email.Contains('@'))
                return (false, "Email không hợp lệ.");

            if (dto.Dob.HasValue && dto.Dob.Value > DateOnly.FromDateTime(DateTime.Today))
                return (false, "Ngày sinh không hợp lệ.");

            return await _staffProfileRepository.UpdateProfileAsync(userId, dto);
        }
    }

}
