using MailKit.Net.Smtp;
using MimeKit;

namespace CampusServicesPortal.Api.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(
        string toEmail,
        string subject,
        string body)
    {
        var email = new MimeMessage();

        email.From.Add(
            new MailboxAddress(
                "Campus Services Portal",
                _configuration["EmailSettings:Email"]
            )
        );

        email.To.Add(
            MailboxAddress.Parse(toEmail)
        );

        email.Subject = subject;

        email.Body = new TextPart("plain")
        {
            Text = body
        };

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            _configuration["EmailSettings:SmtpServer"],
            int.Parse(
                _configuration["EmailSettings:Port"]!
            ),
            MailKit.Security.SecureSocketOptions.StartTls
        );

        await smtp.AuthenticateAsync(
            _configuration["EmailSettings:Email"],
            _configuration["EmailSettings:AppPassword"]
        );

        await smtp.SendAsync(email);

        await smtp.DisconnectAsync(true);
    }
}