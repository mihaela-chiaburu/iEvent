using iEvent.Application.DTOs.Admin;
using iEvent.Application.DTOs.Common;
using iEvent.Application.DTOs.User;
using iEvent.Application.Interfaces.Services;
using iEvent.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iEvent.WebApi.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = RoleNames.SuperAdmin)]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<UserRespDto>>> GetAll([FromQuery] UserFilterDto filter)
    {
        var result = await _userService.GetPaginatedUsersAsync(filter);
        return Ok(result);
    }

    [HttpPost("create-manager")]
    public async Task<IActionResult> CreateManager([FromBody] AdminUserCreateDto request)
    {
        await _userService.CreateManagerAsync(request);

        return StatusCode(201, new { message = "Manager created successfully." });
    }

    [HttpPut("{id}/role")]
    public async Task<IActionResult> UpdateRole(string id, [FromBody] UpdateUserRoleRequest request)
    {
        await _userService.UpdateUserRoleAsync(id, request.Role);

        return Ok(new { message = "User role updated successfully." });
    }

    [HttpPatch("{id}/lock")]
    public async Task<IActionResult> LockUser(string id)
    {
        await _userService.LockUserAsync(id);

        return Ok(new { message = "User has been locked successfully." });
    }

    [HttpPatch("{id}/unlock")]
    public async Task<IActionResult> UnlockUser(string id)
    {
        await _userService.UnlockUserAsync(id);

        return Ok(new { message = "User has been unlocked successfully." });
    }
}