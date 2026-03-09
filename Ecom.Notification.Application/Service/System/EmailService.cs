using Ecom.Notification.Application.Interface.System;
using Ecom.Notification.Core.Models.Connection;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp; // Bắt buộc phải là MailKit, không phải System.Net.Mail
using MimeKit;
namespace Ecom.Notification.Application.Service.System
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        private async Task<SmtpClient> GetConnectedClientAsync()
        {
            var smtp = new SmtpClient();
            // 1. Chỉ comment dòng quan trọng: Gom toàn bộ bước kết nối và xác thực vào đây
            await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_settings.Mail, _settings.Password);
            return smtp;
        }

        public async Task<bool> SendHtmlEmailAsync(string toEmail, string subject, string htmlContent)
        {
            try
            {
                // 2. Chỉ comment dòng quan trọng: Gọi dùng luôn, không cần nhìn thấy dòng Connect/Authenticate nữa
                using var smtp = await GetConnectedClientAsync();

                var email = CreateMimeMessage(toEmail, subject, htmlContent);
                await smtp.SendAsync(email);

                await smtp.DisconnectAsync(true);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi email đến {ToEmail}: {Message}", toEmail, ex.Message);
                return false;
            }
        }

        private MimeMessage CreateMimeMessage(string to, string sub, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.DisplayName, _settings.Mail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = sub;
            message.Body = new BodyBuilder { HtmlBody = body }.ToMessageBody();
            return message;
        }
    }
}
