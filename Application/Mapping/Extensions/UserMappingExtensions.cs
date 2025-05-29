using Domain.DTOs.Requests.Users;
using Domain.DTOs.Responses.Users;
using Domain.Entities;

namespace Application.Mapping.Extensions;

public static class UserMappingExtensions
{
    public static User ToEntity(this RegisterUserRequest command)
    {
        return new User
        {
            Email = command.Email,
            FirstName = command.FirstName,
            PhoneNumber = command.PhoneNumber,
            UserName = command.UserName
        };
    }
    
    public static User ToEntity(this AddUserRequest command)
    {
        return new User
        {
            Email = command.Email,
            FirstName = command.FirstName,
            PhoneNumber = command.PhoneNumber,
            UserName = command.UserName
        };
    }
    
    public static User ToEntity(this UpdateUserRequest command)
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
