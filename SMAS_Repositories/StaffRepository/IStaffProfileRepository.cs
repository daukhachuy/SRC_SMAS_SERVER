using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.StaffRepository
{
    public interface IStaffProfileRepository
    {
        Task<StaffProfileDto?> GetProfileAsync(int userId);
        Task<(bool Success, string? ErrorMessage)> UpdateProfileAsync(int userId, UpdateProfileStaffRequestDto dto);
    }

}
