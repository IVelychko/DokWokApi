using Domain.DTOs.Commands.Users;
using Domain.DTOs.Requests.Users;
using Domain.DTOs.Responses.Users;
using Domain.Entities;

namespace Application.Mapping.Extensions;

public static class UserMappingExtensions
{
    public static AddUserCommand ToCommand(this AddUserRequest request) =>
        new(request.FirstName, request.UserName, request.Email, request.PhoneNumber, request.Password);

    public static RegisterUserCommand ToCommand(this RegisterUserRequest request) =>
        new(request.FirstName, request.UserName, request.Email, request.PhoneNumber, request.Password);

    public static UpdateUserCommand ToCommand(this UpdateUserRequest request) =>
        new(request.Id, request.FirstName, request.UserName, request.Email, request.PhoneNumber);

    // public static UserModel ToModel(this RegisterUserCommand command)
    // {
    //     return new()
    //     {
    //         Email = command.Email,
    //         FirstName = command.FirstName,
    //         PhoneNumber = command.PhoneNumber,
    //         UserName = command.UserName
    //     };
    // }
    
    public static User ToEntity(this RegisterUserCommand command)
    {
        return new User
        {
            Email = command.Email,
            FirstName = command.FirstName,
            PhoneNumber = command.PhoneNumber,
            UserName = command.UserName
        };
    }

    // public static UserModel ToModel(this AddUserCommand command)
    // {
    //     return new()
    //     {
    //         Email = command.Email,
    //         FirstName = command.FirstName,
    //         PhoneNumber = command.PhoneNumber,
    //         UserName = command.UserName
    //     };
    // }
    
    public static User ToEntity(this AddUserCommand command)
    {
        return new User
        {
            Email = command.Email,
            FirstName = command.FirstName,
            PhoneNumber = command.PhoneNumber,
            UserName = command.UserName
        };
    }

    // public static UserModel ToModel(this UpdateUserCommand command)
    // {
    //     return new()
    //     {
    //         Email = command.Email,
    //         FirstName = command.FirstName,
    //         PhoneNumber = command.PhoneNumber,
    //         UserName = command.UserName,
    //         Id = command.Id
    //     };
    // }
    
    public static User ToEntity(this UpdateUserCommand command)
    {
        return new User
        {
            Email = command.Email,
            FirstName = command.FirstName,
            PhoneNumber = command.PhoneNumber,
            UserName = command.UserName,
            Id = command.Id
        };
    }

    // public static UserResponse ToResponse(this UserModel model)
    // {
    //     return new()
    //     {
    //         Id = model.Id,
    //         FirstName = model.FirstName,
    //         Email = model.Email,
    //         PhoneNumber = model.PhoneNumber,
    //         UserName = model.UserName,
    //         UserRoleId = model.UserRoleId,
    //         UserRole = model.UserRole
    //     };
    // }
    
    public static UserResponse ToResponse(this User entity)
    {
        return new UserResponse
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            Email = entity.Email,
            PhoneNumber = entity.PhoneNumber,
            UserName = entity.UserName,
            UserRoleId = entity.UserRoleId,
            UserRole = entity.UserRole?.Name ?? string.Empty
        };
    }

    // public static AuthorizedUserResponse ToResponse(this AuthorizedUserModel model)
    // {
    //     return new()
    //     {
    //         Id = model.Id,
    //         Email = model.Email,
    //         FirstName = model.FirstName,
    //         PhoneNumber = model.PhoneNumber,
    //         UserName = model.UserName,
    //         Token = model.Token,
    //         RefreshToken = model.RefreshToken,
    //         UserRoleId = model.UserRoleId,
    //         UserRole = model.UserRole
    //     };
    // }
    
    public static AuthorizedUserResponse ToAuthorizedResponse(this User entity, string securityToken, RefreshToken refreshToken)
    {
        return new AuthorizedUserResponse
        {
            Id = entity.Id,
            Email = entity.Email,
            FirstName = entity.FirstName,
            PhoneNumber = entity.PhoneNumber,
            UserName = entity.UserName,
            Token = securityToken,
            RefreshToken = refreshToken,
            UserRoleId = entity.UserRoleId,
            UserRole = entity.UserRole?.Name ?? string.Empty
        };
    }
}
