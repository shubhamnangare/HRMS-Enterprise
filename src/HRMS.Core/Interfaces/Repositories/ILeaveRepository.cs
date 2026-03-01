using HRMS.Core.Entities;
using HRMS.Core.Enums;

namespace HRMS.Core.Interfaces.Repositories
{
    public interface ILeaveRepository : IGenericRepository<LeaveRequest>
    {
        // Basic queries
        Task<IEnumerable<LeaveRequest>> GetLeaveRequestsByEmployeeAsync(int employeeId);
        Task<IEnumerable<LeaveRequest>> GetLeaveRequestsByStatusAsync(LeaveStatus status);
        Task<IEnumerable<LeaveRequest>> GetLeaveRequestsByDateRangeAsync(DateTime startDate, DateTime endDate);

        // Statistics and calculations
        Task<decimal> GetTotalLeaveDaysAsync(int employeeId, int year, LeaveType? leaveType = null);
        Task<Dictionary<LeaveType, decimal>> GetLeaveBalanceAsync(int employeeId, int year);
        Task<Dictionary<LeaveStatus, int>> GetLeaveStatisticsAsync(DateTime startDate, DateTime endDate);

        // Validation
        Task<bool> HasOverlappingLeaveAsync(int employeeId, DateTime startDate, DateTime endDate, int? excludeId = null);
        Task<bool> CanApplyLeaveAsync(int employeeId, LeaveType leaveType, DateTime startDate, DateTime endDate);
        Task<int> GetAvailableLeaveDaysAsync(int employeeId, int year, LeaveType leaveType);

        // Manager/Admin queries
        Task<IEnumerable<LeaveRequest>> GetPendingApprovalsAsync(int managerId);
        Task<IEnumerable<LeaveRequest>> GetLeaveRequestsByDepartmentAsync(int departmentId, DateTime startDate, DateTime endDate);

        // Dashboard queries
        Task<IEnumerable<LeaveRequest>> GetUpcomingLeavesAsync(int days);
        Task<int> GetEmployeesOnLeaveAsync(DateTime date);
    }
}