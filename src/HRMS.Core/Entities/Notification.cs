using HRMS.Core.Entities.Base;
using HRMS.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Core.Entities
{
    public class Notification : BaseEntity
    {
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;

        public NotificationType Type { get; set; }

        public string? UserId { get; set; }

        public bool IsRead { get; set; }

        public DateTime? ReadAt { get; set; }

        [StringLength(200)]
        public string? Link { get; set; }

        public bool IsGlobal { get; set; }
    }
}