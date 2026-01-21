using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.Auth
{
    public class LoginResponse
    {
        public string? Token { get; set; } 
        public string MsgCode { get; set; } = null!;
    }
}
