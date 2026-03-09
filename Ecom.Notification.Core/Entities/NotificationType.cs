using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ecom.Notification.Core.Entities;

[Index("TypeCode", Name = "UQ__Notifica__3E1CDC7C213CEC6E", IsUnique = true)]
public partial class NotificationType
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string TypeCode { get; set; } = null!;

    [StringLength(100)]
    public string TypeName { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string TargetRole { get; set; } = null!;

    [StringLength(255)]
    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [InverseProperty("Type")]
    public virtual ICollection<NotificationTemplate> NotificationTemplates { get; set; } = new List<NotificationTemplate>();
}
