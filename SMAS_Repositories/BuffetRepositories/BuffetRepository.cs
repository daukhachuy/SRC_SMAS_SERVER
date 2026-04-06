using SMAS_BusinessObject.DTOs.BuffetDTO;
using SMAS_BusinessObject.Models;
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


        public async Task<bool> UpdateStatusByBuffetId(int buffetId)
        {
            return await _buffetDAO.UpdateStatusByBuffetId(buffetId);
        }
        public async Task<BuffetListResponseDTO?> GetByIdAsync(int id)
        {
            var buffet = await _buffetDAO.GetByIdAsync(id);
            return buffet == null ? null : MapToResponseDto(buffet);
        }

        public async Task<BuffetListResponseDTO> CreateAsync(BuffetCreateDto dto)
        {
            var buffet = MapFromCreateDto(dto);
            var created = await _buffetDAO.CreateAsync(buffet);
            return MapToResponseDto(created);
        }

        public async Task<BuffetListResponseDTO?> UpdateAsync(int id, BuffetUpdateDto dto)
        {
            var buffet = await _buffetDAO.GetByIdAsync(id);
            if (buffet == null) return null;

            MapFromUpdateDto(dto, buffet);
            var updated = await _buffetDAO.UpdateAsync(buffet);
            return MapToResponseDto(updated);
        }

        public Task<bool> DeleteAsync(int id)
            => _buffetDAO.DeleteAsync(id);

        public Task<bool> UpdateStatusAsync(int id, bool isAvailable)
            => _buffetDAO.UpdateStatusAsync(id, isAvailable);

        // -------------------------------------------------------
        // Mapping helpers
        // -------------------------------------------------------

        private static BuffetListResponseDTO MapToResponseDto(Buffet b) => new BuffetListResponseDTO
        {
            BuffetId = b.BuffetId,
            Name = b.Name,
            Description = b.Description,
            MainPrice = b.MainPrice,
            ChildrenPrice = b.ChildrenPrice,
            SidePrice = b.SidePrice,
            Image = b.Image,
            IsAvailable = b.IsAvailable,
            CreatedBy = b.CreatedBy,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt
        };

        private static Buffet MapFromCreateDto(BuffetCreateDto dto) => new Buffet
        {
            Name = dto.Name,
            Description = dto.Description,
            MainPrice = dto.MainPrice,
            ChildrenPrice = dto.ChildrenPrice,
            SidePrice = dto.SidePrice,
            Image = dto.Image,
            IsAvailable = dto.IsAvailable ?? true,
            CreatedBy = dto.CreatedBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        private static void MapFromUpdateDto(BuffetUpdateDto dto, Buffet buffet)
        {
            buffet.Name = dto.Name;
            buffet.Description = dto.Description;
            buffet.MainPrice = dto.MainPrice;
            buffet.ChildrenPrice = dto.ChildrenPrice;
            buffet.SidePrice = dto.SidePrice;
            buffet.Image = dto.Image;
            buffet.IsAvailable = dto.IsAvailable;
            buffet.UpdatedAt = DateTime.UtcNow;
        }
    }
}
