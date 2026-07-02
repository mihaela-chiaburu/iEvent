using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iEvent.Domain.Entities
{
    public class AuditLog
    {
        [Key]
        public Guid LogId { get; set; }

        [Required]
        public Guid AdminId { get; set; }

        [Required]
        [StringLength(300)]
        public string Action { get; set; } = string.Empty;

        [Required]
        public DateTime Timestamp { get; set; }

        [ForeignKey(nameof(AdminId))]
        public AdminUser? AdminUser { get; set; }
    }
}
