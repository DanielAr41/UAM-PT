// Ubicación: ProjectRoot/Services/EmailService.cs
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using UAM_PT.Models;

namespace UAM_PT.Services
{
    public class EmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public void SendPasswordRecoveryEmail(string toEmail, string recoveryCode)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("NovaMer", _emailSettings.Email));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "Recuperación de Contraseña";

            var body = new TextPart("plain")
            {
                Text = $"Tu código de recuperación de contraseña es: {recoveryCode}"
            };

            message.Body = body;

            using (var client = new SmtpClient())
            {
                client.Connect(_emailSettings.SmtpServer, _emailSettings.Port, false);
                client.Authenticate(_emailSettings.Email, _emailSettings.Password);
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}