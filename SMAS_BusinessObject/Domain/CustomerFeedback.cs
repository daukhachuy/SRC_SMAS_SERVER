using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.Domain
{
    public class CustomerFeedback
    {
        public int FeedbackId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string FeedbackType { get; set; } = string.Empty;
        public string ResponseStatus { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
