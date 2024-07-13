using AutoMapper;
using DokWokApi.BLL.Interfaces;
using DokWokApi.Exceptions;
using Microsoft.AspNetCore.Mvc;
using DokWokApi.BLL.Models.User;
using DokWokApi.BLL;
using Microsoft.AspNetCore.Authorization;
using DokWokApi.Extensions;
using DokWokApi.Infrastructure;

namespace DokWokApi.Controllers;

[ApiController]
[Route(ApiRoutes.Users.Controller)]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpGet(ApiRoutes.Users.GetAllCustomers)]
    public async Task<IActionResult> GetAllCustomers()
    {
        var customers = await _userService.GetAllCustomersAsync();
        return Ok(customers);
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [HttpGet(ApiRoutes.Users.GetUserByUserName)]
    public async Task<IActionResult> GetUserByUserName(string userName, [FromServices] IUserService userService)
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        if (userId is null)
        {
            return Unauthorized();
        }

        var authorizedUser = await userService.GetByIdAsync(userId);
        if (authorizedUser is null)
        {
            return Unauthorized();
        }

        var rolesResult = await userService.GetUserRolesAsync(authorizedUser.Id!);
        var isAdmin = rolesResult.Match(roles => roles.Contains(UserRoles.Admin),
            e => false);
        if (!isAdmin && authorizedUser.UserName != userName)
        {
            return new ForbidResult();
        }

        var user = await _userService.GetByUserNameAsync(userName);
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [HttpGet(ApiRoutes.Users.GetUserById)]
    public async Task<IActionResult> GetUserById(string id, [FromServices] IUserService userService)
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        if (userId is null)
        {
            return Unauthorized();
        }

        var authorizedUser = await userService.GetByIdAsync(userId);
        if (authorizedUser is null)
        {
            return Unauthorized();
        }

        var rolesResult = await userService.GetUserRolesAsync(authorizedUser.Id!);
        var isAdmin = rolesResult.Match(roles => roles.Contains(UserRoles.Admin),
            e => false);
        if (!isAdmin && authorizedUser.Id != id)
        {
            return new ForbidResult();
        }

        var user = await _userService.GetByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [HttpGet(ApiRoutes.Users.GetCustomerById)]
    public async Task<IActionResult> GetCustomerById(string id, [FromServices] IUserService userService)
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        if (userId is null)
        {
            return Unauthorized();
        }

        var authorizedUser = await userService.GetByIdAsync(userId);
        if (authorizedUser is null)
        {
            return Unauthorized();
        }

        var rolesResult = await userService.GetUserRolesAsync(authorizedUser.Id!);
        var isAdmin = rolesResult.Match(roles => roles.Contains(UserRoles.Admin),
            e => false);
        if (!isAdmin && authorizedUser.Id != id)
        {
            return new ForbidResult();
        }

        var user = await _userService.GetCustomerByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPost]
    public async Task<IActionResult> AddUser(UserRegisterModel postModel, [FromServices] IMapper mapper)
    {
        var model = mapper.Map<UserModel>(postModel);
        var result = await _userService.AddAsync(model, postModel.Password!);
        return result.ToCreatedAtActionUser(_logger, nameof(GetCustomerById), nameof(UsersController));
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [HttpPut]
    public async Task<IActionResult> UpdateUser(
        UserPutModel putModel, [FromServices] IUserService userService, [FromServices] IMapper mapper)
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        if (userId is null)
        {
            return Unauthorized();
        }

        var authorizedUser = await userService.GetByIdAsync(userId);
        if (authorizedUser is null)
        {
            return Unauthorized();
        }

        var rolesResult = await userService.GetUserRolesAsync(authorizedUser.Id!);
        var isAdmin = rolesResult.Match(roles => roles.Contains(UserRoles.Admin),
            e => false);
        if (!isAdmin && authorizedUser.Id != putModel.Id)
        {
            return new ForbidResult();
        }

        var model = mapper.Map<UserModel>(putModel);
        var result = await _userService.UpdateAsync(model);
        return result.ToOk(_logger);
    }

    [Authorize(Roles = $"{UserRoles.Customer}")]
    [HttpPut(ApiRoutes.Users.UpdateCustomerPassword)]
    public async Task<IActionResult> UpdateCustomerPassword(UserPasswordChangeModel model, [FromServices] IUserService userService)
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        if (userId is null)
        {
            return Unauthorized();
        }

        var authorizedUser = await userService.GetByIdAsync(userId);
        if (authorizedUser is null)
        {
            return Unauthorized();
        }
        else if (authorizedUser.Id != model.UserId)
        {
            return new ForbidResult();
        }

        var result = await _userService.UpdateCustomerPasswordAsync(model);
        return result.ToOkPasswordUpdate(_logger);
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPut(ApiRoutes.Users.UpdateCustomerPasswordAsAdmin)]
    public async Task<IActionResult> UpdateCustomerPasswordAsAdmin(UserPasswordChangeAsAdminModel model)
    {
        var result = await _userService.UpdateCustomerPasswordAsAdminAsync(model);
        return result.ToOkPasswordUpdate(_logger);
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpDelete(ApiRoutes.Users.DeleteUserById)]
    public async Task<IActionResult> DeleteUserById(string id)
    {
        var result = await _userService.DeleteAsync(id);
        if (result is null)
        {
            _logger.LogInformation("Not found");
            return NotFound();
        }
        else if (result.Value)
        {
            return Ok();
        }

        _logger.LogError("Server error");
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPost(ApiRoutes.Users.LoginCustomer)]
    public async Task<IActionResult> LoginCustomer(UserLoginModel loginModel)
    {
        var result = await _userService.AuthenticateCustomerLoginAsync(loginModel);
        return result.ToOk(_logger);
    }

    [HttpPost(ApiRoutes.Users.LoginAdmin)]
    public async Task<IActionResult> LoginAdmin(UserLoginModel loginModel)
    {
        var result = await _userService.AuthenticateAdminLoginAsync(loginModel);
        return result.ToOk(_logger);
    }

    [HttpPost(ApiRoutes.Users.RegisterUser)]
    public async Task<IActionResult> RegisterUser(UserRegisterModel registerModel)
    {
        var result = await _userService.AuthenticateRegisterAsync(registerModel);
        return result.ToOk(_logger);
    }

    [Authorize(Roles = $"{UserRoles.Customer},{UserRoles.Admin}")]
    [HttpGet(ApiRoutes.Users.IsCustomerTokenValid)]
    public async Task<IActionResult> IsCustomerTokenValid([FromServices] IUserService userService)
    {
        return await ValidateToken(HttpContext, userService);
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpGet(ApiRoutes.Users.IsAdminTokenValid)]
    public async Task<IActionResult> IsAdminTokenValid([FromServices] IUserService userService)
    {
        return await ValidateToken(HttpContext, userService);
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpGet(ApiRoutes.Users.GetUserRoles)]
    public async Task<IActionResult> GetUserRoles(string userId)
    {
        var result = await _userService.GetUserRolesAsync(userId);
        return result.Match<IActionResult>(roles => new OkObjectResult(roles), e =>
        {
            if (e is EntityNotFoundException notFoundException)
            {
                _logger.LogInformation(notFoundException, "Not found");
                return new NotFoundResult();
            }

            _logger.LogError(e, "Server error");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        });
    }

    [HttpGet(ApiRoutes.Users.IsCustomerUserNameTaken)]
    public async Task<IActionResult> IsUserNameTaken(string userName)
    {
        var result = await _userService.IsUserNameTaken(userName);
        return result.ToOkIsTaken(_logger);
    }

    [HttpGet(ApiRoutes.Users.IsCustomerEmailTaken)]
    public async Task<IActionResult> IsEmailTaken(string email)
    {
        var result = await _userService.IsEmailTaken(email);
        return result.ToOkIsTaken(_logger);
    }

    [HttpGet(ApiRoutes.Users.IsCustomerPhoneNumberTaken)]
    public async Task<IActionResult> IsPhoneNumberTaken(string phoneNumber)
    {
        var result = await _userService.IsPhoneNumberTaken(phoneNumber);
        return result.ToOkIsTaken(_logger);
    }

    private async Task<IActionResult> ValidateToken(HttpContext context, IUserService userService)
    {
        var userId = context.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        if (userId is null)
        {
            return Unauthorized();
        }

        var authorizedUser = await userService.GetByIdAsync(userId);
        if (authorizedUser is null)
        {
            return Unauthorized();
        }

        return Ok(authorizedUser);
    }
}
