using iEvent.Application.DTOs;
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
    public async Task<ActionResult<PagedResult<UserRespDto>>> GetAll( [FromQuery] string? search,
        [FromQuery] string? filterByRole,
        [FromQuery] string? filterByStatus,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _userService.GetPaginatedUsersAsync(search, filterByRole, filterByStatus, page, pageSize);
        return Ok(result);
    }

    [HttpPost("create-manager")]
    public async Task<IActionResult> CreateManager([FromBody] AdminUserCreateDto request)
    {
        var result = await _userService.CreateManagerAsync(request);

        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return StatusCode(201, new { message = "Manager created successfully." });
    }

    [HttpPut("{id}/role")]
    public async Task<IActionResult> UpdateRole(string id, [FromBody] UpdateUserRoleRequest request)
    {
        var result = await _userService.UpdateUserRoleAsync(id, request.Role);

        if (!result.Succeeded)
        {
            if (result.Errors != null && result.Errors.Contains("User not found."))
            {
                return NotFound();
            }
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { message = "User role updated successfully." });
    }
}