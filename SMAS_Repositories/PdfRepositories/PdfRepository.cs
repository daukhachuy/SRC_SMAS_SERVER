using SMAS_BusinessObject.DTOs.BookEventDTO;
using SMAS_BusinessObject.DTOs.ContractDTO;
using SMAS_BusinessObject.DTOs.PDFDTO;
using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.PdfRepositories
{
    public class PdfRepository : IPdfRepository
    {
        private readonly PdfDao pdfDao;

        public PdfRepository(PdfDao pdfDao)
        {
            this.pdfDao = pdfDao;
        }

        public async Task<PdfInvoiceDTO> GetInvoiceByIdAsync(string OrderCode)
        {
            var order = await pdfDao.GetInvoiceByIdAsync(OrderCode); 
            if (order == null)
                return new PdfInvoiceDTO();
            return new PdfInvoiceDTO
            {
                OrderCode = order.OrderCode,
                CustomerName = order.User?.Fullname,
                CustomerPhone = order.User?.Phone,
                OrderType = order.OrderType,
                NumberOfGuests = order.NumberOfGuests,
                SubTotal = order.SubTotal,
                DiscountAmount = order.DiscountAmount,
                TaxAmount = order.TaxAmount,
                DeliveryPrice = order.DeliveryPrice,
                TotalAmount = order.TotalAmount,
                PaymentMethod = order.Payments.FirstOrDefault()?.PaymentMethod ?? "N/A",
                Items = order.OrderItems.Select(oi => new OrderItemInvoice
                  {
                    ItemName = oi.Food?.Name
                               ?? oi.Buffet?.Name
                               ?? oi.Combo?.Name
                               ?? "N/A",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    Subtotal = oi.Subtotal
                  }).ToList(),
                CreatedAt = order.CreatedAt,
                ClosedAt = order.ClosedAt
            };
        }

        public async  Task<ContractResponseDTO> GetContractByIdAsync(string contractId)
        {
            var contract = await pdfDao.GetContractByIdAsync(contractId);
            if (contract == null)
                return null;

            return new ContractResponseDTO
            {
                ContractId = contract.ContractId,
                ContractCode = contract.ContractCode,
                CustomerId = contract.CustomerId,
                CustomerName = contract.Customer?.Fullname,
                CustomerEmail = contract.Customer?.Email,
                CustomerPhone = contract.Customer?.Phone,

                ManagerName = contract.BookEvent?.ConfirmedByNavigation?.User.Fullname,
                ManagerEmail = contract.BookEvent?.ConfirmedByNavigation?.User.Email,
                ManagerPhone = contract.BookEvent?.ConfirmedByNavigation?.User.Phone,

                BookEventId = contract.BookEventId,
                BookingCode = contract.BookEvent?.BookingCode,
                EventName = contract.BookEvent?.Event?.Title,

                EventType = contract.EventType,
                EventDate = contract.EventDate,
                NumberOfGuests = contract.NumberOfGuests,
                TotalAmount = contract.TotalAmount,
                DepositAmount = contract.DepositAmount,
                RemainingAmount = contract.RemainingAmount,
                SignMethod = contract.SignMethod,
                SignedAt = contract.SignedAt,
                ContractFileUrl = contract.ContractFileUrl,
                ServiceDetails = contract.ServiceDetails,
                TermsAndConditions = contract.TermsAndConditions,
                Status = contract.Status,
                CreatedAt = contract.CreatedAt,
                UpdatedAt = contract.UpdatedAt,

                Payments = contract.Payments.Select(p => new PaymentSummaryDTO
                {
                    PaymentId = p.PaymentId,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    PaymentStatus = p.PaymentStatus,
                    PaidAt = p.PaidAt
                }).ToList()
            };
        }

        public async Task<BookEventListResponseDTO> GetBookEventdetailAsync(string contractId)
        {
            var contract = await pdfDao.GetContractByIdAsync(contractId);
            var bookEventId = contract?.BookEventId;
            if (bookEventId == null) return null;
            var bookEvent = await pdfDao.GetBookEventAsync(bookEventId.Value);

            return new BookEventListResponseDTO
            {
                BookingCode = bookEvent.BookingCode,
                Status = bookEvent.Status,
                ReservationDate = bookEvent.ReservationDate,
                ReservationTime = bookEvent.ReservationTime,
                IsContract = bookEvent.IsContract,
                TotalAmount = bookEvent.TotalAmount,
                Note = bookEvent.Note,
                CreatedAt = bookEvent.CreatedAt,
                UpdatedAt = bookEvent.UpdatedAt,
                ConfirmedAt = bookEvent.ConfirmedAt,

                Customer = new UserBookEventDto
                {
                    UserId = bookEvent.Customer.UserId,
                    Fullname = bookEvent.Customer.Fullname,
                    Phone = bookEvent.Customer.Phone,
                    Email = bookEvent.Customer.Email
                },

                Event = new EventBookEventDto
                {
                    EventId = bookEvent.Event.EventId,
                    Title = bookEvent.Event.Title,
                    EventType = bookEvent.Event.EventType,
                    Image = bookEvent.Event.Image,
                    BasePrice = bookEvent.Event.BasePrice
                },

                ConfirmedBy = bookEvent.ConfirmedByNavigation == null ? null : new StaffBookEventDto
                {
                    UserId = bookEvent.ConfirmedByNavigation.User.UserId,
                    Fullname = bookEvent.ConfirmedByNavigation.User.Fullname
                },

                Contract = bookEvent.Contract == null ? null : new ContractBookEventDto
                {
                    ContractId = bookEvent.Contract.ContractId,
                    ContractCode = bookEvent.Contract.ContractCode,
                    Status = bookEvent.Contract.Status,
                    TotalAmount = bookEvent.Contract.TotalAmount,
                    DepositAmount = bookEvent.Contract.DepositAmount,
                    RemainingAmount = bookEvent.Contract.RemainingAmount
                },

                Services = bookEvent.BookEventServices.Select(s => new BookEventServiceDto
                {
                    ServiceId = s.ServiceId,
                    ServiceName = s.Service?.Title,
                    Unit = s.Service?.Unit,
                    Quantity = s.Quantity,
                    UnitPrice = s.UnitPrice,
                    Note = s.Note
                }).ToList(),

                Foods = (bookEvent.EventFoods ?? new List<EventFood>()).Select(ef => new BookEventFoodDto
                {
                    FoodId = ef.FoodId,
                    FoodName = ef.Food?.Name,
                    Quantity = ef.Quantity,
                    UnitPrice = ef.UnitPrice,
                    Note = ef.Note
                }).ToList()

            };
        }
    }
}
