using System.Net.Mail;

namespace cheap.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var smtpServer = _configuration["EmailSettings:SmtpServer"];
        var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
        var smtpUser = _configuration["EmailSettings:SmtpUser"];
        var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
        var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"]);

        using (var smtpClient = new SmtpClient(smtpServer, smtpPort))
        {
            smtpClient.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPassword);
            smtpClient.EnableSsl = enableSsl;
            
            var mail = new MailMessage
            {
                From = new MailAddress(smtpUser, "DM ME!"),
                Subject = subject,
                Body = body
            };

            mail.To.Add(new MailAddress(to));

            await smtpClient.SendMailAsync(mail);
        }
    }
}
