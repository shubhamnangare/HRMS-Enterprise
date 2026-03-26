using HRMS.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Core.DTOs.Leave
{
    public class CreateLeaveRequestDto
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
        public string Reason { get; set; } = string.Empty;

        public bool IsPaid { get; set; } = true;
    }
}