using HRMS.Core.Entities;
using HRMS.Core.Enums;
using HRMS.Core.Interfaces.Repositories;
using HRMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Infrastructure.Repositories
{
    public class AttendanceRepository : GenericRepository<Attendance>, IAttendanceRepository
    {
        public AttendanceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Attendance?> GetByDateAsync(int employeeId, DateTime date)
        {
            return await _dbSet
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date.Date == date.Date && !a.IsDeleted);
        }

        public async Task<IEnumerable<Attendance>> GetByEmployeeAsync(int employeeId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _dbSet.Where(a => a.EmployeeId == employeeId && !a.IsDeleted);

            if (fromDate.HasValue)
                query = query.Where(a => a.Date >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(a => a.Date <= toDate.Value);

            return await query
                .OrderByDescending(a => a.Date)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalWorkingHoursAsync(int employeeId, DateTime fromDate, DateTime toDate)
        {
            return await _dbSet
                .Where(a => a.EmployeeId == employeeId
                    && a.Date >= fromDate
                    && a.Date <= toDate
                    && !a.IsDeleted
                    && a.Status == AttendanceStatus.Present)
                .SumAsync(a => a.TotalHours ?? 0);
        }
    }
}