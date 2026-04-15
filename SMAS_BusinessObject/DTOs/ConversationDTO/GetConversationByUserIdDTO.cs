using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.ConversationDTO
{
    public class GetConversationByUserIdDTO
    {
        public int ConversationId { get; set; }

        public int UserId { get; set; }

        public DateTime? LastMessageAt { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool? IsActive { get; set; }
    }
    public class ConversationDTO
    {
        public int ConversationId { get; set; }
        public int UserId { get; set; }

        public string UserName { get; set; } = null!;
        public string? UserAvatar { get; set; }

        public string? LastMessage { get; set; }
        public DateTime? LastMessageAt { get; set; }

        public int UnreadCount { get; set; }
    }
    public class MessageDTO 
    {
        public int MessageId { get; set; }
        public int ConversationId { get; set; }

        public int SenderId { get; set; }
        public string SenderName { get; set; } = null!;

        public string? Content { get; set; }
        public string? AttachmentUrl { get; set; }
        public string MessageType { get; set; } = "text";

        public DateTime? SentAt { get; set; }
        public bool IsRead { get; set; }
    }

    public class SendMessageRequestDTO
    {
        public int ConversationId { get; set; }
        public string? Content { get; set; }
        public string? AttachmentUrl { get; set; }
        public string? MessageType { get; set; } // text | image
    }

    public class GetManagerResponseDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string? Avatar { get; set; }
        public string Position { get; set; }
    }

    public class GetCutomerResponseDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string? Avatar { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
}
