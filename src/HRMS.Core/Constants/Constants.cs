using HRMS.Core.Enums;

namespace HRMS.Core.Constants;

public static class Constants
{
    // Leave Entitlements (days per year)
    public static class LeaveEntitlement
    {
        public const int AnnualLeaveNewJoiner = 12;
        public const int AnnualLeaveStandard = 15;
        public const int AnnualLeaveSenior = 18;
        public const int AnnualLeaveExecutive = 21;
        public const int SickLeave = 12;
        public const int CasualLeave = 10;
        public const int MaternityLeave = 90;
        public const int PaternityLeave = 10;
        public const int BereavementLeave = 5;
        public const int MarriageLeave = 5;
        public const int EmergencyLeave = 3;
        public const int MaxUnpaidLeave = 30;
    }

    // Years of service thresholds
    public static class ServiceYears
    {
        public const int NewJoiner = 0;
        public const int Standard = 1;
        public const int Senior = 3;
        public const int Executive = 5;
    }

    // Attendance settings
    public static class AttendanceSettings
    {
        public static TimeSpan OfficeStartTime = new TimeSpan(9, 0, 0); // 9:00 AM
        public static TimeSpan OfficeEndTime = new TimeSpan(18, 0, 0); // 6:00 PM
        public static TimeSpan LateThreshold = new TimeSpan(0, 15, 0); // 15 minutes grace
        public static TimeSpan HalfDayThreshold = new TimeSpan(4, 0, 0); // 4 hours = half day
        public const decimal OvertimeMultiplier = 1.5m; // 1.5x pay for overtime
        public const int WorkingHoursPerDay = 8;
    }

    // Display names for enums
    public static readonly Dictionary<LeaveType, string> LeaveTypeNames = new()
    {
        { LeaveType.Annual, "Annual Leave" },
        { LeaveType.Sick, "Sick Leave" },
        { LeaveType.Maternity, "Maternity Leave" },
        { LeaveType.Paternity, "Paternity Leave" },
        { LeaveType.Unpaid, "Unpaid Leave" },
        { LeaveType.Compensatory, "Compensatory Leave" },
        { LeaveType.Bereavement, "Bereavement Leave" },
        { LeaveType.Marriage, "Marriage Leave" },
        { LeaveType.Emergency, "Emergency Leave" }
    };

    public static readonly Dictionary<AttendanceStatus, string> AttendanceStatusNames = new()
    {
        { AttendanceStatus.Present, "Present" },
        { AttendanceStatus.Absent, "Absent" },
        { AttendanceStatus.Late, "Late" },
        { AttendanceStatus.HalfDay, "Half Day" },
        { AttendanceStatus.Holiday, "Holiday" },
        { AttendanceStatus.Weekend, "Weekend" },
        { AttendanceStatus.OnLeave, "On Leave" },
        { AttendanceStatus.Remote, "Remote Work" }
    };

    public static readonly HashSet<LeaveType> PaidLeaveTypes = new()
    {
        LeaveType.Annual,
        LeaveType.Sick,
        LeaveType.Maternity,
        LeaveType.Paternity,
        LeaveType.Compensatory,
        LeaveType.Bereavement,
        LeaveType.Marriage,
        LeaveType.Emergency
    };

    // Validation messages
    public static class Messages
    {
        // Common messages
        public const string NotFound = "{0} not found";
        public const string AlreadyExists = "{0} already exists";

        // Leave messages
        public const string StartDateAfterEndDate = "Start date cannot be after end date";
        public const string PastLeaveNotAllowed = "Cannot apply for leave in the past";
        public const string OverlapDetected = "Leave dates overlap with existing request";
        public const string InsufficientBalance = "Insufficient {0} leave balance. Available: {1} days";
        public const string LeaveNotFound = "Leave request not found";
        public const string AlreadyProcessed = "Leave request already {0}";
        public const string CannotCancelStartedLeave = "Cannot cancel leave that has already started";

        // Employee messages
        public const string EmployeeNotFound = "Employee not found";
        public const string DuplicateEmail = "Email already exists";
        public const string DuplicateEmployeeCode = "Employee code already exists";

        // Department messages
        public const string DepartmentNotFound = "Department not found";
        public const string DuplicateDepartmentCode = "Department code already exists";
        public const string DepartmentHasEmployees = "Cannot delete department with assigned employees";

        // Attendance messages
        public const string AttendanceNotFound = "Attendance record not found";
        public const string AlreadyMarked = "Attendance already marked for {0:dd/MM/yyyy}";
        public const string CannotMarkFuture = "Cannot mark attendance for future dates";
        public const string InvalidCheckTime = "Check-out time must be after check-in time";
        public const string EmployeeNotActive = "Cannot mark attendance for inactive employee";
    }
}