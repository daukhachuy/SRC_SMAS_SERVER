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
    }
}
