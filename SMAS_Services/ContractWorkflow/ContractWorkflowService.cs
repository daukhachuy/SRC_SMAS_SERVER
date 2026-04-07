using System.Text.Json;
using Microsoft.Extensions.Options;
using SMAS_BusinessObject.Configurations;
using SMAS_BusinessObject.DTOs.PayOSDTO;
using SMAS_BusinessObject.DTOs.Workflow;
using SMAS_BusinessObject.Models;
using SMAS_Repositories.ContractWorkflow;
using BookEventServiceEntity = SMAS_BusinessObject.Models.BookEventService;
using SMAS_Services.EmailServices;
using SMAS_Services.PaymentServices;

namespace SMAS_Services.ContractWorkflow;

public class ContractWorkflowService : IContractWorkflowService
{
    private readonly IContractWorkflowRepository _repo;
    private readonly IEmailService _emailService;
    private readonly IPaymentService _paymentService;
    private readonly AppSettings _appSettings;

    public ContractWorkflowService(
        IContractWorkflowRepository repo,
        IEmailService emailService,
        IPaymentService paymentService,
        IOptions<AppSettings> appSettings)
    {
        _repo = repo;
        _emailService = emailService;
        _paymentService = paymentService;
        _appSettings = appSettings.Value;
    }

    public async Task<(BookEventReviewResponseDTO? dto, int statusCode, string? error)> ReviewBookEventAsync(
        int bookEventId, BookEventReviewRequestDTO request, int staffUserId)
    {
        var be = await _repo.GetBookEventForReviewAsync(bookEventId);
        if (be == null)
            return (null, 404, "Không tìm thấy sự kiện");

        if (be.Status != "Pending")
            return (null, 400, "Sự kiện không ở trạng thái chờ duyệt");

        var d = request.Decision?.Trim();
        if (d != "Approved" && d != "Rejected")
            return (null, 400, "Quyết định không hợp lệ, chỉ chấp nhận Approved hoặc Rejected");

        if (d == "Approved")
        {
            be.Status = "Approved";
            be.ConfirmedBy = staffUserId;
            be.ConfirmedAt = DateTime.UtcNow;
            be.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            be.Status = "Rejected";
            be.Note = request.Note;
            be.UpdatedAt = DateTime.UtcNow;
        }

        await _repo.UpdateBookEventAsync(be);

        return (new BookEventReviewResponseDTO
        {
            BookEventId = be.BookEventId,
            Status = be.Status!,
            ConfirmedBy = be.ConfirmedBy,
            ConfirmedAt = be.ConfirmedAt,
            Note = be.Note
        }, 200, null);
    }

    public async Task<(CreateContractFromBookEventResponseDTO? dto, int statusCode, string? error)> CreateContractFromBookEventAsync(
        int bookEventId, CreateContractFromBookEventRequestDTO request)
    {
        var be = await _repo.GetBookEventForCreateContractAsync(bookEventId);
        if (be == null)
            return (null, 404, "Không tìm thấy sự kiện");

        if (be.Status != "Approved")
            return (null, 400, "Sự kiện chưa được duyệt");

        if (be.ContractId != null)
            return (null, 400, "Hợp đồng đã được tạo cho sự kiện này");

        int dp = request.DepositPercent;
        if (dp <= 0 || dp >= 100)
            return (null, 400, "Phần trăm đặt cọc không hợp lệ");

        decimal totalAmount = be.TotalAmount ?? 0;
        decimal depositAmount = totalAmount * dp / 100m;
        decimal remainingAmount = totalAmount - depositAmount;

        var contractCode = "HD-" + DateTime.UtcNow.ToString("yyyyMMdd") + "-" + bookEventId;

        var contract = new Contract
        {
            ContractCode = contractCode,
            CustomerId = be.CustomerId,
            BookEventId = be.BookEventId,
            EventType = be.Event?.EventType,
            EventDate = be.ReservationDate,
            NumberOfGuests = be.NumberOfGuests,
            TotalAmount = totalAmount,
            DepositAmount = depositAmount,
            RemainingAmount = remainingAmount,
            TermsAndConditions = request.TermsAndConditions,
            Status = "Draft",
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repo.CreateContractAndLinkBookEventAsync(contract, be);

        return (new CreateContractFromBookEventResponseDTO
        {
            ContractId = created.ContractId,
            ContractCode = created.ContractCode!,
            BookEventId = be.BookEventId,
            TotalAmount = created.TotalAmount,
            DepositAmount = created.DepositAmount ?? 0,
            RemainingAmount = created.RemainingAmount ?? 0,
            Status = created.Status!,
            CreatedAt = created.CreatedAt!.Value
        }, 201, null);
    }

    public async Task<(BookEventDetailResponseDTO? dto, int statusCode, string? error)> GetBookEventDetailAsync(int bookEventId)
    {
        var be = await _repo.GetBookEventForDetailAsync(bookEventId);
        if (be == null)
            return (null, 404, "Không tìm thấy sự kiện");

        var contract = be.Contract;
        decimal totalAmount = contract?.TotalAmount ?? be.TotalAmount ?? 0;
        decimal depositAmount = contract?.DepositAmount ?? 0;

        var payments = ((IEnumerable<Payment>?)contract?.Payments ?? Enumerable.Empty<Payment>())
            .Where(p => p.PaymentStatus == "Paid")
            .OrderBy(p => p.PaidAt)
            .ToList();
        decimal paidAmount = payments.Sum(p => p.Amount);
        decimal remainingAmount = totalAmount - paidAmount;

        var dto = new BookEventDetailResponseDTO
        {
            BookEventId = be.BookEventId,
            BookingCode = be.BookingCode,
            Status = be.Status,
            CreatedAt = be.CreatedAt,
            Customer = new BookEventDetailCustomerDTO
            {
                CustomerId = be.CustomerId,
                FullName = be.Customer.Fullname,
                Phone = be.Customer.Phone,
                Email = be.Customer.Email
            },
            EventInfo = new BookEventDetailEventInfoDTO
            {
                ReservationDate = be.ReservationDate.ToString("yyyy-MM-dd"),
                ReservationTime = be.ReservationTime.ToString("HH:mm"),
                NumberOfGuests = be.NumberOfGuests,
                Note = be.Note
            },
            ConfirmedBy = new BookEventDetailConfirmedByDTO
            {
                StaffId = be.ConfirmedBy,
                FullName = be.ConfirmedByNavigation?.User?.Fullname,
                ConfirmedAt = be.ConfirmedAt
            },
            Foods = ((IEnumerable<EventFood>?)be.EventFoods ?? Enumerable.Empty<EventFood>()).Select(ef =>
            {
                var unit = ef.UnitPrice ?? ef.Food?.PromotionalPrice ?? ef.Food?.Price ?? 0;
                var qty = ef.Quantity;
                return new BookEventDetailFoodDTO
                {
                    FoodId = ef.FoodId,
                    Name = ef.Food?.Name ?? "",
                    Image = ef.Food?.Image,
                    Quantity = qty,
                    UnitPrice = unit,
                    Subtotal = unit * qty,
                    Note = ef.Note
                };
            }).ToList(),
            Services = ((IEnumerable<BookEventServiceEntity>?)be.BookEventServices ?? Enumerable.Empty<BookEventServiceEntity>()).Select(bs =>
            {
                var qty = bs.Quantity ?? 1;
                var unit = bs.UnitPrice ?? bs.Service?.ServicePrice;
                decimal? sub = unit.HasValue ? unit * qty : null;
                return new BookEventDetailServiceDTO
                {
                    ServiceId = bs.ServiceId,
                    Name = bs.Service?.Title ?? "",
                    Quantity = bs.Quantity,
                    UnitPrice = unit,
                    Subtotal = sub,
                    Note = bs.Note
                };
            }).ToList(),
            Contract = contract == null
                ? new BookEventDetailContractDTO()
                : new BookEventDetailContractDTO
                {
                    ContractId = contract.ContractId,
                    ContractCode = contract.ContractCode,
                    Status = contract.Status,
                    SignedAt = contract.SignedAt,
                    DepositDueUtc = contract.Status == "Signed" && contract.SignedAt.HasValue
                        ? contract.SignedAt.Value.AddHours(DepositDeadlineHours())
                        : null,
                    ContractFileUrl = contract.ContractFileUrl,
                    TermsAndConditions = contract.TermsAndConditions
                },
            Payment = new BookEventDetailPaymentSectionDTO
            {
                TotalAmount = totalAmount,
                DepositAmount = depositAmount,
                PaidAmount = paidAmount,
                RemainingAmount = remainingAmount,
                Payments = ((IEnumerable<Payment>?)contract?.Payments ?? Enumerable.Empty<Payment>())
                    .OrderBy(p => p.PaidAt)
                    .Select(p => new BookEventDetailPaymentItemDTO
                    {
                        PaymentId = p.PaymentId,
                        PaymentCode = p.PaymentCode,
                        Amount = p.Amount,
                        PaymentMethod = p.PaymentMethod,
                        PaymentStatus = p.PaymentStatus,
                        PaidAt = p.PaidAt,
                        Note = p.Note
                    }).ToList()
            }
        };

        return (dto, 200, null);
    }

    public async Task<(SendContractSignResponseDTO? dto, int statusCode, string? error)> SendContractSignEmailAsync(
        int contractId, string publicBaseUrl)
    {
        var contract = await _repo.GetContractByIdWithCustomerAsync(contractId);
        if (contract == null)
            return (null, 404, "Không tìm thấy hợp đồng");

        if (contract.Status == "Signed")
            return (null, 400, "Hợp đồng đã được ký, không thể gửi lại");

        if (contract.Status == "Cancelled")
            return (null, 400, "Hợp đồng đã bị hủy");

        if (contract.Status != "Draft" && contract.Status != "Sent")
            return (null, 400, "Trạng thái không hợp lệ");

        var email = contract.Customer?.Email;
        if (string.IsNullOrWhiteSpace(email))
            return (null, 400, "Khách hàng chưa có email.");

        var token = Guid.NewGuid().ToString("N");
        var deadline = DateTime.UtcNow.AddHours(72);
        var baseUrl = string.IsNullOrWhiteSpace(publicBaseUrl) ? _appSettings.PublicBaseUrl.TrimEnd('/') : publicBaseUrl.TrimEnd('/');
        var signLink = $"{baseUrl}/api/contract/sign?token={token}";

        var customerName = contract.Customer?.Fullname ?? "Quý khách";
        var html = $@"
<p>Xin chào {customerName},</p>
<p>Vui lòng ký hợp đồng <strong>{contract.ContractCode}</strong>.</p>
<p><a href=""{signLink}"">Ký hợp đồng</a></p>
<p>Link có hiệu lực đến: {deadline:yyyy-MM-dd HH:mm} UTC.</p>
<p>Trân trọng,<br/>SMAS Restaurant</p>";

        try
        {
            await _emailService.SendAsync(
                email,
                $"Hợp đồng {contract.ContractCode} - Vui lòng ký xác nhận",
                html);
        }
        catch
        {
            return (null, 500, "Không thể gửi email, vui lòng thử lại");
        }

        await _repo.UpdateContractAfterSendSignAsync(contract, token, DateTime.UtcNow);

        return (new SendContractSignResponseDTO
        {
            ContractId = contract.ContractId,
            ContractCode = contract.ContractCode!,
            SentTo = MaskEmail(email),
            Deadline = deadline,
            Message = "Đã gửi email hợp đồng thành công"
        }, 200, null);
    }

    public async Task<(ContractDetailByTokenDTO? dto, int statusCode, string? error)> GetContractByTokenAsync(string? token)
    {
        var t = token?.Trim();
        if (string.IsNullOrEmpty(t))
            return (null, 404, "Link không hợp lệ");

        var contract = await _repo.GetContractBySignTokenWithBookEventAsync(t);
        var (status, error) = ValidateContractReadyToSign(contract);
        if (status != 0)
            return (null, status, error);

        return (new ContractDetailByTokenDTO
        {
            ContractCode = contract!.ContractCode,
            EventDate = contract.EventDate.ToString("yyyy-MM-dd"),
            NumberOfGuests = contract.NumberOfGuests,
            TotalAmount = contract.TotalAmount,
            DepositAmount = contract.DepositAmount,
            TermsAndConditions = contract.TermsAndConditions,
            ContractFileUrl = contract.ContractFileUrl,
            Token = t
        }, 200, null);
    }

    public async Task<(ContractSignResponseDTO? dto, int statusCode, string? error)> SignContractByTokenAsync(ContractSignRequestDTO request)
    {
        var token = request.Token?.Trim();
        if (string.IsNullOrEmpty(token))
            return (null, 404, "Link không hợp lệ");

        var contract = await _repo.GetContractBySignTokenWithBookEventAsync(token);
        var (status, error) = ValidateContractReadyToSign(contract);
        if (status != 0)
            return (null, status, error);

        await _repo.SignContractAndUpdateBookEventAsync(contract!);

        var signedAt = contract.SignedAt!.Value;
        return (new ContractSignResponseDTO
        {
            ContractId = contract.ContractId,
            ContractCode = contract.ContractCode!,
            SignedAt = signedAt,
            DepositDueUtc = signedAt.AddHours(DepositDeadlineHours()),
            Message = "Ký hợp đồng thành công"
        }, 200, null);
    }

    public async Task<(object? dto, int statusCode, string? error)> DepositAsync(int contractId, string apiBaseUrl)
    {
        var contract = await _repo.GetContractByIdForDepositAsync(contractId);
        if (contract == null)
            return (null, 404, "Không tìm thấy hợp đồng");

        if (contract.Status == "Cancelled")
            return (null, 400, "Hợp đồng đã bị hủy, không thể thanh toán cọc.");

        if (contract.Status != "Signed")
            return (null, 400, "Hợp đồng chưa được ký, không thể nhận cọc");

        var now = DateTime.UtcNow;
        if (!contract.SignedAt.HasValue || now >= contract.SignedAt.Value.AddHours(DepositDeadlineHours()))
            return (null, 400, "Đã hết hạn thanh toán đặt cọc. Vui lòng liên hệ nhà hàng.");

        if (await _repo.ExistsPaidDepositForContractAsync(contractId))
            return (null, 400, "Hợp đồng này đã được thanh toán tiền cọc");

        decimal deposit = contract.DepositAmount ?? 0;
        if (deposit <= 0)
            return (null, 400, "Số tiền đặt cọc không hợp lệ.");

        // orderCode PayOS: dùng int như luồng Order — PayOS so khớp chữ ký với JSON (orderCode long trước đây dễ lệch code 201).
        int orderCode = GeneratePayOsOrderCode(contractId);
        int amountVnd = (int)Math.Round(deposit, MidpointRounding.AwayFromZero);
        var desc = "Dat coc " + (contract.ContractCode ?? "");
        if (desc.Length > 25)
            desc = desc[..25];

        var baseApi = string.IsNullOrWhiteSpace(apiBaseUrl) ? _appSettings.PublicBaseUrl.TrimEnd('/') : apiBaseUrl.TrimEnd('/');
        var returnUrl = $"{baseApi}/api/contract/{contractId}/deposit/callback?status=success";
        var cancelUrl = $"{baseApi}/api/contract/{contractId}/deposit/callback?status=cancel";

        var payResult = await _paymentService.CreateContractDepositPaymentLinkAsync(
            orderCode, amountVnd, desc, returnUrl, cancelUrl);

        if (!payResult.Success || string.IsNullOrEmpty(payResult.CheckoutUrl))
            return (null, 500, payResult.Message ?? "Không thể kết nối PayOS, vui lòng thử lại");

        return (new ContractDepositPayOSResponseDTO
        {
            ContractId = contractId,
            Amount = deposit,
            CheckoutUrl = payResult.CheckoutUrl,
            Message = "Vui lòng thanh toán qua PayOS"
        }, 200, null);
    }

    public async Task<string> DepositCallbackRedirectAsync(
        int contractId, string status, long orderCode, string frontendBaseUrl)
    {
        var fe = string.IsNullOrWhiteSpace(frontendBaseUrl) ? _appSettings.FrontendBaseUrl.TrimEnd('/') : frontendBaseUrl.TrimEnd('/');

        try
        {
            var contract = await _repo.GetContractByIdForDepositAsync(contractId);
            if (contract == null)
                return $"{fe}/events/{contractId}?payment=error";

            var bookEventId = contract.BookEventId ?? contractId;

            if (status == "cancel")
                return $"{fe}/events/{bookEventId}?payment=cancel";

            if (status != "success")
                return $"{fe}/events/{bookEventId}?payment=error";

            var ok = await _paymentService.VerifyPaymentAsync(orderCode);
            if (!ok)
                return $"{fe}/events/{bookEventId}?payment=error";

            if (await _repo.ExistsPaidDepositForContractAsync(contractId))
                return $"{fe}/events/{bookEventId}?payment=success";

            var nowCb = DateTime.UtcNow;
            if (contract.Status == "Cancelled")
                return $"{fe}/events/{bookEventId}?payment=contract_invalid";
            if (contract.Status != "Signed")
                return $"{fe}/events/{bookEventId}?payment=contract_invalid";
            if (!contract.SignedAt.HasValue || nowCb >= contract.SignedAt.Value.AddHours(DepositDeadlineHours()))
                return $"{fe}/events/{bookEventId}?payment=expired";

            decimal deposit = contract.DepositAmount ?? 0;
            decimal totalAmount = contract.TotalAmount;

            var payment = new Payment
            {
                PaymentCode = "DEP-" + DateTime.UtcNow.ToString("yyyyMMddHHmm") + "-" + contractId,
                ContractId = contractId,
                OrderId = null,
                Amount = deposit,
                PaymentMethod = "PayOS",
                PaymentStatus = "Paid",
                TransactionId = $"PAYOS-{contractId}-{DateTime.UtcNow.Ticks}",
                Note = "deposit",
                ReceivedBy = null,
                PaidAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _repo.AddDepositPaymentAndUpdateContractAsync(
                    contract, payment, totalAmount, deposit, DepositDeadlineHours());
            }
            catch (InvalidOperationException)
            {
                return $"{fe}/events/{bookEventId}?payment=expired";
            }

            return $"{fe}/events/{bookEventId}?payment=success";
        }
        catch
        {
            return $"{fe}/events/{contractId}?payment=error";
        }
    }

    public async Task DepositWebhookAsync(int contractId, string rawBody, string? signatureHeader)
    {
        if (!_paymentService.VerifyWebhookSignature(rawBody, signatureHeader))
            return;

        PayOSWebhookPayload? payload;
        try
        {
            payload = JsonSerializer.Deserialize<PayOSWebhookPayload>(rawBody);
        }
        catch
        {
            return;
        }

        if (payload?.Data == null || !payload.Success)
            return;

        var contract = await _repo.GetContractByIdForDepositAsync(contractId);
        if (contract == null)
            return;

        if (contract.Status == "Cancelled")
            return;

        var nowWh = DateTime.UtcNow;
        if (contract.Status != "Signed" || await _repo.ExistsPaidDepositForContractAsync(contractId))
            return;
        if (!contract.SignedAt.HasValue || nowWh >= contract.SignedAt.Value.AddHours(DepositDeadlineHours()))
            return;

        decimal deposit = contract.DepositAmount ?? 0;
        decimal totalAmount = contract.TotalAmount;

        var payment = new Payment
        {
            PaymentCode = "DEP-WH-" + DateTime.UtcNow.ToString("yyyyMMddHHmm") + "-" + contractId,
            ContractId = contractId,
            OrderId = null,
            Amount = deposit,
            PaymentMethod = "PayOS",
            PaymentStatus = "Paid",
            TransactionId = $"PAYOS-{contractId}-{DateTime.UtcNow.Ticks}",
            Note = "deposit",
            ReceivedBy = null,
            PaidAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await _repo.AddDepositPaymentAndUpdateContractAsync(
                contract, payment, totalAmount, deposit, DepositDeadlineHours());
        }
        catch
        {
            // Luôn 200 cho PayOS — bỏ qua lỗi nghiệp vụ
        }
    }

    public Task<int> CancelExpiredSignedDepositContractsAsync() =>
        _repo.CancelSignedContractsPastDepositWindowAsync(DepositDeadlineHours());

    private int DepositDeadlineHours() =>
        Math.Max(1, _appSettings.DepositDeadlineHoursAfterSign);

    /// <summary>Mã đơn PayOS (int, duy nhất mỗi lần gọi — cùng kiểu với orderCode đơn hàng).</summary>
    private static int GeneratePayOsOrderCode(int contractId)
    {
        unchecked
        {
            int r = Random.Shared.Next(1, int.MaxValue);
            int n = (contractId * 100_007) ^ r ^ (int)(DateTime.UtcNow.Ticks % int.MaxValue);
            if (n <= 0)
                n = r;
            return n;
        }
    }

    public async Task<(ConfirmBookEventResponseDTO? dto, int statusCode, string? error)> ConfirmBookEventAsync(
        int bookEventId, int staffUserId)
    {
        var be = await _repo.GetBookEventWithContractAndCustomerAsync(bookEventId);
        if (be == null)
            return (null, 404, "Không tìm thấy sự kiện");

        // Sau ký hợp đồng BookEvent vẫn Approved; bản ghi cũ có thể là Confirmed (legacy) — vẫn cho xác nhận cuối.
        if (be.Status != "Approved" && be.Status != "Confirmed")
            return (null, 400, "Sự kiện không ở trạng thái hợp lệ để xác nhận (cần đã duyệt / đã ký hợp đồng).");

        if (be.ContractId == null || be.Contract == null)
            return (null, 400, "Chưa có hợp đồng cho sự kiện này");

        var c = be.Contract;
        if (c.Status != "Deposited")
            return (null, 400, "Khách hàng chưa thanh toán tiền cọc");

        be.Status = "Active";
        be.ConfirmedBy = staffUserId;
        be.ConfirmedAt = DateTime.UtcNow;
        be.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateBookEventAsync(be);

        bool emailSent = false;
        try
        {
            var email = be.Customer.Email;
            if (!string.IsNullOrWhiteSpace(email))
            {
                var remaining = c.RemainingAmount ?? 0;
                var html = $@"
<p>Xin chào {be.Customer.Fullname},</p>
<p>Sự kiện <strong>{be.BookingCode}</strong> đã được xác nhận.</p>
<p>Ngày: {be.ReservationDate:yyyy-MM-dd}, Giờ: {be.ReservationTime:HH:mm}, Số khách: {be.NumberOfGuests}.</p>
<p>Số tiền còn lại cần thanh toán: {remaining:N0} VND.</p>
<p>Trân trọng,<br/>SMAS Restaurant</p>";
                await _emailService.SendAsync(
                    email,
                    $"Xác nhận sự kiện {be.BookingCode}",
                    html);
                emailSent = true;
            }
        }
        catch
        {
            emailSent = false;
        }

        return (new ConfirmBookEventResponseDTO
        {
            BookEventId = be.BookEventId,
            BookingCode = be.BookingCode!,
            Status = be.Status!,
            ConfirmedBy = staffUserId,
            ConfirmedAt = be.ConfirmedAt!.Value,
            EmailSent = emailSent,
            Message = "Sự kiện đã được xác nhận thành công"
        }, 200, null);
    }

    private static string MaskEmail(string email)
    {
        var at = email.IndexOf('@');
        if (at <= 0)
            return "***";
        var local = email[..at];
        var domain = email[at..];
        var prefix = local.Length <= 3 ? local : local[..3];
        return prefix + "***" + domain;
    }

    /// <summary>Cùng điều kiện với ký: token tồn tại, Sent, chưa hết hạn 72h.</summary>
    private static (int statusCode, string? error) ValidateContractReadyToSign(Contract? contract)
    {
        if (contract == null)
            return (404, "Link không hợp lệ");

        if (contract.Status == "Signed")
            return (400, "Hợp đồng đã được ký trước đó");

        if (contract.Status == "Cancelled")
            return (400, "Hợp đồng đã bị hủy");

        if (contract.Status != "Sent")
            return (400, "Link không hợp lệ");

        if (contract.UpdatedAt == null || contract.UpdatedAt.Value.AddHours(72) < DateTime.UtcNow)
            return (400, "Link đã hết hạn, vui lòng liên hệ nhà hàng để gửi lại");

        return (0, null);
    }
}
