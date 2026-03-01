using HRMS.Core.Enums;

namespace HRMS.Services.Employees.Dtos
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string GenderName => Gender.ToString();
        public MaritalStatus MaritalStatus { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public DateTime HireDate { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string? JobGrade { get; set; }
        public decimal Salary { get; set; }
        public EmployeeStatus Status { get; set; }
        public string StatusName => Status.ToString();
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateEmployeeDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? NationalId { get; set; }
        public string? PassportNumber { get; set; }
        public DateTime HireDate { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string? JobGrade { get; set; }
        public decimal Salary { get; set; }
        public EmploymentType EmploymentType { get; set; }
        public int DepartmentId { get; set; }
        public int? ManagerId { get; set; }
        public string? BankName { get; set; }
        public string? BankAccount { get; set; }
        public string? BankBranch { get; set; }
    }

    public class UpdateEmployeeDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string? JobGrade { get; set; }
        public decimal Salary { get; set; }
        public int DepartmentId { get; set; }
        public int? ManagerId { get; set; }
        public EmployeeStatus Status { get; set; }
        public string? BankName { get; set; }
        public string? BankAccount { get; set; }
        public string? BankBranch { get; set; }
    }

    public class EmployeeListDto
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { set; get; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public EmployeeStatus Status { get; set; }
        public string StatusBadgeClass => Status switch
        {
            EmployeeStatus.Active => "success",
            EmployeeStatus.Inactive => "secondary",
            EmployeeStatus.OnLeave => "warning",
            EmployeeStatus.Terminated => "danger",
            EmployeeStatus.Resigned => "info",
            _ => "light"
        };
    }

    public class EmployeeSearchDto
    {
        public string? SearchTerm { get; set; }
        public int? DepartmentId { get; set; }
        public EmployeeStatus? Status { get; set; }
        public int? ManagerId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "LastName";
        public bool SortAscending { get; set; } = true;
    }
}