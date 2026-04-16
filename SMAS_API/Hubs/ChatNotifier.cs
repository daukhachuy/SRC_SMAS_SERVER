using Microsoft.AspNetCore.SignalR;
using SMAS_BusinessObject.DTOs.ConversationDTO;
using SMAS_Services.Realtime;

namespace SMAS_API.Hubs
{
    public class ChatNotifier : IChatNotifier
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatNotifier(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyNewMessage(int conversationId, MessageDTO message)
        {
            await _hubContext.Clients.Group($"conversation_{conversationId}")
                .SendAsync("ReceiveMessage", message);
        }

        public async Task NotifyMessagesRead(int conversationId, int readByUserId)
        {
            await _hubContext.Clients.Group($"conversation_{conversationId}")
                .SendAsync("MessagesRead", new { conversationId, readByUserId, readAt = DateTime.UtcNow });
        }

        public async Task NotifyNewConversation(int targetUserId, ConversationDTO conversation)
        {
            await _hubContext.Clients.Group($"user_{targetUserId}")
                .SendAsync("NewConversation", conversation);
        }
    }
}
