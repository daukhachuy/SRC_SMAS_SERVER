using SMAS_BusinessObject.DTOs.BuffetDTO;
using SMAS_BusinessObject.DTOs.Combo;
using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SMAS_BusinessObject.DTOs.BuffetDTO.PriceLessThanMainPriceAttribute;

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
            return buffets.Select(MapToResponseDto).ToList();
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

        public async Task<(BuffetListResponseDTO? Data, string? MsgCode, string? Message)> CreateAsync(
        BuffetCreateDto dto, int? createdBy)
        {
            // Validate Foods
            var validation = await ValidateFoodsAsync(dto.Foods, existingFoodIds: null);
            if (validation.MsgCode != null)
                return (null, validation.MsgCode, validation.Message);

            var buffet = new Buffet
            {
                Name = dto.Name,
                Description = dto.Description,
                MainPrice = dto.MainPrice,
                ChildrenPrice = dto.ChildrenPrice,
                SidePrice = dto.SidePrice,
                Image = dto.Image,
                IsAvailable = true,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                BuffetFoods = dto.Foods.Select(f => new BuffetFood
                {
                    FoodId = f.FoodId,
                    Quantity = f.Quantity,
                    IsUnlimited = f.IsUnlimited ?? false
                }).ToList()
            };

            var created = await _buffetDAO.CreateAsync(buffet);
            return (MapToResponseDto(created), null, null);
        }

        public async Task<(BuffetListResponseDTO? Data, string? MsgCode, string? Message)> UpdateAsync(
        int id, BuffetUpdateDto dto)
        {
            var buffet = await _buffetDAO.GetByIdTrackedAsync(id);
            if (buffet == null)
                return (null, "MSG_404", $"Không tìm thấy buffet với Id = {id}.");

            var oldFoodIds = buffet.BuffetFoods.Select(bf => bf.FoodId).ToHashSet();
            var validation = await ValidateFoodsAsync(dto.Foods, oldFoodIds);
            if (validation.MsgCode != null)
                return (null, validation.MsgCode, validation.Message);

            MapFromUpdateDto(dto, buffet);

            var newFoods = dto.Foods.Select(f => new BuffetFood
            {
                FoodId = f.FoodId,
                Quantity = f.Quantity,
                IsUnlimited = f.IsUnlimited ?? false
            }).ToList();

            var updated = await _buffetDAO.UpdateAsync(buffet, newFoods);
            return (MapToResponseDto(updated), null, null);
        }


        public Task<bool> DeleteAsync(int id)
            => _buffetDAO.DeleteAsync(id);

        public Task<bool> UpdateStatusAsync(int id, bool isAvailable)
            => _buffetDAO.UpdateStatusAsync(id, isAvailable);
        public async Task<(bool Success, string? MsgCode, string? Message)> AddFoodToBuffetAsync(
       int buffetId, BuffetFoodInputDto dto)
        {
            if (!await _buffetDAO.BuffetExistsAsync(buffetId))
                return (false, "MSG_404", $"Không tìm thấy buffet với Id = {buffetId}.");

            // Validate quantity vs unlimited
            if (!(dto.IsUnlimited ?? false) && (!dto.Quantity.HasValue || dto.Quantity.Value < 1))
                return (false, "MSG_030", "Số lượng phải >= 1 nếu không phải unlimited.");

            // Check món đã tồn tại
            var existing = await _buffetDAO.GetBuffetFoodAsync(buffetId, dto.FoodId);
            if (existing != null)
                return (false, "MSG_031", $"Món FoodId = {dto.FoodId} đã có trong buffet.");

            // Validate Food
            var availabilityMap = await _buffetDAO.GetFoodAvailabilityAsync(new[] { dto.FoodId });
            if (!availabilityMap.ContainsKey(dto.FoodId))
                return (false, "MSG_028", $"FoodId = {dto.FoodId} không tồn tại.");
            if (availabilityMap[dto.FoodId] != true)
                return (false, "MSG_029", $"Món FoodId = {dto.FoodId} đang ngừng kinh doanh.");

            await _buffetDAO.AddFoodToBuffetAsync(buffetId, dto.FoodId, dto.Quantity, dto.IsUnlimited);
            return (true, null, null);
        }

        // ⭐ REMOVE 1 món
        public async Task<(bool Success, string? MsgCode, string? Message)> RemoveFoodFromBuffetAsync(
            int buffetId, int foodId)
        {
            if (!await _buffetDAO.BuffetExistsAsync(buffetId))
                return (false, "MSG_404", $"Không tìm thấy buffet với Id = {buffetId}.");

            var count = await _buffetDAO.CountFoodsInBuffetAsync(buffetId);
            if (count <= 1)
                return (false, "MSG_032", "Buffet phải có ít nhất 1 món.");

            var success = await _buffetDAO.RemoveFoodFromBuffetAsync(buffetId, foodId);
            if (!success)
                return (false, "MSG_033", $"Món FoodId = {foodId} không có trong buffet.");

            return (true, null, null);
        }
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
            UpdatedAt = b.UpdatedAt,
            Foods = b.BuffetFoods?.Select(bf => new BuffetFoodItemDto
            {
                FoodId = bf.FoodId,
                FoodName = bf.Food?.Name,
                FoodImage = bf.Food?.Image,
                Quantity = bf.Quantity ?? 0,
                FoodPrice = bf.Food?.Price ?? 0
            }).ToList() ?? new List<BuffetFoodItemDto>()
        };

        //private static Buffet MapFromCreateDto(BuffetCreateDto dto) => new Buffet
        //{
        //    Name = dto.Name,
        //    Description = dto.Description,
        //    MainPrice = dto.MainPrice,
        //    ChildrenPrice = dto.ChildrenPrice,
        //    SidePrice = dto.SidePrice,
        //    Image = dto.Image,
        //    IsAvailable = dto.IsAvailable ?? true,
        //    CreatedBy = dto.CreatedBy,
        //    CreatedAt = DateTime.UtcNow,
        //    UpdatedAt = DateTime.UtcNow
        //};

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

        private async Task<(string? MsgCode, string? Message)> ValidateFoodsAsync(
        List<BuffetFoodInputDto> foods, HashSet<int>? existingFoodIds)
        {
            // Trùng lặp
            var duplicates = foods.GroupBy(f => f.FoodId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key).ToList();
            if (duplicates.Any())
                return ("MSG_027", $"FoodId trùng lặp: {string.Join(", ", duplicates)}");

            // Quantity vs unlimited
            foreach (var f in foods)
            {
                if (!(f.IsUnlimited ?? false) && (!f.Quantity.HasValue || f.Quantity.Value < 1))
                    return ("MSG_030", $"FoodId = {f.FoodId}: Số lượng phải >= 1 nếu không phải unlimited.");
            }

            // Chỉ check availability cho món MỚI thêm (nếu update)
            var idsToCheck = existingFoodIds == null
                ? foods.Select(f => f.FoodId).ToList()
                : foods.Select(f => f.FoodId).Where(fid => !existingFoodIds.Contains(fid)).ToList();

            if (!idsToCheck.Any()) return (null, null);

            var availabilityMap = await _buffetDAO.GetFoodAvailabilityAsync(idsToCheck);

            var missing = idsToCheck.Where(fid => !availabilityMap.ContainsKey(fid)).ToList();
            if (missing.Any())
                return ("MSG_028", $"Các FoodId không tồn tại: {string.Join(", ", missing)}");

            var unavailable = idsToCheck.Where(fid => availabilityMap[fid] != true).ToList();
            if (unavailable.Any())
                return ("MSG_029", $"Các món đang ngừng kinh doanh: {string.Join(", ", unavailable)}");

            return (null, null);
        }
    }
}
