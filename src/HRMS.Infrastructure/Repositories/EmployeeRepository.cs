using HRMS.Core.Entities;
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

        public async Task<Employee?> GetEmployeeWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(e => e.Department)
                .Include(e => e.Manager)
                .Include(e => e.Subordinates)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId)
        {
            return await _dbSet
                .Where(e => e.DepartmentId == departmentId)
                .Include(e => e.Department)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByManagerAsync(int managerId)
        {
            return await _dbSet
                .Where(e => e.ManagerId == managerId)
                .Include(e => e.Department)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> SearchEmployeesAsync(string searchTerm)
        {
            searchTerm = searchTerm.ToLower();
            return await _dbSet
                .Where(e => e.FirstName.ToLower().Contains(searchTerm) ||
                           e.LastName.ToLower().Contains(searchTerm) ||
                           e.Email.ToLower().Contains(searchTerm) ||
                           e.EmployeeCode.ToLower().Contains(searchTerm))
                .Include(e => e.Department)
                .ToListAsync();
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return !await _dbSet.AnyAsync(e => e.Email == email && e.Id != excludeId.Value);
            }
            return !await _dbSet.AnyAsync(e => e.Email == email);
        }

        public async Task<bool> IsEmployeeCodeUniqueAsync(string code, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return !await _dbSet.AnyAsync(e => e.EmployeeCode == code && e.Id != excludeId.Value);
            }
            return !await _dbSet.AnyAsync(e => e.EmployeeCode == code);
        }

        public async Task<int> GetEmployeeCountByDepartmentAsync(int departmentId)
        {
            return await _dbSet.CountAsync(e => e.DepartmentId == departmentId);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesHiredBetweenAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(e => e.HireDate >= startDate && e.HireDate <= endDate)
                .Include(e => e.Department)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesWithUpcomingBirthdaysAsync(int days)
        {
            var today = DateTime.Today;
            var targetDate = today.AddDays(days);

            return await _dbSet
                .Where(e => e.DateOfBirth.Month >= today.Month &&
                           e.DateOfBirth.DayOfYear <= targetDate.DayOfYear)
                .OrderBy(e => e.DateOfBirth)
                .ToListAsync();
        }
    }
}