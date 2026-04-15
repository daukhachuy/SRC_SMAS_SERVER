using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.AIDTO
{
    public class MenuAnalysisDTO
    {
        public List<MenuAnalysisItemDTO> Items { get; set; }
    }

    public class MenuAnalysisItemDTO
    {
        public string Type { get; set; }
        public string Level { get; set; }
        public string Name { get; set; }
        public string Reason { get; set; }
        public string DetailAnalysis { get; set; }
    }
}

