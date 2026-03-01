using HRMS.Core.Entities.Base;
using HRMS.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Core.Entities
{
    public class Employee : BaseEntity
    {
        [Required]
        [StringLength(20)]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? MiddleName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        // Personal Information
        public DateTime DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        [StringLength(20)]
        public string? NationalId { get; set; }

        [StringLength(20)]
        public string? PassportNumber { get; set; }

        public MaritalStatus MaritalStatus { get; set; }

        // Contact Information
        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(20)]
        public string? Mobile { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? State { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        // Professional Information
        public DateTime HireDate { get; set; }

        public DateTime? ProbationEndDate { get; set; }

        public DateTime? ConfirmationDate { get; set; }

        public DateTime? ResignationDate { get; set; }

        public DateTime? TerminationDate { get; set; }

        public EmploymentType EmploymentType { get; set; }

        public EmployeeStatus Status { get; set; }

        [Required]
        [StringLength(100)]
        public string JobTitle { get; set; } = string.Empty;

        [StringLength(20)]
        public string? JobGrade { get; set; }

        public decimal Salary { get; set; }

        [StringLength(100)]
        public string? BankName { get; set; }

        [StringLength(50)]
        public string? BankAccount { get; set; }

        [StringLength(50)]
        public string? BankBranch { get; set; }

        // Foreign Keys
        public int DepartmentId { get; set; }

        public int? ManagerId { get; set; }

        // Navigation properties
        public virtual Department Department { get; set; } = null!;
        public virtual Employee? Manager { get; set; }
        public virtual ICollection<Employee> Subordinates { get; set; } = new HashSet<Employee>();
        public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new HashSet<LeaveRequest>();
        public virtual ICollection<Attendance> Attendances { get; set; } = new HashSet<Attendance>();
    }
}