using HRMS.Core.Entities;
using HRMS.Core.Enums;
using HRMS.Core.Interfaces.Repositories;
using HRMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Infrastructure.Repositories
{
    public class LeaveRepository : GenericRepository<LeaveRequest>, ILeaveRepository
    {
        public LeaveRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<LeaveRequest>> GetByEmployeeAsync(int employeeId)
        {
            return await _dbSet
                .Where(l => l.EmployeeId == employeeId && !l.IsDeleted)
                .Include(l => l.Employee)
                .Include(l => l.ApprovedBy)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<LeaveRequest>> GetPendingLeavesAsync()
        {
            return await _dbSet
                .Where(l => l.Status == LeaveStatus.Pending && !l.IsDeleted)
                .Include(l => l.Employee)
                .ThenInclude(e => e.Department)
                .OrderBy(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> HasOverlapAsync(int employeeId, DateTime startDate, DateTime endDate, int? excludeId = null)
        {
            var query = _dbSet.Where(l => l.EmployeeId == employeeId
                && !l.IsDeleted
                && l.Status != LeaveStatus.Rejected
                && l.Status != LeaveStatus.Cancelled
                && ((startDate >= l.StartDate && startDate <= l.EndDate) ||
                    (endDate >= l.StartDate && endDate <= l.EndDate) ||
                    (startDate <= l.StartDate && endDate >= l.EndDate)));

            if (excludeId.HasValue)
                query = query.Where(l => l.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<int> GetUsedLeaveDaysAsync(int employeeId, LeaveType leaveType, int year)
        {
            return (int)await _dbSet
                .Where(l => l.EmployeeId == employeeId
                    && l.LeaveType == leaveType
                    && l.StartDate.Year == year
                    && l.Status == LeaveStatus.Approved
                    && !l.IsDeleted)
                .SumAsync(l => l.TotalDays);
        }

        public async Task<int> GetEmployeesOnLeaveAsync(DateTime date)
        {
            var employeeIds = await _dbSet
                .Where(l => !l.IsDeleted
                    && l.Status == LeaveStatus.Approved
                    && l.StartDate <= date
                    && l.EndDate >= date)
                .Select(l => l.EmployeeId)
                .Distinct()
                .ToListAsync();

            return employeeIds.Count;
        }

        public void Update(LeaveRequest leave)
        {
            _dbSet.Update(leave);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesEntitiesOnLeaveAsync(DateTime date)
        {
            var employeeIds = await _dbSet
                .Where(l => !l.IsDeleted
                    && l.Status == LeaveStatus.Approved
                    && l.StartDate <= date
                    && l.EndDate >= date)
                .Select(l => l.EmployeeId)
                .Distinct()
                .ToListAsync();

            if (!employeeIds.Any())
                return new List<Employee>();

            return await _context.Employees
                .Where(e => employeeIds.Contains(e.Id) && !e.IsDeleted)
                .Include(e => e.Department)
                .ToListAsync();
        }

        public async Task<IEnumerable<LeaveRequest>> GetUpcomingLeavesAsync(int days = 7)
        {
            var startDate = DateTime.Today;
            var endDate = startDate.AddDays(days);

            return await _dbSet
                .Where(l => !l.IsDeleted
                    && l.Status == LeaveStatus.Approved
                    && l.StartDate >= startDate
                    && l.StartDate <= endDate)
                .Include(l => l.Employee)
                .ThenInclude(e => e.Department)
                .OrderBy(l => l.StartDate)
                .ToListAsync();
        }
    }
}