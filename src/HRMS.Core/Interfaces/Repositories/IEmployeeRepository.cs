using HRMS.Core.Entities;
using HRMS.Core.Enums;

namespace HRMS.Core.Interfaces.Repositories;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<Employee?> GetByEmailAsync(string email);
    Task<Employee?> GetByEmployeeCodeAsync(string code);
    Task<Employee?> GetEmployeeWithDetailsAsync(int id);
    Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId);
    Task<IEnumerable<Employee>> SearchEmployeesAsync(string? searchTerm,int? deptId, EmployeeStatus? status);
    Task<IEnumerable<Employee>> SearchEmployeesAsync(string? searchTerm);
    Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId);
    Task<IEnumerable<Employee>> GetEmployeesByManagerAsync(int managerId);
    Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
    Task<bool> IsEmployeeCodeUniqueAsync(string code, int? excludeId = null);
    void Update(Employee employee);
}