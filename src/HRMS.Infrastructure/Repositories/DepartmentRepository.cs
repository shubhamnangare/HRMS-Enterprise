using HRMS.Core.Entities;
using HRMS.Core.Interfaces.Repositories;
using HRMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Infrastructure.Repositories
{
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Department?> GetDepartmentWithEmployeesAsync(int id)
        {
            return await _dbSet
                .Include(d => d.Employees)
                .Include(d => d.Manager)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Department?> GetDepartmentWithManagerAsync(int id)
        {
            return await _dbSet
                .Include(d => d.Manager)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<Department>> GetDepartmentsWithEmployeeCountAsync()
        {
            return await _dbSet
                .Include(d => d.Employees)
                .Include(d => d.Manager)
                .Select(d => new Department
                {
                    Id = d.Id,
                    Code = d.Code,
                    Name = d.Name,
                    Description = d.Description,
                    Location = d.Location,
                    Phone = d.Phone,
                    Email = d.Email,
                    Budget = d.Budget,
                    Manager = d.Manager,
                    Employees = d.Employees // This will load the employees collection
                })
                .ToListAsync();
        }

        public async Task<bool> IsDepartmentCodeUniqueAsync(string code, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return !await _dbSet.AnyAsync(d => d.Code == code && d.Id != excludeId.Value);
            }
            return !await _dbSet.AnyAsync(d => d.Code == code);
        }

        public async Task<bool> IsDepartmentNameUniqueAsync(string name, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return !await _dbSet.AnyAsync(d => d.Name == name && d.Id != excludeId.Value);
            }
            return !await _dbSet.AnyAsync(d => d.Name == name);
        }

        public async Task<int> GetTotalEmployeesInDepartmentAsync(int departmentId)
        {
            return await _dbSet
                .Where(d => d.Id == departmentId)
                .Select(d => d.Employees.Count)
                .FirstOrDefaultAsync();
        }

        public async Task<decimal> GetDepartmentBudgetAsync(int departmentId)
        {
            var department = await _dbSet
                .Where(d => d.Id == departmentId)
                .FirstOrDefaultAsync();

            return department?.Budget ?? 0;
        }
    }
}