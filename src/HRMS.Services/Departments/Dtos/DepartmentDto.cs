using HRMS.Services.Employees.Dtos;

namespace HRMS.Services.Departments.Dtos
{
    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public decimal? Budget { get; set; }
        public int? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public int EmployeeCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateDepartmentDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public decimal? Budget { get; set; }
        public int? ManagerId { get; set; }
    }

    public class UpdateDepartmentDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public decimal? Budget { get; set; }
        public int? ManagerId { get; set; }
    }

    public class DepartmentListDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? ManagerName { get; set; }
        public int EmployeeCount { get; set; }
        public decimal? Budget { get; set; }
    }

    public class DepartmentDetailDto : DepartmentDto
    {
        public List<EmployeeListDto> Employees { get; set; } = new();
    }
}