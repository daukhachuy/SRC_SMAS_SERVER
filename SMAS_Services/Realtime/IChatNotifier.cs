using SMAS_BusinessObject.DTOs.ConversationDTO;

namespace SMAS_Services.Realtime
{
    public interface IChatNotifier
    {
        Task NotifyNewMessage(int conversationId, MessageDTO message);

        Task NotifyMessagesRead(int conversationId, int readByUserId);

        Task NotifyNewConversation(int targetUserId, ConversationDTO conversation);
    }
}
