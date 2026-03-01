using HRMS.Core.Entities;

namespace HRMS.Core.Interfaces.Services
{
    public interface IEmployeeService
    {
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<Employee> CreateEmployeeAsync(Employee employee);
        Task<Employee> UpdateEmployeeAsync(Employee employee);
        Task<bool> DeleteEmployeeAsync(int id);
        Task<IEnumerable<Employee>> SearchEmployeesAsync(string searchTerm);
        Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
        Task<byte[]> ExportEmployeesToExcelAsync();
        Task ImportEmployeesFromExcelAsync(byte[] fileData);
    }
}