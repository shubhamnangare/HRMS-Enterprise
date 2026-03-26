namespace HRMS.Core.Enums
{
    // Employee enums
    public enum Gender
    {
        Male = 1,
        Female = 2,
        Other = 3
    }

    public enum MaritalStatus
    {
        Single = 1,
        Married = 2,
        Divorced = 3,
        Widowed = 4
    }

    public enum EmploymentType
    {
        Permanent = 1,
        Contract = 2,
        Intern = 3,
        Probation = 4,
        Temporary = 5
    }

    public enum EmployeeStatus
    {
        Active = 1,
        Inactive = 2,
        OnLeave = 3,
        Terminated = 4,
        Resigned = 5
    }

    // Leave enums
    public enum LeaveType
    {
        Annual = 1,
        Sick = 2,
        Maternity = 3,
        Paternity = 4,
        Unpaid = 5,
        Compensatory = 6,
        Bereavement = 7,
        Marriage = 8,
        Emergency = 9
    }

    public enum LeaveStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        Cancelled = 4
    }

    // Attendance enums
    public enum AttendanceStatus
    {
        Present = 1,
        Absent = 2,
        Late = 3,
        HalfDay = 4,
        Holiday = 5,
        Weekend = 6,
        OnLeave = 7,
        Remote = 8
    }

    // Audit enums
    public enum AuditAction
    {
        Create = 1,
        Update = 2,
        Delete = 3,
        View = 4,
        Login = 5,
        Logout = 6,
        Export = 7,
        Import = 8
    }

    public enum EntityType
    {
        Employee = 1,
        Department = 2,
        Leave = 3,
        Attendance = 4,
        User = 5,
        Role = 6
    }
}