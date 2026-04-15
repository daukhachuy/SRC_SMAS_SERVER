using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.DTOs.ConversationDTO;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_DataAccess.DAO
{
    public class ConversationDAO
    {
        private readonly RestaurantDbContext _context;

        public ConversationDAO(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<List<Conversation>> GetAllAsync()
        {
            return await _context.Conversations
                .Include(c => c.User)
                .Include(c => c.Messages)
                .ThenInclude(m => m.Sender)
                .Where(c => c.IsActive == true)
                .ToListAsync();
        }

        public async Task<Conversation?> GetByIdAsync(int id)
        {
            return await _context.Conversations
                .Include(c => c.User)
                .Include(c => c.Messages)
                .ThenInclude(m => m.Sender)
                .FirstOrDefaultAsync(c => c.ConversationId == id);
        }

        //public async Task<Conversation?> GetByUserIdAsync(int userId, int customerid)
        //{
        //    var result = await _context.Conversations
        //        .Include(c => c.User)
        //        .Include(c => c.Messages)
        //        .ThenInclude(m => m.Sender)
        //        .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive == true );
        //    var user = await _context.Staff.Include(u => u.User).Where(s => s.UserId == userId).FirstOrDefaultAsync();
        //    if (user.Position != "Manager")
        //    {
        //        return null;
        //    }
        //    var result2 = result.Messages.Where(m => m.SenderId == customerid).FirstOrDefault();
        //    if (result2 != null) return result;
        //    return result;
        //}

        public async Task<Conversation?> GetByUserIdAsync(int userId, int customerId)
        {
            // check admin có phải manager không
            var user = await _context.Staff
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (user == null || user.Position != "Manager")
                return null;

            // tìm conversation có message giữa admin và customer
            var conversation = await _context.Conversations
                .Include(c => c.Messages)
                .ThenInclude(m => m.Sender)
                .FirstOrDefaultAsync(c =>
                    c.IsActive == true &&
                    c.Messages.Any(m =>
                        (m.SenderId == userId && m.Conversation.UserId == customerId) ||
                        (m.SenderId == customerId)
                    )
                );

            return conversation;
        }

        public async Task<Conversation?> GetByCustomerIdAsync(int customerId)
        {
            // check admin có phải manager không
            var user = await _context.Users
                .FirstOrDefaultAsync(s => s.UserId == customerId);

            if (user == null || user.Role != "Customer")
                return null;

            // tìm conversation có message giữa admin và customer
            var conversation = await _context.Conversations
                .Include(c => c.Messages)
                .ThenInclude(m => m.Sender)
                .FirstOrDefaultAsync(c =>
                    c.IsActive == true &&
                    c.Messages.Any(m =>
                        (m.SenderId == customerId) 
                    )
                );

            return conversation;
        }

        public async Task AddAsync(Conversation conversation)
        {
            await _context.Conversations.AddAsync(conversation);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<List<Message>> GetByConversationIdAsync(int conversationId)
        {
            return await _context.Messages.Include(m => m.Sender)
                .Where(m => m.ConversationId == conversationId)
                .ToListAsync();
        }
        public async Task<List<Message>> GetMessagesByidAsync(int userId)
        {
            var messages = await _context.Messages
                .Where(m => m.SenderId == userId)
                .ToListAsync();
            if (messages == null || messages.Count == 0)
                return new List<Message>();
            var conversationId = messages
                .Select(m => m.ConversationId)
                .FirstOrDefault();

            var result = await _context.Messages.Include(m => m.Sender)
                .Where(m => m.ConversationId == conversationId)
                .ToListAsync();
            return result;
        }

        public async Task AddAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
        }

        public async Task<List<Message>> GetUnreadMessagesAsync(int conversationId, int currentUserId)
        {
            return await _context.Messages
                .Where(m => m.ConversationId == conversationId
                            && m.SenderId != currentUserId
                            && m.IsRead == false)
                .ToListAsync();
        }

        public async Task<List<GetManagerResponseDTO>> GetAllManagerToConversationAsync()
        {
            var manager = await _context.Staff
                .Include(u => u.User)
                .Where(m => m.Position == "Manager" && m.IsWorking == true)
                .ToListAsync();

            return manager.Select(c => new GetManagerResponseDTO
            {
                UserId = c.UserId,
                UserName = c.User.Fullname,
                Avatar = c.User.Avatar,
                Position = c.Position ?? "Manager"
            }).ToList();
        }

        public async Task<List<GetCutomerResponseDTO>> GetAllCustomerToConversationAsync()
        {
            var customer = await _context.Users
                .Where(c => c.IsActive == true && c.Role == "Customer")
                .ToListAsync();
            return customer.Select(c => new GetCutomerResponseDTO
            {
                UserId = c.UserId,
                UserName = c.Fullname,
                Avatar = c.Avatar,
                Phone = c.Phone,
                Email = c.Email
            }).ToList();
        }
    }
}
