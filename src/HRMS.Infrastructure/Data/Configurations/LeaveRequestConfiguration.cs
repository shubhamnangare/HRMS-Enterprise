using HRMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Infrastructure.Data.Configurations
{
    public class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
    {
        public void Configure(EntityTypeBuilder<LeaveRequest> builder)
        {
            builder.ToTable("LeaveRequests");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.Reason)
                .HasMaxLength(500);

            builder.Property(l => l.Remarks)
                .HasMaxLength(500);

            builder.Property(l => l.TotalDays)
                .HasColumnType("decimal(5,2)");

            builder.Property(l => l.StartDate)
                .HasColumnType("date");

            builder.Property(l => l.EndDate)
                .HasColumnType("date");

            builder.Property(l => l.ApprovedDate)
                .HasColumnType("datetime2");

            // Relationships
            builder.HasOne(l => l.Employee)
                .WithMany(e => e.LeaveRequests)
                .HasForeignKey(l => l.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.ApprovedBy)
                .WithMany()
                .HasForeignKey(l => l.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}