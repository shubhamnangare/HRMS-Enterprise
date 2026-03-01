using HRMS.Core.Entities;
using HRMS.Core.Enums;

namespace HRMS.Core.Specifications
{
    public class EmployeeWithDepartmentSpecification : BaseSpecification<Employee>
    {
        public EmployeeWithDepartmentSpecification(int id)
            : base(e => e.Id == id)
        {
            AddInclude(e => e.Department);
            AddInclude(e => e.Manager);
        }

        public EmployeeWithDepartmentSpecification(string departmentName)
            : base(e => e.Department.Name.Contains(departmentName))
        {
            AddInclude(e => e.Department);
            ApplyOrderBy(e => e.LastName);
        }
    }

    public class ActiveEmployeesSpecification : BaseSpecification<Employee>
    {
        public ActiveEmployeesSpecification()
            : base(e => e.Status == EmployeeStatus.Active)
        {
            ApplyOrderBy(e => e.LastName);
        }
    }

    public class EmployeesByDepartmentSpecification : BaseSpecification<Employee>
    {
        public EmployeesByDepartmentSpecification(int departmentId)
            : base(e => e.DepartmentId == departmentId)
        {
            AddInclude(e => e.Department);
        }
    }

    public class EmployeesByManagerSpecification : BaseSpecification<Employee>
    {
        public EmployeesByManagerSpecification(int managerId)
            : base(e => e.ManagerId == managerId)
        {
            AddInclude(e => e.Department);
        }
    }
}