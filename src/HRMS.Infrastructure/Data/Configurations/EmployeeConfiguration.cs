using HRMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Infrastructure.Data.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.EmployeeCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.MiddleName)
                .HasMaxLength(50);

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Phone)
                .HasMaxLength(20);

            builder.Property(e => e.Mobile)
                .HasMaxLength(20);

            builder.Property(e => e.Address)
                .HasMaxLength(500);

            builder.Property(e => e.City)
                .HasMaxLength(100);

            builder.Property(e => e.State)
                .HasMaxLength(100);

            builder.Property(e => e.Country)
                .HasMaxLength(100);

            builder.Property(e => e.PostalCode)
                .HasMaxLength(20);

            builder.Property(e => e.NationalId)
                .HasMaxLength(20);

            builder.Property(e => e.PassportNumber)
                .HasMaxLength(20);

            builder.Property(e => e.JobTitle)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.JobGrade)
                .HasMaxLength(20);

            builder.Property(e => e.Salary)
                .HasColumnType("decimal(18,2)");

            builder.Property(e => e.BankName)
                .HasMaxLength(100);

            builder.Property(e => e.BankAccount)
                .HasMaxLength(50);

            builder.Property(e => e.BankBranch)
                .HasMaxLength(50);

            builder.Property(e => e.DateOfBirth)
                .HasColumnType("date");

            builder.Property(e => e.HireDate)
                .HasColumnType("date");

            builder.Property(e => e.ProbationEndDate)
                .HasColumnType("date");

            builder.Property(e => e.ConfirmationDate)
                .HasColumnType("date");

            builder.Property(e => e.ResignationDate)
                .HasColumnType("date");

            builder.Property(e => e.TerminationDate)
                .HasColumnType("date");
        }
    }
}