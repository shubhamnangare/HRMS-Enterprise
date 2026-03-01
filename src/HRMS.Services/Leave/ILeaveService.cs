using HRMS.Core.Enums;
using HRMS.Services.Leave.Dtos;

namespace HRMS.Services.Leave
{
    public interface ILeaveService
    {
        // Get operations
        Task<LeaveRequestDto?> GetLeaveRequestByIdAsync(int id);
        Task<IEnumerable<LeaveRequestDto>> GetAllLeaveRequestsAsync();
        Task<IEnumerable<LeaveRequestDto>> GetLeaveRequestsByEmployeeAsync(int employeeId);
        Task<IEnumerable<LeaveRequestDto>> GetLeaveRequestsByStatusAsync(LeaveStatus status);
        Task<IEnumerable<LeaveRequestDto>> GetLeaveRequestsByDateRangeAsync(DateTime startDate, DateTime endDate);

        // Create/Update/Delete
        Task<LeaveRequestDto> CreateLeaveRequestAsync(CreateLeaveRequestDto createDto);
        Task<LeaveRequestDto> UpdateLeaveRequestAsync(UpdateLeaveRequestDto updateDto);
        Task<LeaveRequestDto> ApproveLeaveRequestAsync(ApproveLeaveDto approveDto, int approverId);
        Task<LeaveRequestDto> RejectLeaveRequestAsync(RejectLeaveDto rejectDto, int approverId);
        Task<bool> CancelLeaveRequestAsync(int id, int employeeId);
        Task<bool> DeleteLeaveRequestAsync(int id);

        // Balance and validation
        Task<LeaveBalanceDto> GetLeaveBalanceAsync(int employeeId, int year);
        Task<int> GetAvailableLeaveDaysAsync(int employeeId, int year, LeaveType leaveType);
        Task<bool> CanApplyLeaveAsync(int employeeId, LeaveType leaveType, DateTime startDate, DateTime endDate);
        Task<bool> HasOverlappingLeaveAsync(int employeeId, DateTime startDate, DateTime endDate, int? excludeId = null);

        // Manager/Admin operations
        Task<IEnumerable<LeaveRequestDto>> GetPendingApprovalsAsync(int managerId);
        Task<IEnumerable<LeaveRequestDto>> GetTeamLeaveCalendarAsync(int managerId, DateTime month);
        Task<Dictionary<LeaveStatus, int>> GetLeaveStatisticsAsync(DateTime startDate, DateTime endDate);

        // Dashboard
        Task<int> GetEmployeesOnLeaveAsync(DateTime date);
        Task<IEnumerable<LeaveRequestDto>> GetUpcomingLeavesAsync(int days);
    }
}