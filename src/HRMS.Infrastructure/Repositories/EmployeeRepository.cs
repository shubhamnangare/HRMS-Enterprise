using HRMS.Core.Entities;
using HRMS.Core.Enums;
using HRMS.Core.Interfaces.Repositories;
using HRMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Infrastructure.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Employee?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(e => e.Email == email && !e.IsDeleted);
        }

        public async Task<Employee?> GetByEmployeeCodeAsync(string code)
        {
            return await _dbSet
                .FirstOrDefaultAsync(e => e.EmployeeCode == code && !e.IsDeleted);
        }

        public async Task<Employee?> GetEmployeeWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(e => e.Department)
                .Include(e => e.Manager)
                .Include(e => e.Subordinates.Where(s => !s.IsDeleted))
                .Include(e => e.LeaveRequests.Where(l => !l.IsDeleted))
                .Include(e => e.Attendances.Where(a => !a.IsDeleted))
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public async Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId)
        {
            return await _dbSet
                .Where(e => e.DepartmentId == departmentId && !e.IsDeleted)
                .Include(e => e.Department)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> SearchEmployeesAsync(string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await _dbSet
                    .Where(e => !e.IsDeleted)
                    .Include(e => e.Department)
                    .OrderBy(e => e.FirstName)
                    .ThenBy(e => e.LastName)
                    .ToListAsync();
            }

            searchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(e => !e.IsDeleted && (
                    e.FirstName.ToLower().Contains(searchTerm) ||
                    e.LastName.ToLower().Contains(searchTerm) ||
                    e.Email.ToLower().Contains(searchTerm) ||
                    e.EmployeeCode.ToLower().Contains(searchTerm) ||
                    (e.MiddleName != null && e.MiddleName.ToLower().Contains(searchTerm))))
                .Include(e => e.Department)
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> SearchEmployeesAsync(string? searchTerm, int? departmentId, EmployeeStatus? status)
        {
            var query = _dbSet.Where(e => !e.IsDeleted);

            // Apply search term filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(e =>
                    e.FirstName.ToLower().Contains(searchTerm) ||
                    e.LastName.ToLower().Contains(searchTerm) ||
                    e.Email.ToLower().Contains(searchTerm) ||
                    e.EmployeeCode.ToLower().Contains(searchTerm) ||
                    (e.MiddleName != null && e.MiddleName.ToLower().Contains(searchTerm)));
            }

            // Apply department filter
            if (departmentId.HasValue)
            {
                query = query.Where(e => e.DepartmentId == departmentId.Value);
            }

            // Apply status filter
            if (status.HasValue)
            {
                query = query.Where(e => e.Status == status.Value);
            }

            return await query
                .Include(e => e.Department)
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync();
        }
        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId)
        {
            return await _dbSet
                .Where(e => e.DepartmentId == departmentId && !e.IsDeleted)
                .Include(e => e.Department)
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByManagerAsync(int managerId)
        {
            return await _dbSet
                .Where(e => e.ManagerId == managerId && !e.IsDeleted)
                .Include(e => e.Manager)
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync();
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return !await _dbSet.AnyAsync(e => e.Email == email && e.Id != excludeId.Value && !e.IsDeleted);
            }
            return !await _dbSet.AnyAsync(e => e.Email == email && !e.IsDeleted);
        }

        public async Task<bool> IsEmployeeCodeUniqueAsync(string code, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return !await _dbSet.AnyAsync(e => e.EmployeeCode == code && e.Id != excludeId.Value && !e.IsDeleted);
            }
            return !await _dbSet.AnyAsync(e => e.EmployeeCode == code && !e.IsDeleted);
        }


        public void Update(Employee employee)
        {
            _dbSet.Update(employee);
        }
    }
}