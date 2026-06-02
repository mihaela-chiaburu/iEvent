using System;

namespace iEvent.Application.DTOs
{
    public class AuditLogRespDto
    {
        public Guid LogId { get; set; }
        public Guid AdminId { get; set; }
        public string Action { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
