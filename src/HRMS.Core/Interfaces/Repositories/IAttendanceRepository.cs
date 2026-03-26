using HRMS.Core.Entities;

namespace HRMS.Core.Interfaces.Repositories
{
    public interface IAttendanceRepository : IRepository<Attendance>
    {
        Task<Attendance?> GetByDateAsync(int employeeId, DateTime date);
        Task<IEnumerable<Attendance>> GetByEmployeeAsync(int employeeId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<decimal> GetTotalWorkingHoursAsync(int employeeId, DateTime fromDate, DateTime toDate);
    }
}