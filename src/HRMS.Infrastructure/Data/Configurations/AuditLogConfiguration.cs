using HRMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Infrastructure.Data.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.UserId)
                .HasMaxLength(450);

            builder.Property(a => a.UserName)
                .HasMaxLength(100);

            builder.Property(a => a.Action)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.Entity)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.EntityId)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.OldValues)
                .HasColumnType("text");

            builder.Property(a => a.NewValues)
                .HasColumnType("text");

            builder.Property(a => a.IpAddress)
                .HasMaxLength(50);

            builder.Property(a => a.UserAgent)
                .HasMaxLength(500);

            builder.Property(a => a.Timestamp)
                .IsRequired();

            // Indexes for better query performance
            builder.HasIndex(a => a.UserId)
                .HasDatabaseName("IX_AuditLogs_UserId");

            builder.HasIndex(a => a.UserName)
                .HasDatabaseName("IX_AuditLogs_UserName");

            builder.HasIndex(a => a.Entity)
                .HasDatabaseName("IX_AuditLogs_Entity");

            builder.HasIndex(a => a.EntityId)
                .HasDatabaseName("IX_AuditLogs_EntityId");

            builder.HasIndex(a => a.Action)
                .HasDatabaseName("IX_AuditLogs_Action");

            builder.HasIndex(a => a.Timestamp)
                .HasDatabaseName("IX_AuditLogs_Timestamp");

            // Composite index for common queries
            builder.HasIndex(a => new { a.Entity, a.EntityId, a.Action })
                .HasDatabaseName("IX_AuditLogs_Entity_EntityId_Action");
        }
    }
}