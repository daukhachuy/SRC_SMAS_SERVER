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

        public SentimentStatsDTO SentimentStats { get; set; }

        public List<IssueDTO> TopIssues { get; set; }

        public List<SuggestionDTO> Suggestions { get; set; }
    }

    public class SentimentStatsDTO
    {
        public double PositivePercent { get; set; }
        public double NeutralPercent { get; set; }
        public double NegativePercent { get; set; }
    }

    public class IssueDTO
    {
        public string IssueName { get; set; }
        public string Category { get; set; }   // Food | Service | Price | Environment
        public double Percent { get; set; }
        public string Severity { get; set; }   // Low | Medium | High
        public string Description { get; set; }
    }

    public class SuggestionDTO
    {
        public string Title { get; set; }
        public string Detail { get; set; }
        public string Priority { get; set; }   // Low | Medium | High
    }
}
