using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.DTOs.NotificationDTO;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_DataAccess.DAO
{
    public class NotificationDAO
    {
        private readonly RestaurantDbContext _context;

        public NotificationDAO(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateNotificationAsync(Notification notification)
        {
            try
            {
                await _context.Notifications.AddAsync(notification);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating notification: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<Notification>> GetAllAsync()
      => await _context.Notifications
          .AsNoTracking()
          .OrderByDescending(n => n.CreatedAt)
          .ToListAsync();


        public async Task<bool> UpdateNotificationAsync(Notification notification)
        {
            try
            {
                _context.Notifications.Update(notification);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating notification: {ex.Message}");
                return false;
            }
        }
    }
}
