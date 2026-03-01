using HRMS.Core.Entities.Base;
using HRMS.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Core.Entities
{
    public class LeaveRequest : BaseEntity
    {
        [Required]
        public int EmployeeId { get; set; }

        public LeaveType LeaveType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal TotalDays { get; set; }

        [StringLength(500)]
        public string? Reason { get; set; }

        public LeaveStatus Status { get; set; }

        public int? ApprovedById { get; set; }

        public DateTime? ApprovedDate { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        public bool IsPaid { get; set; }

        // Navigation properties
        public virtual Employee Employee { get; set; } = null!;
        public virtual Employee? ApprovedBy { get; set; }
    }
}