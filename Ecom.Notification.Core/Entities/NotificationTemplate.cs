using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ecom.Notification.Core.Entities;

public partial class NotificationTemplate
{
    [Key]
    public int Id { get; set; }

    public int TypeId { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? LanguageCode { get; set; }

    [StringLength(255)]
    public string TitleTemplate { get; set; } = null!;

    public string BodyTemplate { get; set; } = null!;

    [StringLength(500)]
    public string? ActionUrl { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("TypeId")]
    [InverseProperty("NotificationTemplates")]
    public virtual NotificationType Type { get; set; } = null!;

    [InverseProperty("Template")]
    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
}
