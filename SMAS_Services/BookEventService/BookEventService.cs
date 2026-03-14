using SMAS_BusinessObject.DTOs.BookEventDTO;
using SMAS_BusinessObject.Models;
using SMAS_Repositories.BookEventRepository;
using SMAS_Services.EventServices;
using SMAS_Services.FoodServices;
using SMAS_Services.ServiceServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.BookEventService
{
    public class BookEventService : IBookEventService
    {
        private readonly IBookEventRepository _bookEventRepository;
        private readonly IEventService _eventService;
        private readonly IServiceService _serviceService;
        private readonly IFoodService _foodService;

        public BookEventService(
            IBookEventRepository bookEventRepository,
            IEventService eventService,
            IServiceService serviceService,
            IFoodService foodService)
        {
            _bookEventRepository = bookEventRepository;
            _eventService = eventService;
            _serviceService = serviceService;
            _foodService = foodService;
        }

        public async Task<List<BookEventListResponseDTO>> GetAllActiveBookEventAsync()
        {
            return await _bookEventRepository.GetAllActiveBookEventAsync();
        }
        public async Task<BookEventListResponseDTO?> GetBookEventByIdAsync(int bookEventId)
        {
            return await _bookEventRepository.GetBookEventByIdAsync(bookEventId);
        }
        public async Task<List<BookEventListResponseDTO>> GetAllBookEventCompleteAndCancelAsync()
        {
            return await _bookEventRepository.GetAllBookEventCompleteAndCancelAsync();
        }

        /// <summary>
        /// Hoàn thành đặt sự kiện (gộp 3 bước). Chỉ lưu DB khi gọi đủ dữ liệu; dùng transaction, lỗi thì rollback.
        /// NumberOfGuests trong DB lưu số bàn (không có trường numberOfTable).
        /// </summary>
        public async Task<CreateBookEventResponseDTO> CreateBookEventWithDetailsAsync(CreateBookEventRequestDTO request)
        {
            var ev = await _eventService.GetEventByIdAsync(request.EventId);
            if (ev == null)
                throw new ArgumentException($"Không tìm thấy sự kiện với Id: {request.EventId}.");

            decimal totalAmount = ev.BasePrice ?? 0;

            var bookEventServices = new List<SMAS_BusinessObject.Models.BookEventService>();
            foreach (var item in request.Services ?? new List<BookEventServiceItemDTO>())
            {
                var svc = await _serviceService.GetServiceByIdAsync(item.ServiceId);
                if (svc == null)
                    throw new ArgumentException($"Không tìm thấy dịch vụ với Id: {item.ServiceId}.");
                int qty = item.Quantity <= 0 ? 1 : item.Quantity;
                decimal unitPrice = svc.ServicePrice;
                totalAmount += unitPrice * qty;
                bookEventServices.Add(new SMAS_BusinessObject.Models.BookEventService
                {
                    ServiceId = item.ServiceId,
                    Quantity = qty,
                    UnitPrice = unitPrice,
                    Note = item.Note
                });
            }

            var eventFoods = new List<EventFood>();
            foreach (var item in request.Foods ?? new List<EventFoodItemDTO>())
            {
                var food = await _foodService.GetFoodByIdAsync(item.FoodId);
                if (food == null)
                    throw new ArgumentException($"Không tìm thấy món với Id: {item.FoodId}.");
                int qty = item.Quantity <= 0 ? 1 : item.Quantity;
                decimal unitPrice = food.PromotionalPrice ?? food.Price;
                totalAmount += unitPrice * qty;
                eventFoods.Add(new EventFood
                {
                    FoodId = item.FoodId,
                    Quantity = qty,
                    UnitPrice = unitPrice,
                    Note = item.Note
                });
            }

            var note = request.Note?.Trim() ?? "";
            if (!string.IsNullOrWhiteSpace(request.Area))
                note = string.IsNullOrEmpty(note) ? request.Area.Trim() : $"{request.Area.Trim()}. {note}";

            var bookingCode = "BE" + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
            var now = DateTime.UtcNow;
            var bookEvent = new BookEvent
            {
                BookingCode = bookingCode,
                EventId = request.EventId,
                CustomerId = request.CustomerId,
                NumberOfGuests = request.NumberOfGuests,
                ReservationDate = request.ReservationDate,
                ReservationTime = request.ReservationTime,
                Note = string.IsNullOrEmpty(note) ? null : note,
                Status = "Pending",
                TotalAmount = totalAmount,
                IsContract = request.NumberOfGuests >= 30,
                CreatedAt = now,
                UpdatedAt = now
            };

            var created = await _bookEventRepository.CreateBookEventWithDetailsAsync(bookEvent, bookEventServices, eventFoods);
            return new CreateBookEventResponseDTO
            {
                BookEventId = created.BookEventId,
                BookingCode = created.BookingCode!,
                Message = "Đặt sự kiện đã được tạo thành công."
            };
        }
    }
}
