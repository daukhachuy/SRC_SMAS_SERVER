using SMAS_BusinessObject.DTOs.BlogDTo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.BlogServices
{
    public interface IBlogServices
    {

        Task<IEnumerable<BlogResponse>> GetAllBlogsAsync();
    }
}
