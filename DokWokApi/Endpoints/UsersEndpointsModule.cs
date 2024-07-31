using Application.Mapping.Extensions;
using Application.Operations;
using Application.Operations.User;
using Application.Operations.User.Commands.AddUser;
using Application.Operations.User.Commands.DeleteUser;
using Application.Operations.User.Commands.LoginAdmin;
using Application.Operations.User.Commands.LoginCustomer;
using Application.Operations.User.Commands.RefreshToken;
using Application.Operations.User.Commands.RegisterUser;
using Application.Operations.User.Commands.UpdatePassword;
using Application.Operations.User.Commands.UpdatePasswordAsAdmin;
using Application.Operations.User.Commands.UpdateUser;
using Application.Operations.User.Queries.GetAllCustomers;
using Application.Operations.User.Queries.GetAllUsers;
using Application.Operations.User.Queries.GetCustomerById;
using Application.Operations.User.Queries.GetUserById;
using Application.Operations.User.Queries.GetUserByUserName;
using Application.Operations.User.Queries.GetUserRoles;
using Application.Operations.User.Queries.IsUserEmailTaken;
using Application.Operations.User.Queries.IsUserNameTaken;
using Application.Operations.User.Queries.IsUserPhoneNumberTaken;
using Carter;
using DokWokApi.Extensions;
using DokWokApi.Helpers;
using Domain.Errors.Base;
using Domain.Helpers;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

namespace DokWokApi.Endpoints;

public class UsersEndpointsModule : ICarterModule
{
    private const string GetByIdRouteName = nameof(GetCustomerById);

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ApiRoutes.Users.Group).WithTags("Users");

        group.MapGet("/", GetAllUsers)
            .RequireAuthorization(AuthorizationPolicyNames.Admin);

        group.MapGet(ApiRoutes.Users.GetAllCustomers, GetAllCustomers)
            .RequireAuthorization(AuthorizationPolicyNames.Admin);

        group.MapGet(ApiRoutes.Users.GetUserByUserName, GetUserByUserName)
            .RequireAuthorization(AuthorizationPolicyNames.AdminAndCustomer)
            .Produces<UserResponse>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet(ApiRoutes.Users.GetUserById, GetUserById)
            .RequireAuthorization(AuthorizationPolicyNames.AdminAndCustomer)
            .Produces<UserResponse>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet(ApiRoutes.Users.GetCustomerById, GetCustomerById)
            .WithName(GetByIdRouteName)
            .RequireAuthorization(AuthorizationPolicyNames.AdminAndCustomer)
            .Produces<UserResponse>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", AddUser)
            .RequireAuthorization(AuthorizationPolicyNames.Admin)
            .Produces<UserResponse>(StatusCodes.Status201Created)
            .Produces<ProblemDetailsModel>(StatusCodes.Status400BadRequest);

        group.MapPut("/", UpdateUser)
            .RequireAuthorization(AuthorizationPolicyNames.AdminAndCustomer)
            .Produces<UserResponse>()
            .Produces<ProblemDetailsModel>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetailsModel>(StatusCodes.Status400BadRequest);

        group.MapPut(ApiRoutes.Users.UpdateCustomerPassword, UpdateCustomerPassword)
            .RequireAuthorization(AuthorizationPolicyNames.Customer)
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetailsModel>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetailsModel>(StatusCodes.Status400BadRequest);

        group.MapPut(ApiRoutes.Users.UpdateCustomerPasswordAsAdmin, UpdateCustomerPasswordAsAdmin)
            .RequireAuthorization(AuthorizationPolicyNames.Admin)
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetailsModel>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetailsModel>(StatusCodes.Status400BadRequest);

        group.MapDelete(ApiRoutes.Users.DeleteUserById, DeleteUserById)
            .RequireAuthorization(AuthorizationPolicyNames.Admin)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost(ApiRoutes.Users.LoginCustomer, LoginCustomer)
            .Produces<AuthorizedUserResponse>()
            .Produces<ProblemDetailsModel>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetailsModel>(StatusCodes.Status400BadRequest);

        group.MapPost(ApiRoutes.Users.LoginAdmin, LoginAdmin)
            .Produces<AuthorizedUserResponse>()
            .Produces<ProblemDetailsModel>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetailsModel>(StatusCodes.Status400BadRequest);

        group.MapPost(ApiRoutes.Users.RegisterUser, RegisterUser)
            .Produces<AuthorizedUserResponse>(StatusCodes.Status201Created)
            .Produces<ProblemDetailsModel>(StatusCodes.Status400BadRequest);

        group.MapPost(ApiRoutes.Users.RefreshToken, RefreshToken)
            .Produces<AuthorizedUserResponse>()
            .Produces<ProblemDetailsModel>(StatusCodes.Status400BadRequest);

        group.MapGet(ApiRoutes.Users.IsCustomerTokenValid, IsCustomerTokenValid)
            .RequireAuthorization(AuthorizationPolicyNames.AdminAndCustomer)
            .Produces<UserResponse>()
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet(ApiRoutes.Users.IsAdminTokenValid, IsAdminTokenValid)
            .RequireAuthorization(AuthorizationPolicyNames.Admin)
            .Produces<UserResponse>()
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet(ApiRoutes.Users.GetUserRoles, GetUserRoles)
            .RequireAuthorization(AuthorizationPolicyNames.Admin)
            .Produces<IEnumerable<string>>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet(ApiRoutes.Users.IsCustomerUserNameTaken, IsUserNameTaken)
            .Produces<IsTakenResponse>()
            .Produces<ProblemDetailsModel>(StatusCodes.Status400BadRequest);

        group.MapGet(ApiRoutes.Users.IsCustomerEmailTaken, IsEmailTaken)
            .Produces<IsTakenResponse>()
            .Produces<ProblemDetailsModel>(StatusCodes.Status400BadRequest);

        group.MapGet(ApiRoutes.Users.IsCustomerPhoneNumberTaken, IsPhoneNumberTaken)
            .Produces<IsTakenResponse>()
            .Produces<ProblemDetailsModel>(StatusCodes.Status400BadRequest);
    }

    public static async Task<Ok<IEnumerable<UserResponse>>> GetAllUsers(ISender sender)
    {
        var users = await sender.Send(new GetAllUsersQuery());
        return TypedResults.Ok(users);
    }

    public static async Task<Ok<IEnumerable<UserResponse>>> GetAllCustomers(ISender sender)
    {
        var customers = await sender.Send(new GetAllCustomersQuery());
        return TypedResults.Ok(customers);
    }

    public static async Task<IResult> GetUserByUserName(ISender sender, HttpContext httpContext, string userName)
    {
        var result = await AuthenticateAndGetUserByUserName(sender, httpContext, userName);
        return result;
    }

    public static async Task<IResult> GetUserById(ISender sender, HttpContext httpContext, string id)
    {
        var result = await AuthenticateAndGetUserById(sender, httpContext, id, false);
        return result;
    }

    public static async Task<IResult> GetCustomerById(ISender sender, HttpContext httpContext, string id)
    {
        var result = await AuthenticateAndGetUserById(sender, httpContext, id, true);
        return result;
    }

    public static async Task<IResult> AddUser(ISender sender, AddUserRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToCreatedAtRouteResult<UserResponse, string>(GetByIdRouteName);
    }

    public static async Task<IResult> UpdateUser(ISender sender, HttpContext httpContext, UpdateUserRequest request)
    {
        var authorizedUser = await AuthenticateUser(sender, httpContext);
        if (authorizedUser is null)
        {
            return Results.NotFound();
        }

        var isAdmin = IsAuthorizedUserAdmin(httpContext);
        if (!isAdmin && authorizedUser.Id != request.Id)
        {
            return Results.Forbid();
        }

        var result = await sender.Send(request.ToCommand());
        return result.ToOkResult();
    }

    public static async Task<IResult> UpdateCustomerPassword(ISender sender, HttpContext httpContext, UpdatePasswordRequest request)
    {
        var authorizedUser = await AuthenticateUser(sender, httpContext);
        if (authorizedUser is null)
        {
            return Results.NotFound();
        }
        else if (authorizedUser.Id != request.UserId)
        {
            return Results.Forbid();
        }

        var command = new UpdatePasswordCommand(request.UserId, request.OldPassword, request.NewPassword);
        var result = await sender.Send(command);
        return result.ToOkPasswordUpdateResult();
    }

    public static async Task<IResult> UpdateCustomerPasswordAsAdmin(ISender sender, UpdatePasswordAsAdminRequest request)
    {
        var command = new UpdatePasswordAsAdminCommand(request.UserId, request.NewPassword);
        var result = await sender.Send(command);
        return result.ToOkPasswordUpdateResult();
    }

    public static async Task<IResult> DeleteUserById(ISender sender, string id)
    {
        var result = await sender.Send(new DeleteUserCommand(id));
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

    public static async Task<IResult> LoginCustomer(ISender sender, HttpContext httpContext, LoginCustomerRequest request)
    {
        var command = new LoginCustomerCommand(request.UserName, request.Password);
        var result = await sender.Send(command);
        return result.ToOkResult(httpContext);
    }

    public static async Task<IResult> LoginAdmin(ISender sender, HttpContext httpContext, LoginAdminRequest request)
    {
        var command = new LoginAdminCommand(request.UserName, request.Password);
        var result = await sender.Send(command);
        return result.ToOkResult(httpContext);
    }

    public static async Task<IResult> RegisterUser(ISender sender, HttpContext httpContext, RegisterUserRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToCreatedAtRouteResult(httpContext, GetByIdRouteName);
    }

    public static async Task<IResult> RefreshToken(ISender sender, HttpContext httpContext, RefreshTokenRequest request)
    {
        var refreshToken = httpContext.Request.Cookies["RefreshToken"];
        if (refreshToken is null)
        {
            return Results.BadRequest(new ProblemDetailsModel
            {
                Errors = ["There was no refresh token provided"],
                StatusCode = StatusCodes.Status400BadRequest,
                Title = "Bad Request"
            });
        }

        var result = await sender.Send(new RefreshTokenCommand(request.Token, refreshToken));
        return result.ToOkResult(httpContext);
    }

    public static async Task<IResult> IsCustomerTokenValid(ISender sender, HttpContext httpContext)
    {
        return await ValidateToken(sender, httpContext);
    }

    public static async Task<IResult> IsAdminTokenValid(ISender sender, HttpContext httpContext)
    {
        return await ValidateToken(sender, httpContext);
    }

    public static async Task<IResult> GetUserRoles(ISender sender, string userId)
    {
        var result = await sender.Send(new GetUserRolesQuery(userId));
        return result.Match(roles => Results.Ok(roles), error =>
        {
            if (error is NotFoundError)
            {
                return Results.NotFound();
            }

            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        });
    }

    public static async Task<IResult> IsUserNameTaken(ISender sender, string userName)
    {
        var result = await sender.Send(new IsUserNameTakenQuery(userName));
        return result.ToOkIsTakenResult();
    }

    public static async Task<IResult> IsEmailTaken(ISender sender, string email)
    {
        var result = await sender.Send(new IsUserEmailTakenQuery(email));
        return result.ToOkIsTakenResult();
    }

    public static async Task<IResult> IsPhoneNumberTaken(ISender sender, string phoneNumber)
    {
        var result = await sender.Send(new IsUserPhoneNumberTakenQuery(phoneNumber));
        return result.ToOkIsTakenResult();
    }

    private static async Task<UserResponse?> AuthenticateUser(ISender sender, HttpContext httpContext)
    {
        var userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        if (userId is null)
        {
            return null;
        }

        var authorizedUser = await sender.Send(new GetUserByIdQuery(userId));
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

    private static async Task<IResult> AuthenticateAndGetUserById(ISender sender, HttpContext httpContext, string id, bool isCustomer)
    {
        var authorizedUser = await AuthenticateUser(sender, httpContext);
        if (authorizedUser is null)
        {
            return Results.NotFound();
        }

        var isAdmin = IsAuthorizedUserAdmin(httpContext);
        if (!isAdmin && authorizedUser.Id != id)
        {
            return Results.Forbid();
        }

        var user = await sender.Send(isCustomer ? new GetCustomerByIdQuery(id) : new GetUserByIdQuery(id));
        if (user is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(user);
    }

    private static async Task<IResult> AuthenticateAndGetUserByUserName(ISender sender, HttpContext httpContext, string userName)
    {
        var authorizedUser = await AuthenticateUser(sender, httpContext);
        if (authorizedUser is null)
        {
            return Results.NotFound();
        }

        var isAdmin = IsAuthorizedUserAdmin(httpContext);
        if (!isAdmin && authorizedUser.UserName != userName)
        {
            return Results.Forbid();
        }

        var user = await sender.Send(new GetUserByUserNameQuery(userName));
        if (user is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(user);
    }

    private static async Task<IResult> ValidateToken(ISender sender, HttpContext context)
    {
        var userId = context.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        if (userId is null)
        {
            return Results.Unauthorized();
        }

        var authorizedUser = await sender.Send(new GetUserByIdQuery(userId));
        if (authorizedUser is null)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(authorizedUser);
    }
}
