using SMAS_BusinessObject.DTOs.Combo;
using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.ComboRepositories
{
    public class ComboRepository : IComboRepository
    {
        private readonly ComboDAO _comboDAO;

        public ComboRepository(ComboDAO comboDAO)
        {
            _comboDAO = comboDAO;
        }

        public async Task<IEnumerable<ComboListResponse>> GetAvailableComboListAsync()
        {
            var combos = await _comboDAO.GetAvailableCombosWithFoodsAsync();

            return combos.Select(c => new ComboListResponse
            {
                ComboId = c.ComboId,
                Name = c.Name,
                Description = c.Description,
                Price = c.Price,
                DiscountPercent = c.DiscountPercent,
                Image = c.Image,
                StartDate = c.StartDate,
                ExpiryDate = c.ExpiryDate,
                NumberOfUsed = c.NumberOfUsed,
                MaxUsage = c.MaxUsage,
                IsAvailable = c.IsAvailable,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,

                Foods = c.ComboFoods.Select(cf => new ComboFoodItemDto
                {
                    FoodId = cf.FoodId,
                    FoodName = cf.Food.Name,
                    FoodImage = cf.Food.Image,
                    Quantity = cf.Quantity,
                    FoodPrice = cf.Food.Price   // tùy chọn: lấy giá món gốc
                }).ToList()
            });
        }

        public async Task<bool> UpdateStatusByComboId(int comboId)
        {
            return await _comboDAO.UpdateStatusByComboId(comboId);
        }
        public async Task<IEnumerable<ComboListResponse>> GetAllAsync()
        {
            var combos = await _comboDAO.GetAllAsync();
            return combos.Select(MapToResponseDto);
        }

        public async Task<ComboListResponse?> GetByIdAsync(int id)
        {
            var combo = await _comboDAO.GetByIdAsync(id);
            return combo == null ? null : MapToResponseDto(combo);
        }

        public async Task<ComboListResponse> CreateAsync(ComboCreateDto dto)
        {
            var combo = MapFromCreateDto(dto);
            var created = await _comboDAO.CreateAsync(combo);
            return MapToResponseDto(created);
        }

        public async Task<ComboListResponse?> UpdateAsync(int id, ComboUpdateDto dto)
        {
            var combo = await _comboDAO.GetByIdAsync(id);
            if (combo == null) return null;

            MapFromUpdateDto(dto, combo);
            var updated = await _comboDAO.UpdateAsync(combo);
            return MapToResponseDto(updated);
        }

        public Task<bool> DeleteAsync(int id)
            => _comboDAO.DeleteAsync(id);

        public Task<bool> UpdateStatusAsync(int id, bool isAvailable)
            => _comboDAO.UpdateStatusAsync(id, isAvailable);

        // -------------------------------------------------------
        // Mapping helpers
        // -------------------------------------------------------

        private static ComboListResponse MapToResponseDto(Combo c) => new ComboListResponse
        {
            ComboId = c.ComboId,
            Name = c.Name,
            Description = c.Description,
            Price = c.Price,
            DiscountPercent = c.DiscountPercent,
            Image = c.Image,
            StartDate = c.StartDate,
            ExpiryDate = c.ExpiryDate,
            NumberOfUsed = c.NumberOfUsed,
            MaxUsage = c.MaxUsage,
            IsAvailable = c.IsAvailable,
            CreatedBy = c.CreatedBy,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
             Foods = c.ComboFoods?.Select(cf => new ComboFoodItemDto
             {
                 FoodId = cf.FoodId,
                 FoodName = cf.Food?.Name,
                 FoodImage = cf.Food?.Image,
                 Quantity = cf.Quantity,
                 FoodPrice = cf.Food?.Price ?? 0
             }).ToList() ?? new List<ComboFoodItemDto>()
        };

        private static Combo MapFromCreateDto(ComboCreateDto dto) => new Combo
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            DiscountPercent = dto.DiscountPercent,
            Image = dto.Image,
            StartDate = dto.StartDate,
            ExpiryDate = dto.ExpiryDate,
            NumberOfUsed = 0,
            MaxUsage = dto.MaxUsage,
            IsAvailable = dto.IsAvailable ?? true,
            CreatedBy = dto.CreatedBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        private static void MapFromUpdateDto(ComboUpdateDto dto, Combo combo)
        {
            combo.Name = dto.Name;
            combo.Description = dto.Description;
            combo.Price = dto.Price;
            combo.DiscountPercent = dto.DiscountPercent;
            combo.Image = dto.Image;
            combo.StartDate = dto.StartDate;
            combo.ExpiryDate = dto.ExpiryDate;
            combo.MaxUsage = dto.MaxUsage;
            combo.IsAvailable = dto.IsAvailable;
            combo.UpdatedAt = DateTime.UtcNow;
        }
    }
}
