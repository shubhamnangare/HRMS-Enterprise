using HRMS.Core.Entities.Base;
using HRMS.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Core.Entities
{
    public class Attendance : BaseEntity
    {
        public int EmployeeId { get; set; }

        public DateTime Date { get; set; }

        public DateTime? CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        public TimeSpan? TotalHours { get; set; }

        public AttendanceStatus Status { get; set; }

        public decimal OvertimeHours { get; set; }

        public decimal LateMinutes { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation properties
        public virtual Employee Employee { get; set; } = null!;
    }
}