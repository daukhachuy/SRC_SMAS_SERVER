using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.AIDTO
{
    public  class MenuAnalysisItemDTO
    {
        public MenuActionType Type { get; set; }     
        public string Name { get; set; }            
        public string Reason { get; set; }     
        public string DetailAnalysis { get; set; }
    }

    public enum MenuActionType
    {
        Keep,       // giữ
        Remove,     // bỏ
        Improve,    // cải thiện
        Add         // thêm mới
    }
    public class MenuAnalysisDTO
    {
        public List<MenuAnalysisItemDTO> Items { get; set; }
    }
}
