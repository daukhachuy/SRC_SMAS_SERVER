using SMAS_BusinessObject.DTOs.ConversationDTO;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.ConversationServices
{
    public interface IConversationService
    {
        Task<List<ConversationDTO>> GetConversationsAsync();
        Task<List<ConversationDTO>> GetConversationsByUseridAsync(int userid);
        Task<List<MessageDTO>> GetMessagesAsync(int conversationId);
        Task<List<MessageDTO>> GetMessagesByidAsync(int userid);
        Task<MessageDTO> SendMessageAsync(SendMessageRequestDTO request, int sendid);
        Task<bool> MarkAsReadAsync(int conversationId, int currentUserId);
        Task<ConversationDTO> CreateConversationAsync(int userId ,int customerid);
        Task<ConversationDTO> CreateConversationByCustomerAsync(int userId, int managerid);
        Task<List<GetManagerResponseDTO>> GetAllManagerToConversationAsync();
        Task<List<GetCutomerResponseDTO>> GetAllCustomerToConversationAsync();
    }
}
