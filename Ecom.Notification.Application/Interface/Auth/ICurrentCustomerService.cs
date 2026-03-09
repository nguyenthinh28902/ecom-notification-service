using System;
using System.Collections.Generic;
using System.Text;

namespace Ecom.Notification.Application.Interface.Auth
{
    public interface ICurrentCustomerService
    {
        int Id { get; }
        string? Email { get; }
        string? PhoneNumber { get; }
        string? EmailOrPhone { get; }
    }
}
