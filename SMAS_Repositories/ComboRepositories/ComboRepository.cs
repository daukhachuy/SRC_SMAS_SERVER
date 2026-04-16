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

        public async Task<ComboListResponse> CreateAsync(ComboCreateDto dto, int? createdBy)
        {
            var combo = MapFromCreateDto(dto, createdBy);
            var created = await _comboDAO.CreateAsync(combo);
            return MapToResponseDto(created);
        }

        public async Task<(ComboListResponse? Data, string? MsgCode, string? Message)> UpdateAsync(
       int id, ComboUpdateDto dto)
        {
            // 1. Lấy combo
            var combo = await _comboDAO.GetByIdTrackedAsync(id);
            if (combo == null)
                return (null, "MSG_404", $"Không tìm thấy combo với Id = {id}.");

            // 2. Check FoodId trùng lặp
            var duplicateIds = dto.Foods
                .GroupBy(f => f.FoodId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();
            if (duplicateIds.Any())
            {
                return (null, "MSG_027",
                    $"FoodId bị trùng lặp trong danh sách: {string.Join(", ", duplicateIds)}");
            }

            // 3. Validate Food (chỉ check món MỚI thêm)
            var oldFoodIds = combo.ComboFoods.Select(cf => cf.FoodId).ToHashSet();
            var newlyAddedIds = dto.Foods
                .Select(f => f.FoodId)
                .Where(fid => !oldFoodIds.Contains(fid))
                .ToList();

            if (newlyAddedIds.Any())
            {
                var availabilityMap = await _comboDAO.GetFoodAvailabilityAsync(newlyAddedIds);

                // 3a. Không tồn tại
                var missingIds = newlyAddedIds
                    .Where(fid => !availabilityMap.ContainsKey(fid))
                    .ToList();
                if (missingIds.Any())
                {
                    return (null, "MSG_028",
                        $"Các FoodId không tồn tại: {string.Join(", ", missingIds)}");
                }

                // 3b. Tồn tại nhưng không khả dụng
                var unavailableIds = newlyAddedIds
                    .Where(fid => availabilityMap[fid] != true)
                    .ToList();
                if (unavailableIds.Any())
                {
                    return (null, "MSG_029",
                        $"Các món đang ngừng kinh doanh: {string.Join(", ", unavailableIds)}");
                }
            }

            // 4. Map field cơ bản
            MapFromUpdateDto(dto, combo);

            // 5. Build list mới
            var newFoods = dto.Foods.Select(f => new ComboFood
            {
                FoodId = f.FoodId,
                Quantity = f.Quantity
            }).ToList();

            // 6. Diff update
            var updated = await _comboDAO.UpdateAsync(combo, newFoods);
            return (MapToResponseDto(updated), null, null);
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

        private static Combo MapFromCreateDto(ComboCreateDto dto, int? createdBy) => new Combo
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
            IsAvailable = true,        // server tự set
            CreatedBy = createdBy,     // lấy từ JWT
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ComboFoods = dto.Foods.Select(f => new ComboFood
            {
                FoodId = f.FoodId,
                Quantity = f.Quantity
            }).ToList()
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

            if (dto.IsAvailable.HasValue)
                combo.IsAvailable = dto.IsAvailable.Value;

            combo.UpdatedAt = DateTime.UtcNow;
        }

        // ADD
        public async Task<(bool Success, string? MsgCode, string? Message)> AddFoodToComboAsync(
            int comboId, int foodId, int quantity)
        {
            // 1. Validate combo tồn tại
            if (!await _comboDAO.ComboExistsAsync(comboId))
                return (false, "MSG_404", $"Không tìm thấy combo với Id = {comboId}.");

            // 2. Validate quantity
            if (quantity < 1)
                return (false, "MSG_030", "Số lượng phải >= 1.");

            // 3. Check món đã có trong combo chưa
            var existing = await _comboDAO.GetComboFoodAsync(comboId, foodId);
            if (existing != null)
                return (false, "MSG_031", $"Món FoodId = {foodId} đã có trong combo. Hãy dùng API update quantity.");

            // 4. Validate Food tồn tại + khả dụng
            var availabilityMap = await _comboDAO.GetFoodAvailabilityAsync(new[] { foodId });
            if (!availabilityMap.ContainsKey(foodId))
                return (false, "MSG_028", $"FoodId = {foodId} không tồn tại.");
            if (availabilityMap[foodId] != true)
                return (false, "MSG_029", $"Món FoodId = {foodId} đang ngừng kinh doanh.");

            // 5. Thêm
            await _comboDAO.AddFoodToComboAsync(comboId, foodId, quantity);
            return (true, null, null);
        }

        // REMOVE
        public async Task<(bool Success, string? MsgCode, string? Message)> RemoveFoodFromComboAsync(
            int comboId, int foodId)
        {
            if (!await _comboDAO.ComboExistsAsync(comboId))
                return (false, "MSG_404", $"Không tìm thấy combo với Id = {comboId}.");

            // Chặn xóa món cuối cùng (combo phải có ít nhất 1 món)
            var count = await _comboDAO.CountFoodsInComboAsync(comboId);
            if (count <= 1)
                return (false, "MSG_032", "Combo phải có ít nhất 1 món, không thể xóa món cuối cùng.");

            var success = await _comboDAO.RemoveFoodFromComboAsync(comboId, foodId);
            if (!success)
                return (false, "MSG_033", $"Món FoodId = {foodId} không có trong combo.");

            return (true, null, null);
        }

        // UPDATE QUANTITY
        public async Task<(bool Success, string? MsgCode, string? Message)> UpdateFoodQuantityAsync(
            int comboId, int foodId, int quantity)
        {
            if (!await _comboDAO.ComboExistsAsync(comboId))
                return (false, "MSG_404", $"Không tìm thấy combo với Id = {comboId}.");

            if (quantity < 1)
                return (false, "MSG_030", "Số lượng phải >= 1.");

            var success = await _comboDAO.UpdateFoodQuantityAsync(comboId, foodId, quantity);
            if (!success)
                return (false, "MSG_033", $"Món FoodId = {foodId} không có trong combo.");

            return (true, null, null);
        }
    }
}
