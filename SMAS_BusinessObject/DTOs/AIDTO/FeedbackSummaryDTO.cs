using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.AIDTO
{
    public class FeedbackSummaryDTO
    {
        public string Summary { get; set; }
        public SentimentStats SentimentStats { get; set; }
        public List<string> TopIssues { get; set; }
        public List<string> Suggestions { get; set; }
    }

    public class SentimentStats
    {
        public int Positive { get; set; }
        public int Neutral { get; set; }
        public int Negative { get; set; }
    }
}
