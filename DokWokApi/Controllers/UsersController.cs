using DokWokApi.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using DokWokApi.BLL.Models.User;
using Microsoft.AspNetCore.Authorization;
using DokWokApi.Extensions;
using DokWokApi.Infrastructure;
using DokWokApi.DAL.Exceptions;
using DokWokApi.BLL.Extensions;
using DokWokApi.BLL.Models;
using DokWokApi.BLL.Infrastructure;
using System.Security.Claims;

namespace DokWokApi.Controllers;

[ApiController]
[Route(ApiRoutes.Users.Group)]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
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
    public async Task<IActionResult> GetUserByUserName(string userName)
    {
        var result = await AuthenticateAndGetUserByUserName(userName);
        return result;
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [HttpGet(ApiRoutes.Users.GetUserById)]
    public async Task<IActionResult> GetUserById(string id)
    {
        var result = await AuthenticateAndGetUserById(id, false);
        return result;
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [HttpGet(ApiRoutes.Users.GetCustomerById)]
    public async Task<IActionResult> GetCustomerById(string id)
    {
        var result = await AuthenticateAndGetUserById(id, true);
        return result;
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPost]
    public async Task<IActionResult> AddUser(UserRegisterModel postModel)
    {
        var model = postModel.ToModel();
        var result = await _userService.AddAsync(model, postModel.Password!);
        return result.ToCreatedAtActionActionResult(nameof(GetCustomerById), "Users");
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [HttpPut]
    public async Task<IActionResult> UpdateUser(UserPutModel putModel)
    {
        var authorizedUser = await AuthenticateUser();
        if (authorizedUser is null)
        {
            return NotFound();
        }

        var isAdmin = IsAuthorizedUserAdmin();
        if (!isAdmin && authorizedUser.Id != putModel.Id)
        {
            return new ForbidResult();
        }

        var model = putModel.ToModel();
        var result = await _userService.UpdateAsync(model);
        return result.ToOkActionResult();
    }

    [Authorize(Roles = $"{UserRoles.Customer}")]
    [HttpPut(ApiRoutes.Users.UpdateCustomerPassword)]
    public async Task<IActionResult> UpdateCustomerPassword(UserPasswordChangeModel model)
    {
        var authorizedUser = await AuthenticateUser();
        if (authorizedUser is null)
        {
            return NotFound();
        }
        else if (authorizedUser.Id != model.UserId)
        {
            return new ForbidResult();
        }

        var result = await _userService.UpdateCustomerPasswordAsync(model);
        return result.ToOkPasswordUpdateActionResult();
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPut(ApiRoutes.Users.UpdateCustomerPasswordAsAdmin)]
    public async Task<IActionResult> UpdateCustomerPasswordAsAdmin(UserPasswordChangeAsAdminModel model)
    {
        var result = await _userService.UpdateCustomerPasswordAsAdminAsync(model);
        return result.ToOkPasswordUpdateActionResult();
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpDelete(ApiRoutes.Users.DeleteUserById)]
    public async Task<IActionResult> DeleteUserById(string id)
    {
        var result = await _userService.DeleteAsync(id);
        if (result is null)
        {
            return NotFound();
        }
        else if (result.Value)
        {
            return Ok();
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPost(ApiRoutes.Users.LoginCustomer)]
    public async Task<IActionResult> LoginCustomer(UserLoginModel loginModel)
    {
        var result = await _userService.CustomerLoginAsync(loginModel);
        return result.ToOkActionResult(HttpContext);
    }

    [HttpPost(ApiRoutes.Users.LoginAdmin)]
    public async Task<IActionResult> LoginAdmin(UserLoginModel loginModel)
    {
        var result = await _userService.AdminLoginAsync(loginModel);
        return result.ToOkActionResult(HttpContext);
    }

    [HttpPost(ApiRoutes.Users.RegisterUser)]
    public async Task<IActionResult> RegisterUser(UserRegisterModel registerModel)
    {
        var result = await _userService.RegisterAsync(registerModel);
        return result.ToCreatedAtActionActionResult(HttpContext, nameof(GetCustomerById), "Users");
    }

    [HttpPost(ApiRoutes.Users.RefreshToken)]
    public async Task<IActionResult> RefreshToken(SecurityTokenModel model)
    {
        var refreshToken = HttpContext.Request.Cookies["RefreshToken"];
        if (refreshToken is null)
        {
            return BadRequest(new ErrorResultModel { Error = "There was no refresh token provided" });
        }

        RefreshTokenModel refreshTokenModel = new()
        {
            Token = model.Token,
            RefreshToken = refreshToken
        };
        var result = await _userService.RefreshTokenAsync(refreshTokenModel);
        return result.ToOkActionResult(HttpContext);
    }

    [Authorize(Roles = $"{UserRoles.Customer},{UserRoles.Admin}")]
    [HttpGet(ApiRoutes.Users.IsCustomerTokenValid)]
    public async Task<IActionResult> IsCustomerTokenValid()
    {
        return await ValidateToken(HttpContext);
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpGet(ApiRoutes.Users.IsAdminTokenValid)]
    public async Task<IActionResult> IsAdminTokenValid()
    {
        return await ValidateToken(HttpContext);
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpGet(ApiRoutes.Users.GetUserRoles)]
    public async Task<IActionResult> GetUserRoles(string userId)
    {
        var result = await _userService.GetUserRolesAsync(userId);
        return result.Match<IActionResult>(roles => new OkObjectResult(roles), e =>
        {
            if (e is NotFoundException)
            {
                return new NotFoundResult();
            }

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        });
    }

    [HttpGet(ApiRoutes.Users.IsCustomerUserNameTaken)]
    public async Task<IActionResult> IsUserNameTaken(string userName)
    {
        var result = await _userService.IsUserNameTaken(userName);
        return result.ToOkIsTakenActionResult();
    }

    [HttpGet(ApiRoutes.Users.IsCustomerEmailTaken)]
    public async Task<IActionResult> IsEmailTaken(string email)
    {
        var result = await _userService.IsEmailTaken(email);
        return result.ToOkIsTakenActionResult();
    }

    [HttpGet(ApiRoutes.Users.IsCustomerPhoneNumberTaken)]
    public async Task<IActionResult> IsPhoneNumberTaken(string phoneNumber)
    {
        var result = await _userService.IsPhoneNumberTaken(phoneNumber);
        return result.ToOkIsTakenActionResult();
    }

    private async Task<UserModel?> AuthenticateUser()
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        if (userId is null)
        {
            return null;
        }

        var authorizedUser = await _userService.GetByIdAsync(userId);
        if (authorizedUser is null)
        {
            return null;
        }

        return authorizedUser;
    }

    private bool IsAuthorizedUserAdmin()
    {
        var roles = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
        var isAdmin = roles.Contains(UserRoles.Admin);
        return isAdmin;
    }

    private async Task<IActionResult> AuthenticateAndGetUserById(string id, bool isCustomer)
    {
        var authorizedUser = await AuthenticateUser();
        if (authorizedUser is null)
        {
            return NotFound();
        }

        var isAdmin = IsAuthorizedUserAdmin();
        if (!isAdmin && authorizedUser.Id != id)
        {
            return new ForbidResult();
        }

        var user = isCustomer ? await _userService.GetCustomerByIdAsync(id) : await _userService.GetByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    private async Task<IActionResult> AuthenticateAndGetUserByUserName(string userName)
    {
        var authorizedUser = await AuthenticateUser();
        if (authorizedUser is null)
        {
            return NotFound();
        }

        var isAdmin = IsAuthorizedUserAdmin();
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

    private async Task<IActionResult> ValidateToken(HttpContext context)
    {
        var userId = context.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        if (userId is null)
        {
            return Unauthorized();
        }

        var authorizedUser = await _userService.GetByIdAsync(userId);
        if (authorizedUser is null)
        {
            return Unauthorized();
        }

        return Ok(authorizedUser);
    }
}
