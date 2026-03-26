namespace SMAS_Services.EmailServices
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string toEmail, string otp, CancellationToken cancellationToken = default);

        /// <summary>Gửi email HTML (hợp đồng, xác nhận sự kiện, ...).</summary>
        Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default);
    }
}
