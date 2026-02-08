using SMAS_BusinessObject.DTOs.BlogDTo;
using SMAS_Repositories.BlogRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.BlogServices
{
    public class BlogService : IBlogServices
    {

        private readonly IBlogRepository _blogRepository;

        public BlogService(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<IEnumerable<BlogResponse>> GetAllBlogsAsync()
        {
            return await _blogRepository.GetAllBlogsAsync();
        }
    }
}
