using Ecom.Notification.Core.Models.Auth;
using Ecom.Notification.Core.Models.Connection;


namespace Ecom.Notification.Common.Extensions
{
    public static class ConfigAppSettingExtensions
    {
        public static IServiceCollection AddConfigAppSettingExtensions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<InternalAuth>(configuration.GetSection("InternalAuth"));
            services.Configure<RedisConnection>(configuration.GetSection("RedisConnection"));
            return services;
        }
    }
}
