using Ecom.Contracts.Requests;

namespace Ecom.Notification.Application.Interface.System
{
    public interface INotificationDispatcherService
    {
        Task DispatchNotificationAsync(NotificationRequestDto request);
    }
}
