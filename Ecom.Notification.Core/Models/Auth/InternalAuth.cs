using System;
using System.Collections.Generic;
using System.Text;

namespace Ecom.Notification.Core.Models.Auth
{
    public class InternalAuth
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}
