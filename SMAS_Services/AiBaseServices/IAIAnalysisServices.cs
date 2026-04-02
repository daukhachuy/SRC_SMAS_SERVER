using SMAS_BusinessObject.DTOs.AIDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.AiBaseServices
{
    public interface IAIAnalysisServices
    {
        Task<FeedbackSummaryDTO> AnalyzeFeedbackLast3Months();

        Task<MenuAnalysisDTO> AnalyzeMenuLast3Months();

        Task<ComboAnalysisDTO> AnalyzeComboAsync();
    }
}
