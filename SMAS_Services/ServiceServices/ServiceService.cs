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
    }

}
