using System;
using System.Collections.Generic;
using System.Text;

namespace Ecom.Notification.Application.Interface.Auth
{
    public interface IBaseService
    {
        void EnsurePermission(string permission);
    }
}
