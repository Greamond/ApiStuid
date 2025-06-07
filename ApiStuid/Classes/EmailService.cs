using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using MailKit.Security;


namespace ApiStuid.Classes
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly EmailSettings _emailSettings;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>();
        }

        public async Task SendPasswordResetCodeAsync(string email, int resetCode)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SmtpUsername));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = "Код сброса пароля";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                    <h2>Сброс пароля</h2>
                    <p>Ваш код подтверждения: <strong>{resetCode}</strong></p>
                    <p>Если вы не запрашивали сброс пароля, проигнорируйте это письмо.</p>"
                };

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                // Подключение к SMTP серверу
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);

                // Аутентификация
                if (!string.IsNullOrEmpty(_emailSettings.SmtpUsername))
                {
                    await client.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
                }

                // Отправка письма
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Письмо для сброса пароля отправлено на {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при отправке письма для сброса пароля на {email}");
            }
        }

        public class EmailSettings
        {
            public string SmtpServer { get; set; }
            public int SmtpPort { get; set; }
            public bool UseSsl { get; set; }
            public string SmtpUsername { get; set; }
            public string SmtpPassword { get; set; }
            public string SenderName { get; set; }
            public string SenderEmail { get; set; }
        }
    }
}
