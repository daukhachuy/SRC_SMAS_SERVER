using SMAS_BusinessObject.DTOs.StaffDTO;
using SMAS_BusinessObject.Models;
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

        Task<IEnumerable<CustomerResponseDTO>> GetAllAcountCustomerAsync();

        Task<IEnumerable<StaffResponseDTO>> GetAllAcountStaffAsync();

        Task<bool> CreateStaffAsync(CreateNewStaffByUseridResquestDTO request);

        Task<bool> CreateStaffWithUserAsync(CreateNewStaffRequestDTO request);

        Task<StaffDetailresponseDTO> GetStaffDetailToUpdateAsync(int userId);

        Task<bool> AdminUpdateStaffDetail(StaffDetailRequestDTO request);
    }

}
