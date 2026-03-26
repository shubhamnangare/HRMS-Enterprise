using HRMS.Core.Entities;

namespace HRMS.Core.Interfaces.Repositories;

public interface IDepartmentRepository : IRepository<Department>
{
    Task<Department?> GetByCodeAsync(string code);
    Task<Department?> GetDepartmentWithEmployeesAsync(int id);
    Task<bool> HasEmployeesAsync(int departmentId);
    Task<IEnumerable<Department>> GetDepartmentsWithEmployeeCountAsync();
    Task<bool> IsDepartmentCodeUniqueAsync(string code, int? excludeId = null);
    Task<bool> IsDepartmentNameUniqueAsync(string name, int? excludeId = null);
    Task<Department> GetDepartmentWithManagerAsync(int id);
    void Update(Department department);
}