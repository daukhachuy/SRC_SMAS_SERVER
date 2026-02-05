namespace SMAS_BusinessObject.Configurations
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; } = "smtp.gmail.com";
        public int SmtpPort { get; set; } = 587;
        public string? SmtpUser { get; set; }
        public string? SmtpPassword { get; set; }
        public string FromEmail { get; set; } = null!;
        public string FromName { get; set; } = "SMAS Restaurant";
        public bool EnableSsl { get; set; } = true;
    }
}
