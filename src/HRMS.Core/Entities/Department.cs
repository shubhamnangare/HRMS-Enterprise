using HRMS.Core.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Core.Entities
{
    public class Department : BaseEntity
    {
        [Required]
        [StringLength(20)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        public decimal? Budget { get; set; }

        public int? ManagerId { get; set; }

        // Navigation properties
        public virtual Employee? Manager { get; set; }
        public virtual ICollection<Employee> Employees { get; set; } = new HashSet<Employee>();
    }
}