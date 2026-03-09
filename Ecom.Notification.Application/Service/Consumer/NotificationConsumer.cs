using Ecom.Contracts.Requests;
using Ecom.Notification.Application.Interface.System;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecom.Notification.Application.Service.Consumer
{
    public class NotificationConsumer : IConsumer<NotificationRequestDto>
    {
        private readonly INotificationDispatcherService _notificationService;
        private readonly ILogger<NotificationConsumer> _logger;

        public NotificationConsumer(INotificationDispatcherService notificationService
            , ILogger<NotificationConsumer> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<NotificationRequestDto> context)
        {
            var eventData = context.Message;

            _logger.LogInformation("Nhận được yêu cầu gửi mail cho: {Email}", eventData.ReceiverEmail);

            try
            {
                // 3. Chỉ comment dòng quan trọng: Gọi logic xử lý (Render HTML + Push Email)
                // Ný dùng lại hàm Dispatch nãy mình viết trong Controller ấy
                await _notificationService.DispatchNotificationAsync(eventData);

                _logger.LogInformation("Xử lý Consumer thành công cho: {Email}", eventData.ReceiverEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý tin nhắn từ RabbitMQ cho {Email}", eventData.ReceiverEmail);
                // Ném lỗi để MassTransit thực hiện Retry (thử lại) nếu đã cấu hình
                throw;
            }
        }
    }
}
