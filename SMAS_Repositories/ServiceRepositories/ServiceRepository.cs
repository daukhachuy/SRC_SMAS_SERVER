using SMAS_BusinessObject.DTOs.Service;
using SMAS_BusinessObject.Models;
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
        public async Task<ServiceResponse> CreateAsync(ServiceCreateDto dto)
        {
            var entity = MapToEntity(dto);
            var created = await _dao.CreateAsync(entity);
            return MapToDto(created);
        }

        public async Task<ServiceResponse> UpdateAsync(int id, ServiceUpdateDto dto)
        {
            var entity = await _dao.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Service with id {id} not found.");

            ApplyUpdate(entity, dto);
            var updated = await _dao.UpdateAsync(entity);
            return MapToDto(updated);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dao.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Service with id {id} not found.");

            // SOFT DELETE
            entity.IsAvailable = false;
            await _dao.UpdateAsync(entity);
        }

        // ==================== MAPPERS ====================
        private static ServiceResponse MapToDto(Service e) => new()
        {
            ServiceId = e.ServiceId,
            Title = e.Title,
            ServicePrice = e.ServicePrice,
            Description = e.Description,
            Unit = e.Unit,
            Image = e.Image,
            IsAvailable = e.IsAvailable,
            CreatedAt = e.CreatedAt
        };

        private static Service MapToEntity(ServiceCreateDto dto) => new()
        {
            Title = dto.Title.Trim(),
            ServicePrice = dto.ServicePrice,
            Description = dto.Description,
            Unit = dto.Unit,
            Image = dto.Image,
            IsAvailable = dto.IsAvailable ?? true,
            CreatedAt = DateTime.UtcNow
        };

        private static void ApplyUpdate(Service entity, ServiceUpdateDto dto)
        {
            entity.Title = dto.Title.Trim();
            entity.ServicePrice = dto.ServicePrice;
            entity.Description = dto.Description;
            entity.Unit = dto.Unit;
            entity.Image = dto.Image;
            entity.IsAvailable = dto.IsAvailable;
            // CreatedAt gi? nguyên (không update)
        }
    }

}
