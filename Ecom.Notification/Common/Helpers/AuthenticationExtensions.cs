using Ecom.Notification.Common.Requirement;
using Ecom.Notification.Core.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace Ecom.Notification.Common.Helpers
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddAuthenticationExtensions(this IServiceCollection services, IConfiguration configuration)
        {
            var _internalAuth = configuration
                 .GetSection("InternalAuth")
                 .Get<InternalAuth>()
                 ?? throw new InvalidOperationException("JwtSettings missing");
            var _internalAuthWeb = configuration
                 .GetSection("InternalAuthWeb")
                 .Get<InternalAuth>()
                 ?? throw new InvalidOperationException("JwtSettings missing");
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultChallengeScheme = "WebScheme";
            }).AddJwtBearer("Bearer", options =>
              {
                  options.Authority = _internalAuth.Issuer;
                  options.RequireHttpsMetadata = false;

                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidIssuer = _internalAuth.Issuer,

                      // 1. Chỉ comment dòng quan trọng: Tắt kiểm tra Audience để chấp nhận token từ nhiều nguồn
                      ValidateAudience = false,

                      ValidateLifetime = false,
                      ClockSkew = TimeSpan.FromSeconds(20),
                      ValidateIssuerSigningKey = true,
                  };
              }).AddJwtBearer("WebScheme", options =>
              {
                  options.Authority = _internalAuthWeb.Issuer;
                  options.RequireHttpsMetadata = false;
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidIssuer = _internalAuthWeb.Issuer,

                      // 2. Chỉ comment dòng quan trọng: Tắt kiểm tra Audience cho WebScheme
                      ValidateAudience = false,

                      ValidateLifetime = true,
                      ClockSkew = TimeSpan.FromSeconds(20),
                      ValidateIssuerSigningKey = true,
                  };
              });

            services.AddSingleton<IAuthorizationHandler, InternalOrPermissionHandler>();
            
            return services;
        }
    }
}
