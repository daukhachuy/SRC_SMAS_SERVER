using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.AIDTO
{
    public class ComboAnalysisDTO
    {
        public List<ComboAnalysisItemDTO> Items { get; set; }
    }
    public class ComboAnalysisItemDTO
    {
        public string Type { get; set; }
        public string ComboName { get; set; }
        public List<string> Foods { get; set; }
        public string Reason { get; set; }
        public string DetailAnalysis { get; set; }

        public decimal OriginalPrice { get; set; }     
        public decimal SuggestedPrice { get; set; }    
        public int DiscountPercent { get; set; }
    }

}
