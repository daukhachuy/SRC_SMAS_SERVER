using SMAS_BusinessObject.DTOs.Service;
using SMAS_Repositories.ServiceRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.ServiceServices
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;

        public ServiceService(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public async Task<IEnumerable<ServiceListResponse>> GetAllServicesAsync()
        {
            return await _serviceRepository.GetAllServicesAsync();
        }

        public async Task<ServiceListResponse?> GetServiceByIdAsync(int serviceId)
        {
            return await _serviceRepository.GetServiceByIdAsync(serviceId);
        }
        public async Task<ServiceListResponse> CreateAsync(ServiceCreateDto dto)
        {
            return await _serviceRepository.CreateAsync(dto);
        }

        public async Task<ServiceListResponse> UpdateAsync(int id, ServiceUpdateDto dto)
        {
            return await _serviceRepository.UpdateAsync(id, dto);
        }

        public async Task DeleteAsync(int id)
            => await _serviceRepository.DeleteAsync(id);
    }

}
