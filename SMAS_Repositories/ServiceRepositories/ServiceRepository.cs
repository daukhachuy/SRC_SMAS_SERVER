using SMAS_BusinessObject.DTOs.Service;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.ServiceRepositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly ServiceDAO _serviceDAO;

        public ServiceRepository(ServiceDAO serviceDAO)
        {
            _serviceDAO = serviceDAO;
        }

        public async Task<IEnumerable<ServiceListResponse>> GetAllServicesAsync()
        {
            var services = await _serviceDAO.GetAllServicesAsync();
            return services.Select(s => new ServiceListResponse
            {
                ServiceId = s.ServiceId,
                Title = s.Title,
                ServicePrice = s.ServicePrice,
                Description = s.Description,
                Unit = s.Unit,
                Image = s.Image,
                IsAvailable = s.IsAvailable,
                CreatedAt = s.CreatedAt
            });
        }

        public async Task<ServiceListResponse?> GetServiceByIdAsync(int serviceId)
        {
            var s = await _serviceDAO.GetServiceByIdAsync(serviceId);
            if (s == null) return null;
            return new ServiceListResponse
            {
                ServiceId = s.ServiceId,
                Title = s.Title,
                ServicePrice = s.ServicePrice,
                Description = s.Description,
                Unit = s.Unit,
                Image = s.Image,
                IsAvailable = s.IsAvailable,
                CreatedAt = s.CreatedAt
            };
        }
    }

}
