using SMAS_BusinessObject.DTOs.StaffDTO;

namespace SMAS_Repositories.StaffRepositories
{
    public interface IStaffRepository
    {
        /// <summary>
        /// Lấy thông tin staff theo id (UserId).
        /// </summary>
        /// <param name="userId">UserId của staff</param>
        /// <returns>StaffResponse hoặc null nếu không tồn tại</returns>
        Task<StaffResponse?> GetStaffByIdAsync(int userId);
    }
}
