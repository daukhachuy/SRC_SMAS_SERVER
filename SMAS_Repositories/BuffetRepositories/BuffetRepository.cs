using SMAS_BusinessObject.DTOs.BuffetDTO;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.BuffetRepositories
{
    public class BuffetRepository : IBuffetRepository
    {
        private readonly BuffetDAO _buffetDAO;

        public BuffetRepository(BuffetDAO buffetDAO)
        {
            _buffetDAO = buffetDAO;
        }
        public async Task<IEnumerable<BuffetListResponseDTO>> GetAllBuffetsAsync()
        {
            var buffets = await _buffetDAO.GetAllBuffetsAsync();

            return buffets.Select(b => new BuffetListResponseDTO
            {
                BuffetId = b.BuffetId,
                Name = b.Name,
                Description = b.Description,
                MainPrice = b.MainPrice,
                ChildrenPrice = b.ChildrenPrice,
                SidePrice = b.SidePrice,
                Image = b.Image,
                IsAvailable = b.IsAvailable
            }).ToList();
        }

    }
}
