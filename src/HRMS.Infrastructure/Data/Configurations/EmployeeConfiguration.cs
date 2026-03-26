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

            // Employee Code
            builder.Property(e => e.EmployeeCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(e => e.EmployeeCode)
                .IsUnique()
                .HasDatabaseName("IX_Employees_EmployeeCode");

            // Name fields
            builder.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.MiddleName)
                .HasMaxLength(50);

            builder.HasIndex(e => new { e.FirstName, e.LastName })
                .HasDatabaseName("IX_Employees_FullName");

            // Email
            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("IX_Employees_Email");

            // Contact Information
            builder.Property(e => e.Phone)
                .HasMaxLength(20);

            builder.Property(e => e.Mobile)
                .HasMaxLength(20);

            // Address
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

            // Identification
            builder.Property(e => e.NationalId)
                .HasMaxLength(20);

            builder.HasIndex(e => e.NationalId)
                .IsUnique()
                .HasDatabaseName("IX_Employees_NationalId")
                .HasFilter("[NationalId] IS NOT NULL");

            builder.Property(e => e.PassportNumber)
                .HasMaxLength(20);

            builder.HasIndex(e => e.PassportNumber)
                .IsUnique()
                .HasDatabaseName("IX_Employees_PassportNumber")
                .HasFilter("[PassportNumber] IS NOT NULL");

            // Professional Information
            builder.Property(e => e.JobTitle)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(e => e.JobTitle)
                .HasDatabaseName("IX_Employees_JobTitle");

            builder.Property(e => e.JobGrade)
                .HasMaxLength(20);

            builder.Property(e => e.Salary)
                .HasColumnType("decimal(18,2)");

            // Banking Information
            builder.Property(e => e.BankName)
                .HasMaxLength(100);

            builder.Property(e => e.BankAccount)
                .HasMaxLength(50);

            builder.Property(e => e.BankBranch)
                .HasMaxLength(50);

            // Dates
            builder.Property(e => e.DateOfBirth)
                .HasColumnType("date");

            builder.Property(e => e.HireDate)
                .HasColumnType("date")
                .IsRequired();

            builder.Property(e => e.ProbationEndDate)
                .HasColumnType("date");

            builder.Property(e => e.ConfirmationDate)
                .HasColumnType("date");

            builder.Property(e => e.ResignationDate)
                .HasColumnType("date");

            builder.Property(e => e.TerminationDate)
                .HasColumnType("date");

            // Status and Type indexes
            builder.HasIndex(e => e.Status)
                .HasDatabaseName("IX_Employees_Status");

            builder.HasIndex(e => e.EmploymentType)
                .HasDatabaseName("IX_Employees_EmploymentType");

            builder.HasIndex(e => e.Gender)
                .HasDatabaseName("IX_Employees_Gender");

            // Foreign Keys
            builder.HasIndex(e => e.DepartmentId)
                .HasDatabaseName("IX_Employees_DepartmentId");

            builder.HasIndex(e => e.ManagerId)
                .HasDatabaseName("IX_Employees_ManagerId");

            // Relationships
            builder.HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Manager)
                .WithMany(e => e.Subordinates)
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}