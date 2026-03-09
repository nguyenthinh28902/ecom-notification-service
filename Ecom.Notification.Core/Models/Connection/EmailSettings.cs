using System;
using System.Collections.Generic;
using System.Text;

namespace Ecom.Notification.Core.Models.Connection
{
    public class EmailSettings
    {
        public string Host { get; set; } = null!;
        public int Port { get; set; }
        public string DisplayName { get; set; } = null!;
        public string Mail { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
