using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.OrderDTO
{
    public class ChooseAssignedStaffRequestDTO
    {
        [Required(ErrorMessage = "Mã đơn hàng không được bỏ trống ")]
        public string OrderCode { get; set; }
        [Required(ErrorMessage = "Id nhân viên không được bỏ trống ")]
        public int StaffId { get; set; }
    }
}
