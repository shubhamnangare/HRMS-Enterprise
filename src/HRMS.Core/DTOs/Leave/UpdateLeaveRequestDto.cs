using HRMS.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Core.DTOs.Leave;

public class UpdateLeaveRequestDto
{
    [Required]
    public int Id { get; set; }

    public LeaveType? LeaveType { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [StringLength(500)]
    public string? Reason { get; set; }

    public bool? IsPaid { get; set; }
}