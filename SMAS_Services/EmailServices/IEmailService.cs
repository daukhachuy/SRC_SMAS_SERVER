namespace SMAS_Services.EmailServices
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string toEmail, string otp, CancellationToken cancellationToken = default);
    }
}
