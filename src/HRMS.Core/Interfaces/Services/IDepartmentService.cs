using HRMS.Core.Entities;

namespace HRMS.Core.Interfaces.Services
{
    public interface IDepartmentService
    {
        Task<Department?> GetDepartmentByIdAsync(int id);
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();
        Task<Department> CreateDepartmentAsync(Department department);
        Task<Department> UpdateDepartmentAsync(Department department);
        Task<bool> DeleteDepartmentAsync(int id);
        Task<Department?> GetDepartmentWithEmployeesAsync(int id);
        Task<bool> IsDepartmentCodeUniqueAsync(string code, int? excludeId = null);
    }
}