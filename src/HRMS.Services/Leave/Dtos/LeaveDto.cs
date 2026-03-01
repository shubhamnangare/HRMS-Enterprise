using HRMS.Core.Enums;

namespace HRMS.Services.Leave.Dtos
{
    public class LeaveRequestDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public LeaveType LeaveType { get; set; }
        public string LeaveTypeName => LeaveType.ToString();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalDays { get; set; }
        public string? Reason { get; set; }
        public LeaveStatus Status { get; set; }
        public string StatusName => Status.ToString();
        public string StatusBadgeClass => Status switch
        {
            LeaveStatus.Pending => "warning",
            LeaveStatus.Approved => "success",
            LeaveStatus.Rejected => "danger",
            LeaveStatus.Cancelled => "secondary",
            _ => "light"
        };
        public int? ApprovedById { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? Remarks { get; set; }
        public bool IsPaid { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateLeaveRequestDto
    {
        public int EmployeeId { get; set; }
        public LeaveType LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Reason { get; set; }
        public bool IsPaid { get; set; } = true;
    }

    public class UpdateLeaveRequestDto
    {
        public int Id { get; set; }
        public LeaveType LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Reason { get; set; }
    }

    public class ApproveLeaveDto
    {
        public int Id { get; set; }
        public string? Remarks { get; set; }
    }

    public class RejectLeaveDto
    {
        public int Id { get; set; }
        public string? Remarks { get; set; }
    }

    public class LeaveBalanceDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int Year { get; set; }
        public Dictionary<LeaveType, LeaveBalanceDetail> Balances { get; set; } = new();
    }

    public class LeaveBalanceDetail
    {
        public decimal Entitled { get; set; }
        public decimal Used { get; set; }
        public decimal Remaining => Entitled - Used;
        public decimal Pending { get; set; }
    }
}