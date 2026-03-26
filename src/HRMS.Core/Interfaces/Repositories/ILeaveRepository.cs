using HRMS.Core.Entities;
using HRMS.Core.Enums;

namespace HRMS.Core.Interfaces.Repositories
{
    public interface ILeaveRepository : IRepository<LeaveRequest>
    {
        Task<IEnumerable<LeaveRequest>> GetByEmployeeAsync(int employeeId);
        Task<IEnumerable<LeaveRequest>> GetPendingLeavesAsync();
        Task<bool> HasOverlapAsync(int employeeId, DateTime startDate, DateTime endDate, int? excludeId = null);
        Task<int> GetUsedLeaveDaysAsync(int employeeId, LeaveType leaveType, int year);
        Task<int> GetEmployeesOnLeaveAsync(DateTime date);
        Task<IEnumerable<Employee>> GetEmployeesEntitiesOnLeaveAsync(DateTime date);
        Task<IEnumerable<LeaveRequest>> GetUpcomingLeavesAsync(int days = 7);
        void Update(LeaveRequest leave);
    }
}