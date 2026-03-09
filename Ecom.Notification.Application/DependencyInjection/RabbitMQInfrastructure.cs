using Ecom.Notification.Application.Service.Consumer;
using Ecom.Notification.Core.Models.Connection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecom.Notification.Application.DependencyInjection
{
    public static class RabbitMQInfrastructure
    {
        public static IServiceCollection AddRabbitMQInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitSettings = configuration
                .GetSection(nameof(RabbitMQSettings))
                .Get<RabbitMQSettings>()
                ?? throw new InvalidOperationException("RabbitMQSettings missing");

            services.AddMassTransit(x =>
            {
                // Đăng ký class xử lý logic khi có tin nhắn đến
                x.AddConsumers(typeof(NotificationConsumer).Assembly);
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitSettings.Host, (ushort)rabbitSettings.Port, "/", h =>
                    {
                        h.Username(rabbitSettings.UserName);
                        h.Password(rabbitSettings.Password);
                    });

                    // Cấu hình Endpoint để lắng nghe Queue cụ thể
                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
