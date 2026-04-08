using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMAS_DataAccess.DAO
{
    public class ServiceDAO
    {
        private readonly RestaurantDbContext _context;

        public ServiceDAO(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<List<SMAS_BusinessObject.Models.Service>> GetAllServicesAsync()
        {
            return await _context.Services.ToListAsync();
        }

        public async Task<SMAS_BusinessObject.Models.Service?> GetServiceByIdAsync(int serviceId)
        {
            return await _context.Services
                .FirstOrDefaultAsync(s => s.ServiceId == serviceId && (s.IsAvailable == true || s.IsAvailable == null));
        }

        public async Task<Service> CreateAsync(Service service)
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            return service;
        }

        public async Task<Service> UpdateAsync(Service service)
        {
            _context.Services.Update(service);
            await _context.SaveChangesAsync();
            return service;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null) return false;

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return true;
        }

        // Patch status: ch? c?p nh?t IsAvailable
        public async Task<bool> UpdateStatusAsync(int id, bool isAvailable)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null) return false;

            service.IsAvailable = !service.IsAvailable;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
