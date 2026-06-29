using iEvent.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<PagedResult<UserRespDto>> GetPaginatedUsersAsync(string? search, string? filterByRole, 
                                                        string? filterByStatus, int page, int pageSize);
        Task<IdentityResultDto> CreateManagerAsync(AdminUserCreateDto request);
        Task<IdentityResultDto> UpdateUserRoleAsync(string userId, string newRole);
        Task<IdentityResultDto> LockUserAsync(string userId);
        Task<IdentityResultDto> UnlockUserAsync(string userId);
    }
}
