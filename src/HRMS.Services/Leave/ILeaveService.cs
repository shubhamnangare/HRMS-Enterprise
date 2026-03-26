using HRMS.Core.DTOs.Leave;
using HRMS.Core.Entities;
using HRMS.Core.Enums;
using HRMS.Services.Leave.Dtos;

namespace HRMS.Services.Leave
{
    public interface ILeaveService
    {
        Task<LeaveRequestDto?> GetLeaveRequestByIdAsync(int id);
        Task<IEnumerable<LeaveRequestDto>> GetLeaveRequestsByEmployeeAsync(int employeeId);
        Task<IEnumerable<LeaveRequestDto>> GetAllLeaveRequestsAsync();
        Task<LeaveRequestDto> CreateLeaveRequestAsync(CreateLeaveRequestDto createDto);
        Task<LeaveRequestDto> UpdateLeaveRequestAsync(UpdateLeaveRequestDto updateDto);
        Task<bool> CancelLeaveRequestAsync(int id);

        // Approve/Reject (for HR/Admin)
        Task<LeaveRequestDto> ApproveLeaveRequestAsync(int id, int approverId, string? remarks = null);
        Task<LeaveRequestDto> RejectLeaveRequestAsync(int id, int approverId, string remarks);

        // Leave Balance
        Task<LeaveBalanceDto> GetLeaveBalanceAsync(int employeeId, int year);

        // Validation
        Task<bool> CanApplyLeaveAsync(int employeeId, LeaveType leaveType, DateTime startDate, DateTime endDate);


        // Dashboard/HR
        Task<IEnumerable<LeaveRequestDto>> GetPendingLeavesAsync();
        Task<IEnumerable<Employee>> GetEmployeesOnLeaveAsync(DateTime date);
    }
}