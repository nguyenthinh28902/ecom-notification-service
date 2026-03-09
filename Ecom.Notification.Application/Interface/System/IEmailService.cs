using System;
using System.Collections.Generic;
using System.Text;

namespace Ecom.Notification.Application.Interface.System
{
    public interface IEmailService
    {
        Task<bool> SendHtmlEmailAsync(string toEmail, string subject, string htmlContent);
    }
}
