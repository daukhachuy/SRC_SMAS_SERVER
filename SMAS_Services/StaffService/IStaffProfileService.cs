using SMAS_BusinessObject.DTOs.StaffDTO;
using SMAS_BusinessObject.Models;
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

        Task<IEnumerable<CustomerResponseDTO>> GetAllAcountCustomerAsync();

        Task<IEnumerable<StaffResponseDTO>> GetAllAcountStaffAsync();

        Task<IEnumerable<StaffResponseDTO>> FilterAccountStaffAsync(FilterAccountStaffRequestDTO filter);

        Task<IEnumerable<CustomerResponseDTO>> FilterAccountCustomerAsync(bool request);

        Task<bool> CreateStaffAsync(CreateNewStaffByUseridResquestDTO request);

        Task<bool> CreateStaffWithUserAsync(CreateNewStaffRequestDTO request);

        Task<StaffDetailresponseDTO> GetStaffDetailToUpdateAsync(int userId);

        Task<bool> AdminUpdateStaffDetail(StaffDetailRequestDTO request);
    }

}
