using iEvent.Application.DTOs.Common;
using iEvent.Application.DTOs.User;

namespace iEvent.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<PagedResultDto<UserRespDto>> GetUsersAsync(UserFilterDto filter);
        Task<bool> ExistsByEmailAsync(string email);
        Task<(string Id, string Email, string? PhoneNumber, IList<string> Roles)?> ValidateCredentialsAsync(string email, string password);
        Task<IdentityResultDto> CreateUserWithRoleAsync(string email, string password, string role, string? PhoneNumber);
        Task<IdentityResultDto> UpdateUserRoleAsync(string userId, string newRole);
        Task<(string Id, string Email, string Name, string? PhoneNumber)?> GetUserBasicInfoAsync(string userId);
        Task<IdentityResultDto> LockUserAsync(string userId, DateTimeOffset until);
        Task<IdentityResultDto> UnlockUserAsync(string userId);
    }
}
