using HRMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Infrastructure.Data.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Departments");

            builder.HasKey(d => d.Id);

            // Code
            builder.Property(d => d.Code)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(d => d.Code)
                .IsUnique()
                .HasDatabaseName("IX_Departments_Code");

            // Name
            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(d => d.Name)
                .IsUnique()
                .HasDatabaseName("IX_Departments_Name");

            // Description
            builder.Property(d => d.Description)
                .HasMaxLength(500);

            // Location
            builder.Property(d => d.Location)
                .HasMaxLength(200);

            // Contact
            builder.Property(d => d.Phone)
                .HasMaxLength(20);

            builder.Property(d => d.Email)
                .HasMaxLength(100);

            // Budget
            builder.Property(d => d.Budget)
                .HasColumnType("decimal(18,2)");

            // Foreign Key
            builder.HasIndex(d => d.ManagerId)
                .HasDatabaseName("IX_Departments_ManagerId");

            // Relationships
            builder.HasOne(d => d.Manager)
                .WithMany()
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}