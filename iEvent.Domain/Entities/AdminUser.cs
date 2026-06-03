using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using iEvent.Domain.Enums;

namespace iEvent.Domain.Entities
{
    public class AdminUser
    {
        [Key]
        public Guid AdminId { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(450)]
        public string IdentityUserId { get; set; } = string.Empty;

        public List<AuditLog> AuditLogs { get; set; } = new();
    }
}
