using Ecom.Notification.Application.Interface.Auth;
using Ecom.Notification.Core.Exceptions;
using Ecom.Notification.Core.Models.Auth;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecom.Notification.Application.Service.Auth
{
    public class BaseService: IBaseService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<BaseService> _logger;
        public BaseService(ICurrentUserService currentUserService, ILogger<BaseService> logger) {
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public void EnsurePermission(string permission)
        {
            var userId = _currentUserService.UserId;
            var scopes = _currentUserService.Scopes;
            var roles = _currentUserService.Roles;
            _logger.LogInformation("Checking permission '{Permission}' for user {UserId} with scopes: {Scopes}", permission, userId, string.Join(", ", scopes));
            var hasPermission = scopes.Contains(permission);
            var hasPermissionAdmin = false;// roles.Contains(DepartmentCode.Admin.ToString());


            if (!hasPermission && !hasPermissionAdmin)
            {
                _logger.LogWarning("User {UserId} does not have permission '{Permission}'", userId, permission);
                throw new UnauthorizedException($"User does not have permission: {permission}");
            }
        }
    }
}
