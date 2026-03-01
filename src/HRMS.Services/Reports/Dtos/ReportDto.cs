using HRMS.Core.Enums;

namespace HRMS.Services.Reports.Dtos
{
    public class EmployeeReportDto
    {
        public DateTime ReportDate { get; set; }
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int InactiveEmployees { get; set; }
        public int TerminatedEmployees { get; set; }
        public Dictionary<string, int> EmployeesByDepartment { get; set; } = new();
        public Dictionary<EmploymentType, int> EmployeesByType { get; set; } = new();
        public Dictionary<Gender, int> EmployeesByGender { get; set; } = new();
        public List<NewHireDto> NewHires { get; set; } = new();
    }

    public class NewHireDto
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
    }

    public class LeaveReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int PendingRequests { get; set; }
        public int RejectedRequests { get; set; }
        public decimal TotalLeaveDays { get; set; }
        public Dictionary<LeaveType, LeaveSummaryDto> LeaveSummary { get; set; } = new();
        public List<EmployeeLeaveDto> TopLeaveTakers { get; set; } = new();
    }

    public class LeaveSummaryDto
    {
        public int RequestCount { get; set; }
        public decimal TotalDays { get; set; }
        public int EmployeesCount { get; set; }
    }

    public class EmployeeLeaveDto
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public decimal TotalDays { get; set; }
        public int RequestCount { get; set; }
    }

    public class AttendanceReportDto
    {
        public DateTime Month { get; set; }
        public int TotalWorkingDays { get; set; }
        public double AverageAttendance { get; set; }
        public int TotalAbsences { get; set; }
        public int TotalLates { get; set; }
        public Dictionary<string, AttendanceSummaryDto> EmployeeAttendance { get; set; } = new();
    }

    public class AttendanceSummaryDto
    {
        public int Present { get; set; }
        public int Absent { get; set; }
        public int Late { get; set; }
        public int HalfDay { get; set; }
        public int OnLeave { get; set; }
        public double AttendancePercentage { get; set; }
    }
}