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

            // Properties
            builder.Property(l => l.Reason)
                .HasMaxLength(500);

            builder.Property(l => l.Remarks)
                .HasMaxLength(500);

            builder.Property(l => l.TotalDays)
                .HasColumnType("decimal(5,2)");

            builder.Property(l => l.StartDate)
                .HasColumnType("date")
                .IsRequired();

            builder.Property(l => l.EndDate)
                .HasColumnType("date")
                .IsRequired();

            builder.Property(l => l.ApprovedDate)
                .HasColumnType("datetime2");

            // Indexes
            builder.HasIndex(l => l.EmployeeId)
                .HasDatabaseName("IX_LeaveRequests_EmployeeId");

            builder.HasIndex(l => l.Status)
                .HasDatabaseName("IX_LeaveRequests_Status");

            builder.HasIndex(l => l.LeaveType)
                .HasDatabaseName("IX_LeaveRequests_LeaveType");

            builder.HasIndex(l => new { l.StartDate, l.EndDate })
                .HasDatabaseName("IX_LeaveRequests_DateRange");

            builder.HasIndex(l => new { l.EmployeeId, l.Status, l.StartDate })
                .HasDatabaseName("IX_LeaveRequests_Employee_Status_Date");

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