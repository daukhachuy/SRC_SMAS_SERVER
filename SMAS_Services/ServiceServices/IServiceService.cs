using SMAS_BusinessObject.DTOs.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.ServiceServices
{
    public interface IServiceService
    {
        Task<IEnumerable<ServiceListResponse>> GetAllServicesAsync();
        Task<ServiceListResponse?> GetServiceByIdAsync(int serviceId);
        Task<ServiceListResponse> CreateAsync(ServiceCreateDto dto);
        Task<ServiceListResponse> UpdateAsync(int id, ServiceUpdateDto dto);
        Task DeleteAsync(int id);
    }

}
