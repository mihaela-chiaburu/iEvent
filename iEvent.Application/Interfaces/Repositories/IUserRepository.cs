using iEvent.Application.DTOs;

namespace iEvent.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<PagedResult<UserRespDto>> GetUsersAsync(UserFilterDto filter);
        Task<bool> ExistsByEmailAsync(string email);
        Task<IdentityResultDto> CreateUserWithRoleAsync(string email, string password, string role, string? PhoneNumber);
        Task<IdentityResultDto> UpdateUserRoleAsync(string userId, string newRole);
        Task<(string Id, string Email, string Name, string? PhoneNumber)?> GetUserBasicInfoAsync(string userId);
    }
}
