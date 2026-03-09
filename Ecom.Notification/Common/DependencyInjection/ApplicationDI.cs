using Ecom.Notification.Application.DependencyInjection;

namespace Ecom.Notification.Common.DependencyInjection
{
    public static class ApplicationDI
    {
        public static IServiceCollection AddApplicationDI(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDependencyInjectionApplication(configuration);
            return services;
        }
    }
}
