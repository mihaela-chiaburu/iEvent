using iEvent.Application.DTOs;
using iEvent.Application.DTOs.Admin;
using iEvent.Application.Interfaces.Repositories;
using iEvent.Application.Interfaces.Services;
using iEvent.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserProfileService _userProfileService;

        public UserService(
            IUserRepository userRepository,
            IUserProfileService userProfileService)
        {
            _userRepository = userRepository;
            _userProfileService = userProfileService;
        }

        public async Task<PagedResult<UserRespDto>> GetPaginatedUsersAsync(string? search, string? filterByRole, string? filterByStatus, int page, int pageSize)
        {
            bool? isActive = null;
            if (!string.IsNullOrWhiteSpace(filterByStatus))
            {
                if (filterByStatus.Equals("active", StringComparison.OrdinalIgnoreCase)) isActive = true;
                else if (filterByStatus.Equals("blocked", StringComparison.OrdinalIgnoreCase)) isActive = false;
            }

            var filterDto = new UserFilterDto
            {
                Search = search,
                Role = filterByRole,
                IsActive = isActive,
                Page = page < 1 ? 1 : page,
                PageSize = pageSize < 1 ? 10 : pageSize
            };

            return await _userRepository.GetUsersAsync(filterDto);
        }

        public async Task<IdentityResultDto> CreateManagerAsync(AdminUserCreateDto request)
        {
            var validManagerRoles = new HashSet<string>
    {
        RoleNames.EventManager,
        RoleNames.BookingManager,
        RoleNames.SuperAdmin
    };

            var roleString = request.Role.ToString();

            if (!validManagerRoles.Contains(roleString))
            {
                return new IdentityResultDto(false, new[] { "Invalid manager role." });
            }

            var exists = await _userRepository.ExistsByEmailAsync(request.Email);
            if (exists)
            {
                return new IdentityResultDto(false, new[] { "User already exists." });
            }

            var result = await _userRepository.CreateUserWithRoleAsync(request.Email, request.Password,
                roleString, request.PhoneNumber
            );

            if (!result.Succeeded)
            {
                return result;
            }

            await _userProfileService.CreateAdminProfileAsync(
                result.CreatedUserId!,
                request.Email,
                request.Name,
                request.PhoneNumber
            );

            return new IdentityResultDto(true);
        }

        public async Task<IdentityResultDto> UpdateUserRoleAsync(string userId, string newRole)
        {
            var validRoles = new HashSet<string>
            {
                RoleNames.SuperAdmin,
                RoleNames.EventManager,
                RoleNames.BookingManager,
                RoleNames.Customer
            };

            if (!validRoles.Contains(newRole))
            {
                return new IdentityResultDto(false, new[] { "Invalid role." });
            }

            var userInfo = await _userRepository.GetUserBasicInfoAsync(userId);
            if (userInfo == null)
            {
                return new IdentityResultDto(false, new[] { "User not found." });
            }

            var result = await _userRepository.UpdateUserRoleAsync(userId, newRole);
            if (!result.Succeeded)
            {
                return result;
            }

            await _userProfileService.SyncProfileAfterRoleChangeAsync(
                userInfo.Value.Id,
                userInfo.Value.Email,
                userInfo.Value.Name,
                userInfo.Value.PhoneNumber,
                newRole
            );

            return new IdentityResultDto(true);
        }

        public async Task<IdentityResultDto> LockUserAsync(string userId)
        {
            var lockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
            return await _userRepository.LockUserAsync(userId, lockoutEnd);
        }

        public async Task<IdentityResultDto> UnlockUserAsync(string userId)
        {
            return await _userRepository.UnlockUserAsync(userId);
        }
    }
}
