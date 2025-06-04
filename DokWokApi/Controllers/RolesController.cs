using DokWokApi.Constants;
using DokWokApi.Filters;
using Domain.Abstractions.Services;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DokWokApi.Controllers;

[ApiController]
[Route(ApiRoutes.Roles.Group)]
public class RolesController : ControllerBase
{
    private readonly IUserService _userService;

    public RolesController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet(ApiRoutes.Roles.GetAllUsersByRoleName)]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> GetAllUsersByRoleName(string roleName)
    {
        var users = await _userService.GetAllUsersByRoleNameAsync(roleName);
        return Ok(users);
    }

    [HttpGet(ApiRoutes.Roles.GetUserByRoleNameAndUserId)]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [AuthorizeUserRetrievalById]
    public async Task<IActionResult> GetUserByRoleNameAndUserId(string roleName, long userId)
    {
        var user = await _userService.GetUserByRoleNameAndUserIdAsync(roleName, userId);
        return Ok(user);
    }
}