using Application.Operations.User;
using Application.Operations.User.Commands.AddUser;
using Application.Operations.User.Commands.RegisterUser;
using Application.Operations.User.Commands.UpdateUser;
using Domain.Models.User;

namespace Application.Mapping.Extensions;

public static class UserMappingExtensions
{
    public static AddUserCommand ToCommand(this AddUserRequest request) =>
        new(request.FirstName, request.UserName, request.Email, request.PhoneNumber, request.Password);

    public static RegisterUserCommand ToCommand(this RegisterUserRequest request) =>
        new(request.FirstName, request.UserName, request.Email, request.PhoneNumber, request.Password);

    public static UpdateUserCommand ToCommand(this UpdateUserRequest request) =>
        new(request.Id, request.FirstName, request.UserName, request.Email, request.PhoneNumber, request.UserRoleId);

    public static UserModel ToModel(this RegisterUserCommand command)
    {
        return new()
        {
            Email = command.Email,
            FirstName = command.FirstName,
            PhoneNumber = command.PhoneNumber,
            UserName = command.UserName
        };
    }

    public static UserModel ToModel(this AddUserCommand command)
    {
        return new()
        {
            Email = command.Email,
            FirstName = command.FirstName,
            PhoneNumber = command.PhoneNumber,
            UserName = command.UserName
        };
    }

    public static UserModel ToModel(this UpdateUserCommand command)
    {
        return new()
        {
            Email = command.Email,
            FirstName = command.FirstName,
            PhoneNumber = command.PhoneNumber,
            UserName = command.UserName,
            Id = command.Id,
            UserRoleId = command.UserRoleId
        };
    }

    public static UserResponse ToResponse(this UserModel model)
    {
        return new()
        {
            Id = model.Id,
            FirstName = model.FirstName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            UserName = model.UserName,
            UserRoleId = model.UserRoleId,
            UserRole = model.UserRole
        };
    }

    public static AuthorizedUserResponse ToResponse(this AuthorizedUserModel model)
    {
        return new()
        {
            Id = model.Id,
            Email = model.Email,
            FirstName = model.FirstName,
            PhoneNumber = model.PhoneNumber,
            UserName = model.UserName,
            Token = model.Token,
            RefreshToken = model.RefreshToken,
            UserRoleId = model.UserRoleId,
            UserRole = model.UserRole
        };
    }
}
