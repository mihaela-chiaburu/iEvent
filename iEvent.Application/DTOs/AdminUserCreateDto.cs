using iEvent.Domain.Enums;

namespace iEvent.Application.DTOs
{
    public class AdminUserCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public AdminRole Role { get; set; } = AdminRole.EventManager;
    }
}
