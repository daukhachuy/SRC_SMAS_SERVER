using SMAS_BusinessObject.DTOs.DiscountDTO;
using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.DiscountRepositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly DiscountDao _context;

        public DiscountRepository(DiscountDao context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DiscountResponse>> GetAllDiscountsAsync()
        {
            var discounts = await _context.GetAllDiscountsAsync();
            return discounts.Select(d => new DiscountResponse
            {
                DiscountId = d.DiscountId,
                Code = d.Code,
                Description = d.Description,
                DiscountType = d.DiscountType,
                Value = d.Value,
                MinOrderAmount = d.MinOrderAmount,
                MaxDiscountAmount = d.MaxDiscountAmount,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                UsageLimit = d.UsageLimit,
                UsedCount = d.UsedCount,
                ApplicableFor = d.ApplicableFor,
                Status = d.Status,
                CreatedAt = d.CreatedAt
            });
        }

        public async Task<DiscountResponse?> GetDiscountByCodeAsync(string Code)
        {
            var discount = await _context.GetDiscountByIdAsync(Code);
            if (discount == null) return null;
            return new DiscountResponse
            {
                DiscountId = discount.DiscountId,
                Code = discount.Code,
                Description = discount.Description,
                DiscountType = discount.DiscountType,
                Value = discount.Value,
                MinOrderAmount = discount.MinOrderAmount,
                MaxDiscountAmount = discount.MaxDiscountAmount,
                StartDate = discount.StartDate,
                EndDate = discount.EndDate,
                UsageLimit = discount.UsageLimit,
                UsedCount = discount.UsedCount,
                ApplicableFor = discount.ApplicableFor,
                Status = discount.Status,
                CreatedAt = discount.CreatedAt
            };
        }
        // ─── GET BY ID ─────────────────────────────────────────────────────────
        public async Task<DiscountResponse?> GetByIdAsync(int id)
        {
            var entity = await _context.GetByIdAsync(id);
            return entity is null ? null : MapToDto(entity);
        }

        // ─── CREATE ────────────────────────────────────────────────────────────
        public async Task<DiscountResponse> CreateAsync(DiscountCreateDto dto)
        {
            var entity = MapToEntity(dto);
            var created = await _context.CreateAsync(entity);
            return MapToDto(created);
        }

        // ─── UPDATE ────────────────────────────────────────────────────────────
        public async Task<DiscountResponse> UpdateAsync(int id, DiscountUpdateDto dto)
        {
            var entity = await _context.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Discount with id {id} not found.");

            ApplyUpdate(entity, dto);

            var updated = await _context.UpdateAsync(entity);
            return MapToDto(updated);
        }

        public Task<bool> DeleteAsync(int id) => _context.DeleteAsync(id);

        public Task<bool> UpdateStatusAsync(int id, string status) => _context.UpdateStatusAsync(id, status);

        // ==================== MAPPERS ====================

        /// <summary>Entity → ResponseDto</summary>
        private static DiscountResponse MapToDto(Discount e) => new()
        {
            DiscountId = e.DiscountId,
            Code = e.Code,
            Description = e.Description,
            DiscountType = e.DiscountType,
            Value = e.Value,
            MinOrderAmount = e.MinOrderAmount,
            MaxDiscountAmount = e.MaxDiscountAmount,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            UsageLimit = e.UsageLimit,
            UsedCount = e.UsedCount,
            ApplicableFor = e.ApplicableFor,
            Status = e.Status,
            CreatedBy = e.CreatedBy,
            CreatedAt = e.CreatedAt
        };

        /// <summary>CreateDto → Entity (dùng khi tạo mới)</summary>
        private static Discount MapToEntity(DiscountCreateDto dto) => new()
        {
            Code = dto.Code.Trim().ToUpper(),
            Description = dto.Description,
            DiscountType = dto.DiscountType,
            Value = dto.Value,
            MinOrderAmount = dto.MinOrderAmount,
            MaxDiscountAmount = dto.MaxDiscountAmount,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            UsageLimit = dto.UsageLimit,
            UsedCount = 0,
            ApplicableFor = dto.ApplicableFor,
            Status = dto.Status ?? "Active",
            CreatedBy = dto.CreatedBy,
            CreatedAt = DateTime.UtcNow
        };

        /// <summary>UpdateDto → áp lên Entity có sẵn (giữ nguyên các field không đổi)</summary>
        private static void ApplyUpdate(Discount entity, DiscountUpdateDto dto)
        {
            entity.Description = dto.Description;
            entity.DiscountType = dto.DiscountType;
            entity.Value = dto.Value;
            entity.MinOrderAmount = dto.MinOrderAmount;
            entity.MaxDiscountAmount = dto.MaxDiscountAmount;
            entity.StartDate = dto.StartDate;
            entity.EndDate = dto.EndDate;
            entity.UsageLimit = dto.UsageLimit;
            entity.ApplicableFor = dto.ApplicableFor;
            entity.Status = dto.Status;
        }
    }
}
