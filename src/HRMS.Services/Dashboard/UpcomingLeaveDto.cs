public class UpcomingLeaveDto
{
    public string? EmployeeName { get; set; }
    public string? Department { get; set; }
    public string LeaveTypeName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Days { get; set; }
    public string Status { get; set; }
}