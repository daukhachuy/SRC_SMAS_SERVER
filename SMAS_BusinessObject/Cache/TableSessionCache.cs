using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.Cache
{
    // Phiên bàn lưu trong IMemoryCache - key: "table_session_{tableCode}"
    public class TableSessionCache
    {
        public string TableCode { get; set; } = null!;
        public int TableId { get; set; }
        public string SessionNonce { get; set; } = null!; // UUID mới mỗi lần mở bàn
        public string Status { get; set; } = "ACTIVE";    // ACTIVE | CLOSED
        public int OpenedBy { get; set; }
        public DateTime OpenedAt { get; set; }
        public DateTime ExpiresAt { get; set; }           // 12 giờ
    }
}
