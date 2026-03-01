using HRMS.Core.Enums;

namespace HRMS.Services.Dashboard.Dtos
{
    public class DashboardDto
    {
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int TotalDepartments { get; set; }
        public int EmployeesOnLeave { get; set; }
        public int PendingLeaveRequests { get; set; }
        public int NewHiresThisMonth { get; set; }
        public int BirthdaysThisMonth { get; set; }
        public List<RecentActivityDto> RecentActivities { get; set; } = new();
        public List<UpcomingEventDto> UpcomingEvents { get; set; } = new();
        public List<DepartmentStatsDto> DepartmentStats { get; set; } = new();
        public List<LeaveStatsDto> LeaveStats { get; set; } = new();
    }

    public class RecentActivityDto
    {
        public string ActivityType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }

    public class UpcomingEventDto
    {
        public string EventType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class DepartmentStatsDto
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
        public decimal BudgetUtilization { get; set; }
    }

    public class LeaveStatsDto
    {
        public LeaveType LeaveType { get; set; }
        public string LeaveTypeName => LeaveType.ToString();
        public int RequestCount { get; set; }
        public decimal TotalDays { get; set; }
    }

    public class ChartDataDto
    {
        public string[] Labels { get; set; } = Array.Empty<string>();
        public decimal[] Values { get; set; } = Array.Empty<decimal>();
        public string[] Colors { get; set; } = Array.Empty<string>();
    }
}