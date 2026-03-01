using HRMS.Core.Entities;

namespace HRMS.Core.Interfaces.Repositories
{
    public interface IDepartmentRepository : IGenericRepository<Department>
    {
        Task<Department?> GetDepartmentWithEmployeesAsync(int id);
        Task<Department?> GetDepartmentWithManagerAsync(int id);
        Task<IEnumerable<Department>> GetDepartmentsWithEmployeeCountAsync();
        Task<bool> IsDepartmentCodeUniqueAsync(string code, int? excludeId = null);
        Task<bool> IsDepartmentNameUniqueAsync(string name, int? excludeId = null);
        Task<int> GetTotalEmployeesInDepartmentAsync(int departmentId);
        Task<decimal> GetDepartmentBudgetAsync(int departmentId);
    }
}