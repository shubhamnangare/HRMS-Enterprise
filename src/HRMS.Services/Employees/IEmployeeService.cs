using HRMS.Core.Entities;
using HRMS.Services.Employees.Dtos;
using System.Security.Claims;

namespace HRMS.Services.Employees
{
    public interface IEmployeeService
    {
        Task<EmployeeDto?> GetEmployeeByIdAsync(int id);
        Task<Employee> GetCurrentEmployeeAsync(ClaimsPrincipal user);
        Task<IEnumerable<EmployeeListDto>> GetAllEmployeesAsync();
        Task<IEnumerable<EmployeeListDto>> SearchEmployeesAsync(EmployeeSearchDto searchDto);
        Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createDto);
        Task<EmployeeDto> UpdateEmployeeAsync(UpdateEmployeeDto updateDto);
        Task<bool> DeleteEmployeeAsync(int id);
        Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
        Task<bool> IsEmployeeCodeUniqueAsync(string code, int? excludeId = null);
        Task<int> GetEmployeeCountAsync();
        Task<byte[]> ExportEmployeesToExcelAsync(EmployeeSearchDto searchDto);
        Task<int> ImportEmployeesFromExcelAsync(byte[] fileData);
        Task<IEnumerable<EmployeeListDto>> GetEmployeesByDepartmentAsync(int departmentId);
        Task<IEnumerable<EmployeeListDto>> GetEmployeesByManagerAsync(int managerId);
    }
}