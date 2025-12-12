using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.Domain
{
    public class Messages
    {
        public int MessageId { get; set; }             // PK

        public int ConversationId { get; set; }         // FK 

        public int SenderId { get; set; }               // 

        public string MessageType { get; set; } = string.Empty;  // 

        public string Content { get; set; } = string.Empty;

        public DateTime SentAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsRead { get; set; }
    }
}
