using Microsoft.Extensions.Options;
using SMAS_BusinessObject.Configurations;
using System.Net;
using System.Net.Mail;

namespace SMAS_Services.EmailServices
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendOtpEmailAsync(string toEmail, string otp, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_settings.SmtpUser) || string.IsNullOrWhiteSpace(_settings.SmtpPassword))
            {
                // Nếu chưa cấu hình SMTP (dev), có thể log và bỏ qua hoặc throw
                throw new InvalidOperationException("Chưa cấu hình SMTP (SmtpUser/SmtpPassword) trong appsettings.");
            }

            using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
            {
                EnableSsl = _settings.EnableSsl,
                Credentials = new NetworkCredential(_settings.SmtpUser, _settings.SmtpPassword)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, _settings.FromName),
                Subject = "Mã xác minh đặt lại mật khẩu - SMAS Restaurant",
                Body = $@"
Xin chào,

Bạn đã yêu cầu đặt lại mật khẩu. Mã xác minh OTP của bạn là:

    {otp}

Mã có hiệu lực trong 5 phút. Không chia sẻ mã này với bất kỳ ai.

Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.

Trân trọng,
SMAS Restaurant
".Trim(),
                IsBodyHtml = false
            };
            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage, cancellationToken);
        }
    }
}
