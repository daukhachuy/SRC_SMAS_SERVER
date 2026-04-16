using Azure.Core;
using SMAS_BusinessObject.DTOs.ConversationDTO;
using SMAS_BusinessObject.Models;
using SMAS_Repositories.ConversationRepositories;
using SMAS_Services.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.ConversationServices
{
    public class ConversationService : IConversationService
    {
        public readonly IConversationRepository _conversationRepo;
        private readonly IChatNotifier _chatNotifier;

        public ConversationService(IConversationRepository conversationRepo, IChatNotifier chatNotifier)
        {
            _conversationRepo = conversationRepo;
            _chatNotifier = chatNotifier;
        }
        public async Task<List<ConversationDTO>> GetConversationsAsync()
        {
            var conversations = await _conversationRepo.GetAllAsync();

            var customerid = conversations.Select(c => c.Messages.Select(m => m.SenderId)).SelectMany(ids => ids).Distinct().ToList();
            var customername = conversations.Select(c => c.Messages.Select(m => m.Sender.Fullname)).SelectMany(names => names).Distinct().ToList();
            var customeravatar = conversations.Select(c => c.Messages.Select(m => m.Sender.Avatar)).SelectMany(avatars => avatars).Distinct().ToList();
            return conversations.Select(c =>
            {
                var lastCustomerMessage = c.Messages
                    .Where(m => m.SenderId != c.UserId)
                    .OrderByDescending(m => m.SentAt)
                    .FirstOrDefault();

                var lastMessage = c.Messages
                    .OrderByDescending(m => m.SentAt)
                    .FirstOrDefault();

                return new ConversationDTO
                {
                    ConversationId = c.ConversationId,
                    UserId = c.UserId,
                    UserName = c.User.Fullname,
                    UserAvatar = c.User.Avatar,

                    CustomerId = lastCustomerMessage?.SenderId ?? 0,
                    CustomerName = lastCustomerMessage?.Sender.Fullname ?? "Unknown",
                    CustomerAvatar = lastCustomerMessage?.Sender.Avatar,

                    LastMessage = lastMessage?.Content,
                    LastMessageAt = c.LastMessageAt,

                    UnreadCount = c.Messages.Count(m => (m.IsRead == false) && m.SenderId != c.UserId)
                };
            })
             .OrderByDescending(c => c.LastMessageAt)
             .ToList();
        }

        public async Task<List<ConversationDTO>> GetConversationsByUseridAsync(int userid)
        {
            var conversations = await GetConversationsAsync();

            return conversations
                .Where(c => c.UserId == userid)
                .ToList();
        }

        public async Task<List<MessageDTO>> GetMessagesAsync(int conversationId)
        {
            var messages = await _conversationRepo.GetByConversationIdAsync(conversationId);

            return messages
                .Where(m => m.IsDeleted != true)
                .OrderBy(m => m.SentAt)
                .Select(m => new MessageDTO
                {
                    MessageId = m.MessageId,
                    ConversationId = m.ConversationId,
                    SenderId = m.SenderId,
                    SenderName = m.Sender.Fullname,
                    Content = m.Content,
                    AttachmentUrl = m.AttachmentUrl,
                    MessageType = m.MessageType,
                    SentAt = m.SentAt,
                    IsRead = m.IsRead ?? false,
                }).ToList();
        }

        public async Task<List<MessageDTO>> GetMessagesByidAsync(int userid)
        {
            var messages = await _conversationRepo.GetMessagesByidAsync(userid);

            return messages
                .Where(m => m.IsDeleted != true)
                .OrderBy(m => m.SentAt)
                .Select(m => new MessageDTO
                {
                    MessageId = m.MessageId,
                    ConversationId = m.ConversationId,
                    SenderId = m.SenderId,
                    SenderName = m.Sender.Fullname,
                    Content = m.Content,
                    AttachmentUrl = m.AttachmentUrl,
                    MessageType = m.MessageType,
                    SentAt = m.SentAt,
                    IsRead = m.IsRead ?? false,
                }).ToList();
        }

        public async Task<MessageDTO> SendMessageAsync(SendMessageRequestDTO request , int sendid)
        {
            var conversation = await _conversationRepo.GetByIdAsync(request.ConversationId);
            if (conversation == null)
                throw new Exception("Conversation not found");

            var message = new Message
            {
                ConversationId = request.ConversationId,
                Content = request.Content,
                AttachmentUrl = request.AttachmentUrl,
                MessageType = request.MessageType ?? "string",
                SentAt = DateTime.UtcNow,
                IsRead = false,
                IsDeleted = false
            };
            message.SenderId = sendid;

            await _conversationRepo.AddAsync(message);

            conversation.LastMessageAt = DateTime.UtcNow;
            conversation.UpdatedAt = DateTime.UtcNow;

            await _conversationRepo.SaveAsync();

            var dto = new MessageDTO
            {
                MessageId = message.MessageId,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                Content = message.Content,
                AttachmentUrl = message.AttachmentUrl,
                MessageType = message.MessageType,
                SentAt = message.SentAt,
                IsRead = message.IsRead ?? false
            };

            await _chatNotifier.NotifyNewMessage(request.ConversationId, dto);

            return dto;
        }

        public async Task<bool> MarkAsReadAsync(int conversationId, int currentUserId)
        {
            var messages = await _conversationRepo.GetUnreadMessagesAsync(conversationId, currentUserId);
            try
            {
                foreach (var msg in messages)
                {
                    msg.IsRead = true;
                }
                await _conversationRepo.SaveAsync();

                await _chatNotifier.NotifyMessagesRead(conversationId, currentUserId);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error marking messages as read: {ex.Message}");
                return false;

            }
        }


        public async Task<ConversationDTO> CreateConversationAsync(int userId, int customerid)
        {
            var existing = await _conversationRepo.GetByUserIdAsync(userId, customerid);
            if (existing != null)
            {
                return null;
            }
            var conversation = new Conversation
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,

            };

            await _conversationRepo.AddAsync(conversation);
            await _conversationRepo.SaveAsync();
            var message = new SendMessageRequestDTO
            {
                ConversationId = conversation.ConversationId,
                Content = "Xin chào, Chúng tôi là quản lý nhà hàng SMAS muốn liên lạc với bạn !",
                MessageType = "string"

            };
            await SendMessageAsync(message, userId);

            var messageresponse = new SendMessageRequestDTO
            {
                ConversationId = conversation.ConversationId,
                Content = "",
                MessageType = ""

            };
            await SendMessageAsync(message, customerid);
            return new ConversationDTO
            {
                ConversationId = conversation.ConversationId,
                UserId = conversation.UserId,
                UserName = conversation.User.Fullname,
                UserAvatar = conversation.User.Avatar,
                LastMessage = message.Content,
                LastMessageAt = conversation.LastMessageAt,
                UnreadCount = 0
            };

            await _chatNotifier.NotifyNewConversation(customerid, dto);

            return dto;
        }
        public async Task<ConversationDTO> CreateConversationByCustomerAsync(int userId, int managerid)
        {
            var existing =await _conversationRepo.GetByCustomerIdAsync(userId);
            if (existing != null)
            {
                return null;
            }
            var conversation = new Conversation
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,

            };

            await _conversationRepo.AddAsync(conversation);
            await _conversationRepo.SaveAsync();
            var message = new SendMessageRequestDTO
            {
                ConversationId = conversation.ConversationId,
                Content = "Xin chào, Chúng tôi muốn liên lạc với bạn !",
                MessageType = "string"

            };
            await SendMessageAsync(message, userId);
            var dto = new ConversationDTO
            {
                ConversationId = conversation.ConversationId,
                UserId = conversation.UserId,
                UserName = conversation.User.Fullname,
                UserAvatar = conversation.User.Avatar,
                LastMessage = message.Content,
                LastMessageAt = conversation.LastMessageAt,
                UnreadCount = 0
            };

            await _chatNotifier.NotifyNewConversation(managerid, dto);

            return dto;
        }

        public async Task<List<GetManagerResponseDTO>> GetAllManagerToConversationAsync() => await _conversationRepo.GetAllManagerToConversationAsync();

        public async Task<List<GetCutomerResponseDTO>> GetAllCustomerToConversationAsync() => await _conversationRepo.GetAllCustomerToConversationAsync();
    }
}
