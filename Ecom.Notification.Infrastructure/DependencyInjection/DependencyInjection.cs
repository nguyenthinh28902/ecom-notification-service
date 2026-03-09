using Ecom.Notification.Core.Abstractions.Persistence;
using Ecom.Notification.Core.Models.Connection;
using Ecom.Notification.Infrastructure.DbContexts;
using Ecom.Notification.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecom.Notification.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjectionInfrastructure(this IServiceCollection services,
         IConfiguration configuration)
        {
            ConnectionStrings.MainDb = configuration.GetConnectionString("EcommerceNotification") ?? string.Empty;
            // Đăng ký DbContext sử dụng SQL Server
            services.AddDbContext<EcomNotificationDbContext>(options =>
                options.UseSqlServer(ConnectionStrings.MainDb));

            //add kiến trúc repo and UoW
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
