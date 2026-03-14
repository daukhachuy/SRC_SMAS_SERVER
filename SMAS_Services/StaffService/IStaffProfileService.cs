using SMAS_Repositories.StaffRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.StaffService
{
    public interface IStaffProfileService
    {
        Task<StaffProfileDto?> GetProfileStaffAsync(int userId);
        Task<(bool Success, string? ErrorMessage)> UpdateProfileStaffAsync(int userId, UpdateProfileStaffRequestDto dto);
    }

}
