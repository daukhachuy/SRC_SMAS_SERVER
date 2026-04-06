using SMAS_BusinessObject.DTOs.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.ServiceRepositories
{
    public interface IServiceRepository
    {
        Task<IEnumerable<ServiceListResponse>> GetAllServicesAsync();
        Task<ServiceListResponse?> GetServiceByIdAsync(int serviceId);
        Task<ServiceListResponse> CreateAsync(ServiceCreateDto dto);
        Task<ServiceListResponse> UpdateAsync(int id, ServiceUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateStatusAsync(int id, bool isAvailable);
    }

}
