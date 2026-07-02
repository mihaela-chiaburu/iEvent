using iEvent.Application.DTOs.Admin;
using iEvent.Application.DTOs.Common;
using iEvent.Application.DTOs.User;
using iEvent.Application.Exceptions;
using iEvent.Application.Interfaces.Repositories;
using iEvent.Application.Interfaces.Services;
using iEvent.Domain.Enums;

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

        public async Task<PagedResultDto<UserRespDto>> GetPaginatedUsersAsync(UserFilterDto filter)
        {
            var filterByStatus = filter.FilterByStatus;

            bool? isActive = null;
            if (!string.IsNullOrWhiteSpace(filterByStatus))
            {
                if (filterByStatus.Equals("active", StringComparison.OrdinalIgnoreCase)) isActive = true;
                else if (filterByStatus.Equals("blocked", StringComparison.OrdinalIgnoreCase)) isActive = false;
            }

            var filterDto = new UserFilterDto
            {
                Search = filter.Search,
                Role = filter.Role,
                IsActive = isActive,
                Page = filter.Page < 1 ? 1 : filter.Page,
                PageSize = filter.PageSize < 1 ? 10 : filter.PageSize
            };

            return await _userRepository.GetUsersAsync(filterDto);
        }

        public async Task CreateManagerAsync(AdminUserCreateDto request)
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
                throw new ValidationException("Invalid manager role.");
            }

            var exists = await _userRepository.ExistsByEmailAsync(request.Email);
            if (exists)
            {
                throw new ConflictException("User already exists.");
            }

            var result = await _userRepository.CreateUserWithRoleAsync(request.Email, request.Password,
                roleString, request.PhoneNumber
            );

            if (!result.Succeeded)
            {
                throw new ValidationException("Manager creation failed.", result.Errors ?? Array.Empty<string>());
            }

            await _userProfileService.CreateAdminProfileAsync(
                result.CreatedUserId!,
                request.Email,
                request.Name,
                request.PhoneNumber
            );

        }

        public async Task UpdateUserRoleAsync(string userId, string newRole)
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
                throw new ValidationException("Invalid role.");
            }

            var userInfo = await _userRepository.GetUserBasicInfoAsync(userId);
            if (userInfo == null)
            {
                throw new NotFoundException("User not found.");
            }

            var result = await _userRepository.UpdateUserRoleAsync(userId, newRole);
            if (!result.Succeeded)
            {
                throw new ValidationException("Role update failed.", result.Errors ?? Array.Empty<string>());
            }

            await _userProfileService.SyncProfileAfterRoleChangeAsync(
                userInfo.Value.Id,
                userInfo.Value.Email,
                userInfo.Value.Name,
                userInfo.Value.PhoneNumber,
                newRole
            );

        }

        public async Task LockUserAsync(string userId)
        {
            var lockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
            var result = await _userRepository.LockUserAsync(userId, lockoutEnd);
            if (!result.Succeeded)
            {
                if (result.Errors?.Contains("User not found.") == true)
                {
                    throw new NotFoundException("User not found.");
                }

                throw new ValidationException("User lock failed.", result.Errors ?? Array.Empty<string>());
            }
        }

        public async Task UnlockUserAsync(string userId)
        {
            var result = await _userRepository.UnlockUserAsync(userId);
            if (!result.Succeeded)
            {
                if (result.Errors?.Contains("User not found.") == true)
                {
                    throw new NotFoundException("User not found.");
                }

                throw new ValidationException("User unlock failed.", result.Errors ?? Array.Empty<string>());
            }
        }
    }
}
