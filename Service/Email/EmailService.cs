using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Net.Mail;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Sauvio.Business.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendConfirmationEmail(string toEmail, string token)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_config["EmailSettings:FromEmail"]));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "Confirm your registration";
            message.Body = new TextPart("plain")
            {
                Text = $"Click this link to confirm your account: http://localhost:5163/api/account/confirm?token={token}"
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_config["EmailSettings:SmtpServer"], 587, false);
            await smtp.AuthenticateAsync(_config["EmailSettings:FromEmail"], _config["EmailSettings:Password"]);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
    }
}
