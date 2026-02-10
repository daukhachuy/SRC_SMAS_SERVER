using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.Profile
{
    public class UpdateProfileResponse
    {
        public bool Success { get; set; }
        public string MsgCode { get; set; } = string.Empty;
    }
}
