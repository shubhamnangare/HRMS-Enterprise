using HRMS.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Services.Leave.Dtos;

public class CreateLeaveDto
{
    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public LeaveType LeaveType { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    [StringLength(500)]
    public string Reason { get; set; }

    public bool IsPaid { get; set; } = true;
}

public class UpdateLeaveDto
{
    public int Id { get; set; }

    public LeaveType? LeaveType { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [StringLength(500)]
    public string? Reason { get; set; }

    public bool? IsPaid { get; set; }
}

public class ApproveLeaveDto
{
    [Required]
    public int Id { get; set; }

    [StringLength(500)]
    public string? Remarks { get; set; }
}

public class RejectLeaveDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    [StringLength(500)]
    public string Remarks { get; set; }
}

public class CancelLeaveDto
{
    [Required]
    public int Id { get; set; }

    [StringLength(500)]
    public string? Remarks { get; set; }
}

public class LeaveRequestDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public string EmployeeCode { get; set; }
    public string Department { get; set; }
    public string LeaveType { get; set; }
    public LeaveType LeaveTypeEnum { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalDays { get; set; }
    public string Reason { get; set; }
    public string Status { get; set; }
    public LeaveStatus StatusEnum { get; set; }
    public int? ApprovedById { get; set; }
    public string ApprovedByName { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string Remarks { get; set; }
    public bool IsPaid { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class LeaveBalanceDto
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public int Year { get; set; }
    public Dictionary<LeaveType, LeaveTypeBalance> Balances { get; set; }
}

public class LeaveTypeBalance
{
    public LeaveType LeaveType { get; set; }
    public string LeaveTypeName { get; set; }
    public decimal TotalEntitled { get; set; }
    public decimal Used { get; set; }
    public decimal Remaining { get; set; }
    public decimal Pending { get; set; }
    public bool IsPaid { get; set; }
}

public class LeaveFilterDto
{
    public int? EmployeeId { get; set; }
    public LeaveType? LeaveType { get; set; }
    public LeaveStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int? DepartmentId { get; set; }
}

public class LeaveStatisticsDto
{
    public int TotalRequests { get; set; }
    public int PendingRequests { get; set; }
    public int ApprovedRequests { get; set; }
    public int RejectedRequests { get; set; }
    public int CancelledRequests { get; set; }
    public decimal TotalLeaveDays { get; set; }
    public Dictionary<string, int> LeaveByType { get; set; }
    public Dictionary<string, int> LeaveByDepartment { get; set; }
    public Dictionary<string, int> LeaveByStatus { get; set; }
    public List<UpcomingLeaveDto> UpcomingLeaves { get; set; }
    public MonthlyLeaveSummary MonthlySummary { get; set; }
}

public class UpcomingLeaveDto
{
    public int Id { get; set; }
    public string EmployeeName { get; set; }
    public string Department { get; set; }
    public string LeaveType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Days { get; set; }
    public string Status { get; set; }
}

public class MonthlyLeaveSummary
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; }
    public int TotalLeaves { get; set; }
    public int ApprovedLeaves { get; set; }
    public int PendingLeaves { get; set; }
    public decimal TotalLeaveDays { get; set; }
    public Dictionary<string, int> LeaveByType { get; set; }
}