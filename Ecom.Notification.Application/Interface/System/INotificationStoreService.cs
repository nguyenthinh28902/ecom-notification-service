using Ecom.Contracts.Requests;
using Ecom.Notification.Core.Entities;

namespace Ecom.Notification.Application.Interface.System
{
    public interface INotificationStoreService
    {
        Task<UserNotification?> CreateNotificationAsync(NotificationRequestDto request);
    }
}
