using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Models.Entities
{
    public class AuditLog
    {
        [Key]
        public int LogId { get; set; }

        [StringLength(450)]
        public string? UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Action { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Entity { get; set; } = string.Empty;

        [StringLength(100)]
        public string? EntityId { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string? IPAddress { get; set; }

        [StringLength(2000)]
        public string? Details { get; set; }
    }
}
