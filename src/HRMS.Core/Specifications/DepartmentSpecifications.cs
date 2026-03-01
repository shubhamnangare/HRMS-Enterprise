using HRMS.Core.Entities;

namespace HRMS.Core.Specifications
{
    public class DepartmentWithEmployeesSpecification : BaseSpecification<Department>
    {
        public DepartmentWithEmployeesSpecification(int id)
            : base(d => d.Id == id)
        {
            AddInclude(d => d.Employees);
            AddInclude(d => d.Manager);
        }
    }

    public class DepartmentsWithManagersSpecification : BaseSpecification<Department>
    {
        public DepartmentsWithManagersSpecification()
        {
            AddInclude(d => d.Manager);
            AddInclude(d => d.Employees);
        }
    }
}