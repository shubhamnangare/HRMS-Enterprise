using HRMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Infrastructure.Data.Configurations
{
    public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
    {
        public void Configure(EntityTypeBuilder<Attendance> builder)
        {
            builder.ToTable("Attendances");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.LateMinutes)
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(a => a.OvertimeHours)
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(a => a.TotalHours)
                .HasPrecision(18, 2);

            builder.Property(a => a.Notes)
                .HasMaxLength(500);

            builder.HasIndex(a => new { a.EmployeeId, a.Date })
                .IsUnique();
        }
    }
}