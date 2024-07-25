using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.User;
using DokWokApi.Extensions;
using DokWokApi.Infrastructure;
using DokWokApi.DAL.Exceptions;
using DokWokApi.BLL.Extensions;
using DokWokApi.BLL.Models;
using DokWokApi.BLL.Infrastructure;
using System.Security.Claims;

namespace DokWokApi.Endpoints;

public static class UsersEndpoints
{
    private const string GetByIdRouteName = nameof(GetCustomerById);

    public static void MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ApiRoutes.Users.Group);
        group.MapGet("/", GetAllUsers).RequireAuthorization(AuthorizationPolicyNames.Admin);
        group.MapGet(ApiRoutes.Users.GetAllCustomers, GetAllCustomers).RequireAuthorization(AuthorizationPolicyNames.Admin);
        group.MapGet(ApiRoutes.Users.GetUserByUserName, GetUserByUserName).RequireAuthorization(AuthorizationPolicyNames.AdminAndCustomer);
        group.MapGet(ApiRoutes.Users.GetUserById, GetUserById).RequireAuthorization(AuthorizationPolicyNames.AdminAndCustomer);
        group.MapGet(ApiRoutes.Users.GetCustomerById, GetCustomerById)
            .WithName(GetByIdRouteName)
            .RequireAuthorization(AuthorizationPolicyNames.AdminAndCustomer);

        group.MapPost("/", AddUser).RequireAuthorization(AuthorizationPolicyNames.Admin);

        group.MapPut("/", UpdateUser).RequireAuthorization(AuthorizationPolicyNames.AdminAndCustomer);
        group.MapPut(ApiRoutes.Users.UpdateCustomerPassword, UpdateCustomerPassword)
            .RequireAuthorization(AuthorizationPolicyNames.Customer);
        group.MapPut(ApiRoutes.Users.UpdateCustomerPasswordAsAdmin, UpdateCustomerPasswordAsAdmin)
            .RequireAuthorization(AuthorizationPolicyNames.Admin);

        group.MapDelete(ApiRoutes.Users.DeleteUserById, DeleteUserById).RequireAuthorization(AuthorizationPolicyNames.Admin);

        group.MapPost(ApiRoutes.Users.LoginCustomer, LoginCustomer);
        group.MapPost(ApiRoutes.Users.LoginAdmin, LoginAdmin);
        group.MapPost(ApiRoutes.Users.RegisterUser, RegisterUser);
        group.MapPost(ApiRoutes.Users.RefreshToken, RefreshToken);
        group.MapGet(ApiRoutes.Users.IsCustomerTokenValid, IsCustomerTokenValid)
            .RequireAuthorization(AuthorizationPolicyNames.AdminAndCustomer);
        group.MapGet(ApiRoutes.Users.IsAdminTokenValid, IsAdminTokenValid)
            .RequireAuthorization(AuthorizationPolicyNames.Admin);

        group.MapGet(ApiRoutes.Users.GetUserRoles, GetUserRoles)
            .RequireAuthorization(AuthorizationPolicyNames.Admin);

        group.MapGet(ApiRoutes.Users.IsCustomerUserNameTaken, IsUserNameTaken);
        group.MapGet(ApiRoutes.Users.IsCustomerEmailTaken, IsEmailTaken);
        group.MapGet(ApiRoutes.Users.IsCustomerPhoneNumberTaken, IsPhoneNumberTaken);
    }

    public static async Task<IResult> GetAllUsers(IUserService userService)
    {
        var users = await userService.GetAllAsync();
        return Results.Ok(users);
    }

    public static async Task<IResult> GetAllCustomers(IUserService userService)
    {
        var customers = await userService.GetAllCustomersAsync();
        return Results.Ok(customers);
    }

    public static async Task<IResult> GetUserByUserName(IUserService userService, HttpContext httpContext, string userName)
    {
        var result = await AuthenticateAndGetUserByUserName(userService, httpContext, userName);
        return result;
    }

    public static async Task<IResult> GetUserById(IUserService userService, HttpContext httpContext, string id)
    {
        var result = await AuthenticateAndGetUserById(userService, httpContext, id, false);
        return result;
    }

    public static async Task<IResult> GetCustomerById(IUserService userService, HttpContext httpContext, string id)
    {
        var result = await AuthenticateAndGetUserById(userService, httpContext, id, true);
        return result;
    }

    public static async Task<IResult> AddUser(IUserService userService, UserRegisterModel postModel)
    {
        var model = postModel.ToModel();
        var result = await userService.AddAsync(model, postModel.Password!);
        return result.ToCreatedAtRouteResult(GetByIdRouteName);
    }

    public static async Task<IResult> UpdateUser(IUserService userService, HttpContext httpContext, UserPutModel putModel)
    {
        var authorizedUser = await AuthenticateUser(userService, httpContext);
        if (authorizedUser is null)
        {
            return Results.NotFound();
        }

        var isAdmin = IsAuthorizedUserAdmin(httpContext);
        if (!isAdmin && authorizedUser.Id != putModel.Id)
        {
            return Results.Forbid();
        }

        var model = putModel.ToModel();
        var result = await userService.UpdateAsync(model);
        return result.ToOkResult();
    }

    public static async Task<IResult> UpdateCustomerPassword(IUserService userService, HttpContext httpContext, UserPasswordChangeModel model)
    {
        var authorizedUser = await AuthenticateUser(userService, httpContext);
        if (authorizedUser is null)
        {
            return Results.NotFound();
        }
        else if (authorizedUser.Id != model.UserId)
        {
            return Results.Forbid();
        }

        var result = await userService.UpdateCustomerPasswordAsync(model);
        return result.ToOkPasswordUpdateResult();
    }

    public static async Task<IResult> UpdateCustomerPasswordAsAdmin(IUserService userService, UserPasswordChangeAsAdminModel model)
    {
        var result = await userService.UpdateCustomerPasswordAsAdminAsync(model);
        return result.ToOkPasswordUpdateResult();
    }

    public static async Task<IResult> DeleteUserById(IUserService userService, string id)
    {
        var result = await userService.DeleteAsync(id);
        if (result is null)
        {
            return Results.NotFound();
        }
        else if (result.Value)
        {
            return Results.Ok();
        }

        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }

    public static async Task<IResult> LoginCustomer(IUserService userService, HttpContext httpContext, UserLoginModel loginModel)
    {
        var result = await userService.CustomerLoginAsync(loginModel);
        return result.ToOkResult(httpContext);
    }

    public static async Task<IResult> LoginAdmin(IUserService userService, HttpContext httpContext, UserLoginModel loginModel)
    {
        var result = await userService.AdminLoginAsync(loginModel);
        return result.ToOkResult(httpContext);
    }

    public static async Task<IResult> RegisterUser(IUserService userService, HttpContext httpContext, UserRegisterModel registerModel)
    {
        var result = await userService.RegisterAsync(registerModel);
        return result.ToCreatedAtRouteResult(httpContext, GetByIdRouteName);
    }

    public static async Task<IResult> RefreshToken(IUserService userService, HttpContext httpContext, SecurityTokenModel model)
    {
        var refreshToken = httpContext.Request.Cookies["RefreshToken"];
        if (refreshToken is null)
        {
            return Results.BadRequest(new ErrorResultModel { Error = "There was no refresh token provided" });
        }

        RefreshTokenModel refreshTokenModel = new()
        {
            Token = model.Token,
            RefreshToken = refreshToken
        };
        var result = await userService.RefreshTokenAsync(refreshTokenModel);
        return result.ToOkResult(httpContext);
    }

    public static async Task<IResult> IsCustomerTokenValid(IUserService userService, HttpContext httpContext)
    {
        return await ValidateToken(userService, httpContext);
    }

    public static async Task<IResult> IsAdminTokenValid(IUserService userService, HttpContext httpContext)
    {
        return await ValidateToken(userService, httpContext);
    }

    public static async Task<IResult> GetUserRoles(IUserService userService, string userId)
    {
        var result = await userService.GetUserRolesAsync(userId);
        return result.Match(roles => Results.Ok(roles), e =>
        {
            if (e is NotFoundException)
            {
                return Results.NotFound();
            }

            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        });
    }

    public static async Task<IResult> IsUserNameTaken(IUserService userService, string userName)
    {
        var result = await userService.IsUserNameTaken(userName);
        return result.ToOkIsTakenResult();
    }

    public static async Task<IResult> IsEmailTaken(IUserService userService, string email)
    {
        var result = await userService.IsEmailTaken(email);
        return result.ToOkIsTakenResult();
    }

    public static async Task<IResult> IsPhoneNumberTaken(IUserService userService, string phoneNumber)
    {
        var result = await userService.IsPhoneNumberTaken(phoneNumber);
        return result.ToOkIsTakenResult();
    }

    private static async Task<UserModel?> AuthenticateUser(IUserService userService, HttpContext httpContext)
    {
        var userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        if (userId is null)
        {
            return null;
        }

        var authorizedUser = await userService.GetByIdAsync(userId);
        if (authorizedUser is null)
        {
            return null;
        }

        return authorizedUser;
    }

    private static bool IsAuthorizedUserAdmin(HttpContext httpContext)
    {
        var roles = httpContext.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
        var isAdmin = roles.Contains(UserRoles.Admin);
        return isAdmin;
    }

    private static async Task<IResult> AuthenticateAndGetUserById(IUserService userService, HttpContext httpContext, string id, bool isCustomer)
    {
        var authorizedUser = await AuthenticateUser(userService, httpContext);
        if (authorizedUser is null)
        {
            return Results.NotFound();
        }

        var isAdmin = IsAuthorizedUserAdmin(httpContext);
        if (!isAdmin && authorizedUser.Id != id)
        {
            return Results.Forbid();
        }

        var user = isCustomer ? await userService.GetCustomerByIdAsync(id) : await userService.GetByIdAsync(id);
        if (user is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(user);
    }

    private static async Task<IResult> AuthenticateAndGetUserByUserName(IUserService userService, HttpContext httpContext, string userName)
    {
        var authorizedUser = await AuthenticateUser(userService, httpContext);
        if (authorizedUser is null)
        {
            return Results.NotFound();
        }

        var isAdmin = IsAuthorizedUserAdmin(httpContext);
        if (!isAdmin && authorizedUser.UserName != userName)
        {
            return Results.Forbid();
        }

        var user = await userService.GetByUserNameAsync(userName);
        if (user is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(user);
    }

    private static async Task<IResult> ValidateToken(IUserService userService, HttpContext context)
    {
        var userId = context.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        if (userId is null)
        {
            return Results.Unauthorized();
        }

        var authorizedUser = await userService.GetByIdAsync(userId);
        if (authorizedUser is null)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(authorizedUser);
    }
}
