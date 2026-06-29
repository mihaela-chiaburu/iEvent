using iEvent.Application.DTOs;
using iEvent.Application.Interfaces.Repositories;
using iEvent.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace iEvent.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<PagedResult<UserRespDto>> GetUsersAsync(UserFilterDto filter)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(u =>
                    (u.Email != null && u.Email.Contains(filter.Search)) ||
                    (u.UserName != null && u.UserName.Contains(filter.Search)));
            }

            if (filter.IsActive.HasValue)
            {
                var now = DateTimeOffset.UtcNow;
                query = filter.IsActive.Value
                    ? query.Where(u => u.LockoutEnd == null || u.LockoutEnd <= now)
                    : query.Where(u => u.LockoutEnd != null && u.LockoutEnd > now);
            }

            if (!string.IsNullOrWhiteSpace(filter.Role))
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(filter.Role);
                var roleIds = usersInRole.Select(x => x.Id).ToHashSet();
                query = query.Where(u => roleIds.Contains(u.Id));
            }

            var total = await query.CountAsync();

            var users = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var result = new List<UserRespDto>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new UserRespDto(user.Id, user.Email ?? string.Empty, user.UserName ?? string.Empty, user.PhoneNumber, roles));
            }

            return new PagedResult<UserRespDto> { Items = result, TotalCount = total };
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }

        public async Task<IdentityResultDto> CreateUserWithRoleAsync(string email, string password, string role, string? PhoneNumber)
        {
            var user = new ApplicationUser
            {
                Email = email,
                UserName = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return new IdentityResultDto(false, result.Errors.Select(e => e.Description));
            }

            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return new IdentityResultDto(false, roleResult.Errors.Select(e => e.Description));
            }

            return new IdentityResultDto(true) { CreatedUserId = user.Id }; 
        }

        public async Task<IdentityResultDto> UpdateUserRoleAsync(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new IdentityResultDto(false, new[] { "User not found." });
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    return new IdentityResultDto(false, removeResult.Errors.Select(e => e.Description));
                }
            }

            var addResult = await _userManager.AddToRoleAsync(user, newRole);
            if (!addResult.Succeeded)
            {
                return new IdentityResultDto(false, addResult.Errors.Select(e => e.Description));
            }

            return new IdentityResultDto(true);
        }

        public async Task<(string Id, string Email, string Name, string? PhoneNumber)?> GetUserBasicInfoAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            return (
                user.Id,
                user.Email ?? string.Empty,
                user.UserName ?? string.Empty, 
                user.PhoneNumber
            );
        }

        public async Task<IdentityResultDto> LockUserAsync(string userId, DateTimeOffset until)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new IdentityResultDto(false, new[] { "User not found." });
            }

            await _userManager.SetLockoutEnabledAsync(user, true);

            var result = await _userManager.SetLockoutEndDateAsync(user, until);
            if (!result.Succeeded)
            {
                return new IdentityResultDto(false, result.Errors.Select(e => e.Description));
            }

            await _userManager.UpdateSecurityStampAsync(user);

            return new IdentityResultDto(true);
        }

        public async Task<IdentityResultDto> UnlockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new IdentityResultDto(false, new[] { "User not found." });
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, null);
            if (!result.Succeeded)
            {
                return new IdentityResultDto(false, result.Errors.Select(e => e.Description));
            }

            return new IdentityResultDto(true);
        }
    }
}