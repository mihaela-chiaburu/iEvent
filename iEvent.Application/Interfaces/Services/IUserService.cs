using iEvent.Application.DTOs.Admin;
using iEvent.Application.DTOs.Common;
using iEvent.Application.DTOs.User;

namespace iEvent.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<PagedResultDto<UserRespDto>> GetPaginatedUsersAsync(UserFilterDto filter);
        Task CreateManagerAsync(AdminUserCreateDto request);
        Task UpdateUserRoleAsync(string userId, string newRole);
        Task LockUserAsync(string userId);
        Task UnlockUserAsync(string userId);
    }
}
