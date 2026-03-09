using System;
using System.Collections.Generic;
using Ecom.Notification.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecom.Notification.Infrastructure.DbContexts;

public partial class EcomNotificationDbContext : DbContext
{
    public EcomNotificationDbContext()
    {
    }

    public EcomNotificationDbContext(DbContextOptions<EcomNotificationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<NotificationTemplate> NotificationTemplates { get; set; }

    public virtual DbSet<NotificationType> NotificationTypes { get; set; }

    public virtual DbSet<UserNotification> UserNotifications { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=NotificationDB;Persist Security Info=False;User ID=demo;Password=Thinh@zzxx9;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotificationTemplate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC0795F78F66");

            entity.Property(e => e.LanguageCode).HasDefaultValue("vi-VN");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Type).WithMany(p => p.NotificationTemplates)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Templates_Types");
        });

        modelBuilder.Entity<NotificationType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC07A717102E");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<UserNotification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserNoti__3214EC0773D026B2");

            entity.HasIndex(e => new { e.ReceiverId, e.ReceiverRole, e.IsRead }, "IX_Notifications_Unread").HasFilter("([IsRead]=(0) AND [IsDeleted]=(0))");

            entity.HasIndex(e => new { e.NotificationChannel, e.IsPushed, e.PushCount }, "IX_UserNotifications_DispatchFilter").HasFilter("([IsPushed]=(0))");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.IsPushed).HasDefaultValue(false);
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.NotificationChannel).HasDefaultValue("WEB_PUSH");
            entity.Property(e => e.Priority).HasDefaultValue((byte)0);
            entity.Property(e => e.PushCount).HasDefaultValue(0);

            entity.HasOne(d => d.Template).WithMany(p => p.UserNotifications)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_UserNoti_Templates");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
