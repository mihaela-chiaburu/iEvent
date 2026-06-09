using iEvent.Application.DTOs;
using iEvent.Application.Interfaces.Services;
using iEvent.Domain.Enums;
using iEvent.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace iEvent.WebApi.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = RoleNames.SuperAdmin)]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserProfileService _userProfileService;

    public UsersController(UserManager<ApplicationUser> userManager, IUserProfileService userProfileService)
    {
        _userManager = userManager;
        _userProfileService = userProfileService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserRespDto>>> GetAll()
    {
        var users = await _userManager.Users.ToListAsync();

        var result = new List<UserRespDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            result.Add(new UserRespDto(
                user.Id,
                user.Email ?? string.Empty,
                roles));
        }

        return Ok(result);
    }

    [HttpPost("create-manager")]
    public async Task<IActionResult> CreateManager(CreateManagerRequest request)
    {
        var validManagerRoles = new HashSet<string>
        {
            RoleNames.EventManager,
            RoleNames.BookingManager,
            RoleNames.SuperAdmin
        };

        if (!validManagerRoles.Contains(request.Role))
        {
            return BadRequest(new { message = "Invalid manager role." });
        }

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
            return BadRequest(new
            {
                errors = result.Errors.Select(e => e.Description)
            });
        }

        await _userManager.AddToRoleAsync(user, request.Role);

        await _userProfileService.CreateAdminProfileAsync(
            user.Id,
            user.Email!);

        return Ok(new { message = "Manager created successfully." });
    }

    [HttpPut("{id}/role")]
    public async Task<IActionResult> UpdateRole(
        string id,
        UpdateUserRoleRequest request)
    {
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

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var currentRoles = await _userManager.GetRolesAsync(user);

        if (currentRoles.Any())
        {
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
        }

        await _userManager.AddToRoleAsync(user, request.Role);

        await _userProfileService.SyncProfileAfterRoleChangeAsync(user.Id, user.Email!, request.Role);

        return Ok(new { message = "User role updated successfully." });
    }
}