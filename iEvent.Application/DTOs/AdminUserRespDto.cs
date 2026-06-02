using System;
using iEvent.Domain.Enums;

namespace iEvent.Application.DTOs
{
    public class AdminUserRespDto
    {
        public Guid AdminId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public AdminRole Role { get; set; }
    }
}
