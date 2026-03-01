using HRMS.Core.Entities;

namespace HRMS.Core.Interfaces.Repositories
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        Task<Employee?> GetEmployeeWithDetailsAsync(int id);
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId);
        Task<IEnumerable<Employee>> GetEmployeesByManagerAsync(int managerId);
        Task<IEnumerable<Employee>> SearchEmployeesAsync(string searchTerm);
        Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
        Task<bool> IsEmployeeCodeUniqueAsync(string code, int? excludeId = null);
        Task<int> GetEmployeeCountByDepartmentAsync(int departmentId);
        Task<IEnumerable<Employee>> GetEmployeesHiredBetweenAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Employee>> GetEmployeesWithUpcomingBirthdaysAsync(int days);
    }
}