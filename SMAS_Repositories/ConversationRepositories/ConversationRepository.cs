using Microsoft.EntityFrameworkCore.Query.Internal;
using SMAS_BusinessObject.DTOs.ConversationDTO;
using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.ConversationRepositories
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly ConversationDAO _conversationDAO;

        public ConversationRepository(ConversationDAO conversationDAO)
        {
            _conversationDAO = conversationDAO;
        }

        public async  Task<List<Conversation>> GetAllAsync() => await _conversationDAO.GetAllAsync();
        public async  Task<Conversation?> GetByIdAsync(int id) => await _conversationDAO.GetByIdAsync(id);
        public async Task<Conversation?> GetByUserIdAsync(int userId , int customerid)  => await _conversationDAO.GetByUserIdAsync(userId , customerid);

        public async Task<Conversation?> GetByCustomerIdAsync(int customerid) => await _conversationDAO.GetByCustomerIdAsync( customerid);
        public async Task AddAsync(Conversation conversation) => await _conversationDAO.AddAsync(conversation);
        public async Task<List<Message>> GetByConversationIdAsync(int conversationId) => await _conversationDAO.GetByConversationIdAsync(conversationId);

        public async  Task<List<Message>> GetMessagesByidAsync(int userid)  => await _conversationDAO.GetMessagesByidAsync(userid);
        public async Task AddAsync(Message message) => await _conversationDAO.AddAsync(message);
        public async Task<List<Message>> GetUnreadMessagesAsync(int conversationId, int currentUserId) => await _conversationDAO.GetUnreadMessagesAsync(conversationId, currentUserId);
        public async Task<List<GetManagerResponseDTO>> GetAllManagerToConversationAsync() => await _conversationDAO.GetAllManagerToConversationAsync();

        public async Task<List<GetCutomerResponseDTO>> GetAllCustomerToConversationAsync() => await _conversationDAO.GetAllCustomerToConversationAsync();
        public async Task SaveAsync() => await _conversationDAO.SaveAsync();
    }
}
