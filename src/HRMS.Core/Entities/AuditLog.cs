using HRMS.Core.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Core.Entities
{
    public class AuditLog : BaseEntity
    {
        public string? UserId { get; set; }

        [StringLength(100)]
        public string? UserName { get; set; }

        [StringLength(50)]
        public string Action { get; set; } = string.Empty;

        [StringLength(50)]
        public string Entity { get; set; } = string.Empty;

        [StringLength(50)]
        public string EntityId { get; set; } = string.Empty;

        public string? OldValues { get; set; } // JSON

        public string? NewValues { get; set; } // JSON

        [StringLength(50)]
        public string? IpAddress { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; }

        public DateTime Timestamp { get; set; }
    }
}