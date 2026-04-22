using SMAS_BusinessObject.DTOs.BookEventDTO;
using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.BookEventRepository
{
    public class BookEventRepository : IBookEventRepository
    {
        private readonly BookEventDAO _bookEventDAO;

        public BookEventRepository(BookEventDAO bookEventDAO)
        {
            _bookEventDAO = bookEventDAO;
        }

        public async Task<List<BookEventListResponseDTO>> GetAllActiveBookEventAsync()
        {
            var bookEvents = await _bookEventDAO.GetAllActiveBookEventAsync();
            return MapToDTO(bookEvents);
        }
        public async Task<BookEventListResponseDTO?> GetBookEventByIdAsync(int bookEventId)
        {
            var bookEvent = await _bookEventDAO.GetBookEventByIdAsync(bookEventId);
            if (bookEvent == null) return null;
            return MapToDTO(new List<SMAS_BusinessObject.Models.BookEvent> { bookEvent }).First();
        }

        public async Task<List<BookEventListResponseDTO>> GetAllBookEventCompleteAndCancelAsync()
        {
            var bookEvents = await _bookEventDAO.GetAllBookEventCompleteAndCancelAsync();
            return MapToDTO(bookEvents);
        }

        public async Task<List<BookEventListResponseDTO>> GetBookEventsByCustomerIdAsync(int customerId)
        {
            var bookEvents = await _bookEventDAO.GetBookEventsByCustomerIdAsync(customerId);
            return MapToDTO(bookEvents);
        }

        public async Task<List<BookEventListResponseDTO>> GetBookEventsByStatusAsync(string status)
        {
            var bookEvents = await _bookEventDAO.GetBookEventsByStatusAsync(status);
            return MapToDTO(bookEvents);
        }

        public async Task<BookEvent> CreateBookEventWithDetailsAsync(
            BookEvent bookEvent,
            List<SMAS_BusinessObject.Models.BookEventService> bookEventServices,
            List<EventFood> eventFoods)
        {
            return await _bookEventDAO.CreateBookEventWithDetailsAsync(bookEvent, bookEventServices, eventFoods);
        }

        public async Task<BookEventCheckInResponseDTO> CheckInBookEventAsync(int bookEventId, int managerUserId, List<int> tableIds)
        {
            var (bookEvent, orderCode, selectedTables, checkInAt) =
                await _bookEventDAO.CheckInBookEventAsync(bookEventId, managerUserId, tableIds);

            return new BookEventCheckInResponseDTO
            {
                BookEventId = bookEvent.BookEventId,
                BookingCode = bookEvent.BookingCode,
                OrderCode = orderCode,
                Status = bookEvent.Status,
                CheckInAt = checkInAt,
                TableIds = selectedTables,
                Message = "Check-in sự kiện thành công. Bàn đã được chuyển sang trạng thái EVENT."
            };
        }

        public async Task<BookEventCheckoutResponseDTO> CheckoutBookEventAsync(int bookEventId, int managerUserId)
        {
            var (bookEvent, releasedTableIds, checkOutAt) =
                await _bookEventDAO.CheckoutBookEventAsync(bookEventId, managerUserId);

            return new BookEventCheckoutResponseDTO
            {
                BookEventId = bookEvent.BookEventId,
                BookingCode = bookEvent.BookingCode,
                Status = bookEvent.Status,
                CheckOutAt = checkOutAt,
                ReleasedTableIds = releasedTableIds,
                Message = "Checkout thành công. Bàn đã giải phóng, vui lòng xác nhận thanh toán phần còn lại để hoàn tất sự kiện."
            };
        }

        public async Task<int> NotifyManagersBeforeUpcomingEventsAsync(int hoursBeforeStart)
        {
            return await _bookEventDAO.NotifyManagersBeforeUpcomingEventsAsync(hoursBeforeStart);
        }

        public async Task<IEnumerable<BookEventResponseDTO>> GetBookEvenAsync()
        {
            var bookEvents = await _bookEventDAO.GetBookEvenTocheckinAsync();
            if (bookEvents == null || bookEvents.Count == 0) return null;
            return bookEvents.Select(be => new BookEventResponseDTO
            {
                BookEventId = be.BookEventId,
                BookingCode = be.BookingCode,
                Status = be.Status,
                NumberOfTable = be.NumberOfGuests,
                TitleEvent = be.Event.Title,
                ReservationDate = be.ReservationDate,
                ReservationTime = be.ReservationTime
            }).ToList();
        }

        // ── Private helper mapping ─────────────────────────────────────────────
        private static List<BookEventListResponseDTO> MapToDTO(List<SMAS_BusinessObject.Models.BookEvent> bookEvents)
        {
            return bookEvents.Select(be => new BookEventListResponseDTO
            {
                BookEventId = be.BookEventId,
                BookingCode = be.BookingCode,
                Status = be.Status,
                NumberOfGuests = be.NumberOfGuests,
                ReservationDate = be.ReservationDate,
                ReservationTime = be.ReservationTime,
                IsContract = be.IsContract,
                TotalAmount = be.TotalAmount,
                Note = be.Note,
                CreatedAt = be.CreatedAt,
                UpdatedAt = be.UpdatedAt,
                ConfirmedAt = be.ConfirmedAt,

                Customer = new UserBookEventDto
                {
                    UserId = be.Customer.UserId,
                    Fullname = be.Customer.Fullname,
                    Phone = be.Customer.Phone,
                    Email = be.Customer.Email
                },

                Event = new EventBookEventDto
                {
                    EventId = be.Event.EventId,
                    Title = be.Event.Title,
                    EventType = be.Event.EventType,
                    Image = be.Event.Image,
                    BasePrice = be.Event.BasePrice
                },

                ConfirmedBy = be.ConfirmedByNavigation == null ? null : new StaffBookEventDto
                {
                    UserId = be.ConfirmedByNavigation.User.UserId,
                    Fullname = be.ConfirmedByNavigation.User.Fullname
                },

                Contract = be.Contract == null ? null : new ContractBookEventDto
                {
                    ContractId = be.Contract.ContractId,
                    ContractCode = be.Contract.ContractCode,
                    Status = be.Contract.Status,
                    TotalAmount = be.Contract.TotalAmount,
                    DepositAmount = be.Contract.DepositAmount,
                    RemainingAmount = be.Contract.RemainingAmount
                },

                Services = be.BookEventServices.Select(s => new BookEventServiceDto
                {
                    ServiceId = s.ServiceId,
                    ServiceName = s.Service?.Title,
                    Unit = s.Service?.Unit,
                    Quantity = s.Quantity,
                    UnitPrice = s.UnitPrice,
                    Note = s.Note
                }).ToList(),

                Foods = (be.EventFoods ?? new List<EventFood>()).Select(ef => new BookEventFoodDto
                {
                    FoodId = ef.FoodId,
                    FoodName = ef.Food?.Name,
                    Quantity = ef.Quantity,
                    UnitPrice = ef.UnitPrice,
                    Note = ef.Note
                }).ToList()

            }).ToList();
        }


    }
}
