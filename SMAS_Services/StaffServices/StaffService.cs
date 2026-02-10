using SMAS_BusinessObject.DTOs.StaffDTO;
using SMAS_Repositories.StaffRepositories;

namespace SMAS_Services.StaffServices
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepository;

        public StaffService(IStaffRepository staffRepository)
        {
            _staffRepository = staffRepository;
        }

        public async Task<StaffResponse?> GetStaffByIdAsync(int userId)
        {
            return await _staffRepository.GetStaffByIdAsync(userId);
        }
    }
}
