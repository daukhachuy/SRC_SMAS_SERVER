using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.Auth
{
    public class GoogleRegisterRequest
    {
        [Required]
        public string Token { get; set; }  
    }
}
