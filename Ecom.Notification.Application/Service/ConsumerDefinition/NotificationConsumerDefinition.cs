using Ecom.Notification.Application.Service.Consumer;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecom.Notification.Application.Service.ConsumerDefinition
{
    public class NotificationConsumerDefinition : ConsumerDefinition<NotificationConsumer>
    {
        public NotificationConsumerDefinition()
        {
            // Định nghĩa tên Queue cố định cho Consumer này
            EndpointName = "order-notification-queue";
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<NotificationConsumer> consumerConfigurator, IRegistrationContext context)
        {
            // Cấu hình Retry riêng cho ông này
            endpointConfigurator.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
        }
    }
}
