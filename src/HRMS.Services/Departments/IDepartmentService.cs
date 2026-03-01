using HRMS.Services.Departments.Dtos;

namespace HRMS.Services.Departments
{
    public interface IDepartmentService
    {
        // Get operations
        Task<DepartmentDto?> GetDepartmentByIdAsync(int id);
        Task<IEnumerable<DepartmentListDto>> GetAllDepartmentsAsync();
        Task<DepartmentDetailDto?> GetDepartmentWithEmployeesAsync(int id);

        // Create/Update/Delete
        Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentDto createDto);
        Task<DepartmentDto> UpdateDepartmentAsync(UpdateDepartmentDto updateDto);
        Task<bool> DeleteDepartmentAsync(int id);

        // Validation
        Task<bool> IsDepartmentCodeUniqueAsync(string code, int? excludeId = null);
        Task<bool> IsDepartmentNameUniqueAsync(string name, int? excludeId = null);

        // Statistics
        Task<int> GetDepartmentCountAsync();
        Task<Dictionary<string, int>> GetEmployeeCountByDepartmentAsync();
    }
}