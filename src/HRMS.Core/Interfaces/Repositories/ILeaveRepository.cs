using HRMS.Core.Entities;
using HRMS.Core.Enums;

namespace HRMS.Core.Interfaces.Repositories
{
    public interface ILeaveRepository : IGenericRepository<LeaveRequest>
    {
        Task<IEnumerable<LeaveRequest>> GetLeaveRequestsByEmployeeAsync(int employeeId);
        Task<IEnumerable<LeaveRequest>> GetLeaveRequestsByStatusAsync(LeaveStatus status);
        Task<IEnumerable<LeaveRequest>> GetLeaveRequestsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalLeaveDaysAsync(int employeeId, int year, LeaveType? leaveType = null);
        Task<bool> HasOverlappingLeaveAsync(int employeeId, DateTime startDate, DateTime endDate, int? excludeId = null);
        Task<IEnumerable<LeaveRequest>> GetPendingApprovalsAsync(int managerId);
    }
}