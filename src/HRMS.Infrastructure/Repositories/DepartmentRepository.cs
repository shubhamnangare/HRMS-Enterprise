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

        public async Task<Department?> GetByCodeAsync(string code)
        {
            return await _dbSet
                .FirstOrDefaultAsync(d => d.Code == code && !d.IsDeleted);
        }

        public async Task<Department?> GetDepartmentWithEmployeesAsync(int id)
        {
            return await _dbSet
                .Include(d => d.Employees.Where(e => !e.IsDeleted))
                .Include(d => d.Manager)
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
        }

        public async Task<bool> HasEmployeesAsync(int departmentId)
        {
            return await _dbSet
                .Where(d => d.Id == departmentId && !d.IsDeleted)
                .SelectMany(d => d.Employees)
                .AnyAsync(e => !e.IsDeleted);
        }

        public async Task<IEnumerable<Department>> GetDepartmentsWithEmployeeCountAsync()
        {
            return await _dbSet
                .Where(d => !d.IsDeleted)
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
                    ManagerId = d.ManagerId,
                    Manager = d.Manager,
                    CreatedAt = d.CreatedAt,
                    CreatedBy = d.CreatedBy,
                    UpdatedAt = d.UpdatedAt,
                    UpdatedBy = d.UpdatedBy,
                    IsDeleted = d.IsDeleted,
                    RowVersion = d.RowVersion,
                    Employees = d.Employees.Where(e => !e.IsDeleted).ToList()
                })
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<bool> IsDepartmentCodeUniqueAsync(string code, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return !await _dbSet.AnyAsync(d => d.Code == code && d.Id != excludeId.Value && !d.IsDeleted);
            }
            return !await _dbSet.AnyAsync(d => d.Code == code && !d.IsDeleted);
        }

        public async Task<bool> IsDepartmentNameUniqueAsync(string name, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return !await _dbSet.AnyAsync(d => d.Name == name && d.Id != excludeId.Value && !d.IsDeleted);
            }
            return !await _dbSet.AnyAsync(d => d.Name == name && !d.IsDeleted);
        }

        public async Task<Department> GetDepartmentWithManagerAsync(int id)
        {
           return await _dbSet
                .Include(d => d.Manager)
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
        }
       

        public void Update(Department department)
        {
            _dbSet.Update(department);
        }
    }
}