using SMAS_BusinessObject.DTOs.ConversationDTO;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.ConversationRepositories
{
    public interface IConversationRepository
    {
        Task<List<Conversation>> GetAllAsync();
        Task<Conversation?> GetByIdAsync(int id);
        Task<Conversation?> GetByUserIdAsync(int userId , int customerid);
        Task AddAsync(Conversation conversation);
        Task<List<Message>> GetByConversationIdAsync(int conversationId);

        Task<List<Message>> GetMessagesByidAsync(int userid);
        Task AddAsync(Message message);
        Task<List<Message>> GetUnreadMessagesAsync(int conversationId, int currentUserId);
        Task<Conversation?> GetByCustomerIdAsync(int customerid);
        Task<List<GetManagerResponseDTO>> GetAllManagerToConversationAsync();

        Task<List<GetCutomerResponseDTO>> GetAllCustomerToConversationAsync();
        Task SaveAsync();
    }
}
