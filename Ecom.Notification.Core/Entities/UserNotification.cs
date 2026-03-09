using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ecom.Notification.Core.Entities;

[Index("ReceiverId", "ReceiverRole", "IsDeleted", "CreatedAt", Name = "IX_Notifications_Receiver_Latest", IsDescending = new[] { false, false, false, true })]
[Index("ReceiverEmail", Name = "IX_UserNotifications_Email")]
public partial class UserNotification
{
    [Key]
    public long Id { get; set; }

    public int ReceiverId { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string ReceiverRole { get; set; } = null!;

    public int? TemplateId { get; set; }

    [StringLength(255)]
    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    [StringLength(500)]
    public string? DeepLink { get; set; }

    public bool? IsRead { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ReadAt { get; set; }

    public bool? IsDeleted { get; set; }

    public int? SenderId { get; set; }

    public byte? Priority { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [StringLength(255)]
    public string? ReceiverEmail { get; set; }

    public bool? IsPushed { get; set; }

    public int? PushCount { get; set; }

    public string? LastError { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? NotificationChannel { get; set; }

    [ForeignKey("TemplateId")]
    [InverseProperty("UserNotifications")]
    public virtual NotificationTemplate? Template { get; set; }
}
