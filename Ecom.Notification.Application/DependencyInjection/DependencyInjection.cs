using Ecom.Notification.Application.Interface.Auth;
using Ecom.Notification.Application.Interface.System;
using Ecom.Notification.Application.Service.Auth;
using Ecom.Notification.Application.Service.System;
using Ecom.Notification.Core.Models.Connection;
using Ecom.Notification.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecom.Notification.Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjectionApplication(this IServiceCollection services,
         IConfiguration configuration)
        {
            services.AddDependencyInjectionInfrastructure(configuration);
            services.AddStackExchangeRedis(configuration);
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //cấu hình auth
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ICurrentCustomerService, CurrentCustomerService>();
            services.AddScoped<IBaseService, BaseService>();
            // cấu hình emial
            services.Configure<EmailSettings>(configuration.GetSection(nameof(EmailSettings)));
            services.AddScoped<IEmailService, EmailService>();
            //serivce
            services.AddScoped<INotificationStoreService, NotificationStoreService>();
            services.AddScoped<INotificationDispatcherService, NotificationDispatcherService>();
            //Cấu hình RabbitMQ
            services.AddRabbitMQInfrastructure(configuration);

            services.AddCmsApplication();
            services.AddWebApplication();
            return services;
        }
    }
}
