using iEvent.Application.Interfaces.Services;
using iEvent.Domain.Enums;
using iEvent.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iEvent.WebApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserProfileService _userProfileService;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(UserManager<ApplicationUser> userManager, IJwtTokenService jwtTokenService,
            IUserProfileService userProfileService)
        {
            _userManager = userManager;
            _userProfileService = userProfileService;
            _jwtTokenService = jwtTokenService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var existing = await _userManager.FindByEmailAsync(request.Email);
            if (existing != null)
            {
                return Conflict(new { message = "User already exists." });
            }

            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }

            await _userManager.AddToRoleAsync(user, RoleNames.Customer);

            await _userProfileService.CreateCustomerProfileAsync(user.Id, user.Email!);

            return Ok(new { message = "Registered successfully." });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Unauthorized();
            }

            var valid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!valid)
            {
                return Unauthorized();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenService.GenerateToken(user.Id, user.Email ?? string.Empty, roles);

            return Ok(new { token, roles });
        }

        [Authorize(Roles = RoleNames.SuperAdmin)]
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole(AssignRoleRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return NotFound();
            }

            var validRoles = new HashSet<string>
            {
                RoleNames.SuperAdmin,
                RoleNames.EventManager,
                RoleNames.BookingManager,
                RoleNames.Customer
            };

            if (!validRoles.Contains(request.Role))
            {
                return BadRequest(new { message = "Invalid role." });
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            await _userManager.AddToRoleAsync(user, request.Role);
            await _userProfileService.SyncProfileAfterRoleChangeAsync(user.Id, user.Email!, request.Role);

            return Ok(new { message = "Role updated." });
        }

        public record RegisterRequest(string Email, string Password);
        public record LoginRequest(string Email, string Password);
        public record AssignRoleRequest(string Email, string Role);
    }
}
