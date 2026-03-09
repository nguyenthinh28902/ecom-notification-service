using System;
using System.Collections.Generic;
using System.Text;

namespace Ecom.Notification.Core.Models.Enum
{
    public enum NotificationChannel
    {
        WEB_PUSH, // Chỉ lưu vào DB để hiện icon chuông trên web
        EMAIL,    // Thực hiện hành động gửi Email
        ALL       // Cả hai
    }
}
