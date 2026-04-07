using SMAS_BusinessObject.DTOs.Event;
using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.EventRepositories
{
    public class EventRepository : IEventRepository
    {
        private readonly EventDAO _eventDAO;

        public EventRepository(EventDAO eventDAO)
        {
            _eventDAO = eventDAO;
        }

        public async Task<IEnumerable<EventListResponse>> GetAllEventsAsync()
        {
            var events = await _eventDAO.GetAllEventsAsync();
            return events.Select(e => new EventListResponse
            {
                EventId = e.EventId,
                Title = e.Title,
                Description = e.Description,
                EventType = e.EventType,
                Image = e.Image,
                MinGuests = e.MinGuests,
                MaxGuests = e.MaxGuests,
                BasePrice = e.BasePrice,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                CreatedBy = e.CreatedBy,
                IsActive = e.IsActive
            });
        }

        public async Task<EventListResponse?> GetEventByIdAsync(int eventId)
        {
            var e = await _eventDAO.GetEventByIdAsync(eventId);
            if (e == null) return null;
            return new EventListResponse
            {
                EventId = e.EventId,
                Title = e.Title,
                Description = e.Description,
                EventType = e.EventType,
                Image = e.Image,
                MinGuests = e.MinGuests,
                MaxGuests = e.MaxGuests,
                BasePrice = e.BasePrice,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                CreatedBy = e.CreatedBy,
                IsActive = e.IsActive
            };
        }
        public async Task<EventListResponse> CreateAsync(EventCreateDto dto)
        {
            var entity = MapToEntity(dto);
            var created = await _eventDAO.CreateAsync(entity);
            return MapToDto(created);
        }

        public async Task<EventListResponse> UpdateAsync(int id, EventUpdateDto dto)
        {
            var entity = await _eventDAO.GetEventByIdAsync(id)
                ?? throw new KeyNotFoundException($"Event with id {id} not found.");

            ApplyUpdate(entity, dto);
            var updated = await _eventDAO.UpdateAsync(entity);
            return MapToDto(updated);
        }

        public async Task<EventListResponse> PatchStatusAsync(int id, EventStatusPatchDto dto)
        {
            var updated = await _eventDAO.PatchStatusAsync(id, dto.IsActive)
                ?? throw new KeyNotFoundException($"Event with id {id} not found.");

            return MapToDto(updated);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _eventDAO.GetEventByIdAsync(id)
                ?? throw new KeyNotFoundException($"Event with id {id} not found.");

            await _eventDAO.UpdateStatusAsync(id);
        }

        // ==================== MAPPERS ====================
        private static EventListResponse MapToDto(Event e) => new()
        {
            EventId = e.EventId,
            Title = e.Title,
            Description = e.Description,
            EventType = e.EventType,
            Image = e.Image,
            MinGuests = e.MinGuests,
            MaxGuests = e.MaxGuests,
            BasePrice = e.BasePrice,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            IsActive = e.IsActive
        };

        private static Event MapToEntity(EventCreateDto dto) => new()
        {
            Title = dto.Title.Trim(),
            Description = dto.Description,
            EventType = dto.EventType,
            Image = dto.Image,
            MinGuests = dto.MinGuests,
            MaxGuests = dto.MaxGuests,
            BasePrice = dto.BasePrice,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = dto.CreatedBy,
            IsActive = dto.IsActive ?? true
        };

        private static void ApplyUpdate(Event entity, EventUpdateDto dto)
        {
            entity.Title = dto.Title.Trim();
            entity.Description = dto.Description;
            entity.EventType = dto.EventType;
            entity.Image = dto.Image;
            entity.MinGuests = dto.MinGuests;
            entity.MaxGuests = dto.MaxGuests;
            entity.BasePrice = dto.BasePrice;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;
            // CreatedBy gi? nguy�n
        }
    }

}
