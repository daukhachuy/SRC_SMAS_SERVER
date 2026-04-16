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
        // Link email trỏ về frontend để user ký trên UI, không gọi trực tiếp API.
        var baseUrl = string.IsNullOrWhiteSpace(publicBaseUrl) ? _appSettings.FrontendBaseUrl.TrimEnd('/') : publicBaseUrl.TrimEnd('/');
        var signLink = $"{baseUrl}/contract/sign?token={token}";

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

    public async Task<(ContractDetailByTokenDTO? dto, int statusCode, string? error)> GetContractByTokenAsync(string? token, int currentUserId)
    {
        var t = token?.Trim();
        if (string.IsNullOrEmpty(t))
            return (null, 404, "Link không hợp lệ");

        var contract = await _repo.GetContractBySignTokenWithBookEventAsync(t);
        var (status, error) = ValidateContractReadyToSign(contract);
        if (status != 0)
            return (null, status, error);

        // Customer chỉ được xem hợp đồng của chính mình (theo CustomerId trong BookEvent).
        if (contract!.BookEvent == null || contract.BookEvent.CustomerId != currentUserId)
            return (null, 403, "Bạn không có quyền xem hợp đồng này.");

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

    public async Task<(ContractSignResponseDTO? dto, int statusCode, string? error)> SignContractByTokenAsync(ContractSignRequestDTO request, int currentUserId)
    {
        var token = request.Token?.Trim();
        if (string.IsNullOrEmpty(token))
            return (null, 404, "Link không hợp lệ");

        var contract = await _repo.GetContractBySignTokenWithBookEventAsync(token);
        var (status, error) = ValidateContractReadyToSign(contract);
        if (status != 0)
            return (null, status, error);

        // Customer chỉ được ký hợp đồng của chính mình (theo CustomerId trong BookEvent).
        if (contract!.BookEvent == null || contract.BookEvent.CustomerId != currentUserId)
            return (null, 403, "Bạn không có quyền ký hợp đồng này.");

        await _repo.SignContractAndUpdateBookEventAsync(contract!);

        // Sau khi ký thành công, gửi mail nhắc customer thanh toán cọc (best-effort).
        try { await SendDepositRequiredEmailAsync(contract.ContractId); } catch { /* ignore */ }

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

    public async Task<(object? dto, int statusCode, string? error)> DepositAsync(
        int contractId,
        string apiBaseUrl,
        int currentUserId,
        bool isPrivilegedRole)
    {
        var contract = await _repo.GetContractByIdForDepositAsync(contractId);
        if (contract == null)
            return (null, 404, "Không tìm thấy hợp đồng");

        // Customer chỉ được tạo link cọc cho chính hợp đồng của mình.
        if (!isPrivilegedRole && contract.CustomerId != currentUserId)
            return (null, 403, "Bạn không có quyền thanh toán cọc cho hợp đồng này.");

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
        var returnUrl = $"{baseApi}/api/contract/{contractId}/deposit/callback?depositResult=success";
        var cancelUrl = $"{baseApi}/api/contract/{contractId}/deposit/callback?depositResult=cancel";

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
        int contractId, string status, long orderCode, string payosCode, string frontendBaseUrl)
    {
        var fe = string.IsNullOrWhiteSpace(frontendBaseUrl) ? _appSettings.FrontendBaseUrl.TrimEnd('/') : frontendBaseUrl.TrimEnd('/');
        static string BuildMyOrdersUrl(string feBase, int bookEventId, string paymentStatus) =>
            $"{feBase}/my-orders?tab=event&bookEventId={bookEventId}&payment={paymentStatus}";

        try
        {
            var contract = await _repo.GetContractByIdForDepositAsync(contractId);
            if (contract == null)
                return BuildMyOrdersUrl(fe, contractId, "error");

            var bookEventId = contract.BookEventId ?? contractId;

            if (status == "cancel")
                return BuildMyOrdersUrl(fe, bookEventId, "cancel");

            if (status != "success")
                return BuildMyOrdersUrl(fe, bookEventId, "error");

            if (await _repo.ExistsPaidDepositForContractAsync(contractId))
                return BuildMyOrdersUrl(fe, bookEventId, "success");

            // Callback chỉ phục vụ UX redirect; xác nhận thành công dùng query code=00 từ PayOS.
            // Webhook vẫn là safety-net cho trường hợp user đóng trình duyệt trước khi callback về.
            var payosConfirmed = string.Equals(payosCode, "00", StringComparison.Ordinal);
            if (!payosConfirmed)
                return BuildMyOrdersUrl(fe, bookEventId, "pending_verify");

            var nowCb = DateTime.UtcNow;
            if (contract.Status == "Cancelled")
                return BuildMyOrdersUrl(fe, bookEventId, "contract_invalid");
            if (contract.Status != "Signed")
                return BuildMyOrdersUrl(fe, bookEventId, "contract_invalid");
            if (!contract.SignedAt.HasValue || nowCb >= contract.SignedAt.Value.AddHours(DepositDeadlineHours()))
                return BuildMyOrdersUrl(fe, bookEventId, "expired");

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
                // Race condition: webhook/callback khác đã xử lý trước đó -> coi như thành công.
                return BuildMyOrdersUrl(fe, bookEventId, "success");
            }

            // Gửi mail thông báo kích hoạt (best-effort, tránh làm fail luồng redirect).
            try { await SendContractActivatedEmailAsync(contractId); } catch { /* ignore */ }

            return BuildMyOrdersUrl(fe, bookEventId, "success");
        }
        catch
        {
            return BuildMyOrdersUrl(fe, contractId, "error");
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

            // Gửi mail thông báo kích hoạt (best-effort). Idempotent vì lần gọi trùng sẽ fail do contract không còn Signed.
            try { await SendContractActivatedEmailAsync(contractId); } catch { /* ignore */ }
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

    private async Task SendContractActivatedEmailAsync(int contractId)
    {
        var contract = await _repo.GetContractByIdWithCustomerAndBookEventAsync(contractId);
        if (contract == null)
            return;

        var email = contract.Customer?.Email;
        if (string.IsNullOrWhiteSpace(email))
            return;

        var bookingCode = contract.BookEvent?.BookingCode;
        if (string.IsNullOrWhiteSpace(bookingCode))
            return;

        var fe = _appSettings.FrontendBaseUrl.TrimEnd('/');
        var viewUrl = $"{fe}/contracts/{bookingCode}";
        var customerName = contract.Customer?.Fullname ?? "Quý khách";
        var contractCode = contract.ContractCode ?? $"#{contract.ContractId}";

        var html = $@"
<p>Xin chào {customerName},</p>
<p>Hợp đồng <strong>{contractCode}</strong> đã được kích hoạt thành công sau khi nhận cọc.</p>
<p>Bạn có thể xem chi tiết hợp đồng tại đây: <a href=""{viewUrl}"">{viewUrl}</a></p>
<p>Nếu bạn chưa đăng nhập, hệ thống sẽ yêu cầu đăng nhập trước khi xem.</p>
<p>Trân trọng,<br/>SMAS Restaurant</p>";

        await _emailService.SendAsync(
            email,
            $"Hợp đồng {contractCode} đã được kích hoạt",
            html);
    }

    private async Task SendDepositRequiredEmailAsync(int contractId)
    {
        var contract = await _repo.GetContractByIdWithCustomerAndBookEventAsync(contractId);
        if (contract == null)
            return;

        var email = contract.Customer?.Email;
        if (string.IsNullOrWhiteSpace(email))
            return;

        var bookingCode = contract.BookEvent?.BookingCode;
        if (string.IsNullOrWhiteSpace(bookingCode))
            return;

        var fe = _appSettings.FrontendBaseUrl.TrimEnd('/');
        var encodedBookingCode = Uri.EscapeDataString(bookingCode);
        var viewUrl = $"{fe}/orders?tab=event&bookingCode={encodedBookingCode}";
        var customerName = contract.Customer?.Fullname ?? "Quý khách";
        var contractCode = contract.ContractCode ?? $"#{contract.ContractId}";
        var depositAmount = contract.DepositAmount ?? 0;

        var html = $@"
<p>Xin chào {customerName},</p>
<p>Hợp đồng <strong>{contractCode}</strong> đã được ký thành công.</p>
<p>Vui lòng thanh toán đặt cọc: <strong>{depositAmount:N0} VND</strong>.</p>
<p>Bạn có thể vào trang Đơn hàng của tôi (tab Sự kiện) để tiếp tục thanh toán tại đây: <a href=""{viewUrl}"">{viewUrl}</a></p>
<p>Nếu bạn chưa đăng nhập, hệ thống sẽ yêu cầu đăng nhập trước khi xem và thanh toán.</p>
<p>Trân trọng,<br/>SMAS Restaurant</p>";

        await _emailService.SendAsync(
            email,
            $"Hợp đồng {contractCode} - Yêu cầu thanh toán đặt cọc",
            html);
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
