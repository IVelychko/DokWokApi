using Domain.Entities;
using Domain.Models.User;

namespace Domain.Mapping.Extensions;

public static class UserMappingExtensions
{
    public static UserModel ToModel(this User entity)
    {
        return new()
        {
            Id = entity.Id,
            Email = entity.Email,
            FirstName = entity.FirstName,
            PhoneNumber = entity.PhoneNumber,
            UserName = entity.UserName,
            UserRoleId = entity.UserRoleId,
            UserRole = entity.UserRole is not null ? entity.UserRole.Name : string.Empty
        };
    }

    public static AuthorizedUserModel ToAuthorizedModel(this UserModel model, string token, RefreshToken refreshToken)
    {
        return new()
        {
            Email = model.Email,
            FirstName = model.FirstName,
            PhoneNumber = model.PhoneNumber,
            Id = model.Id,
            UserName = model.UserName,
            Token = token,
            RefreshToken = refreshToken,
            UserRole = model.UserRole,
            UserRoleId = model.UserRoleId
        };
    }

    public static AuthorizedUserModel ToAuthorizedModel(this User entity, string token, RefreshToken refreshToken)
    {
        return new()
        {
            Email = entity.Email,
            FirstName = entity.FirstName,
            PhoneNumber = entity.PhoneNumber,
            Id = entity.Id,
            UserName = entity.UserName,
            Token = token,
            RefreshToken = refreshToken,
            UserRole = entity.UserRole is not null ? entity.UserRole.Name : string.Empty,
            UserRoleId = entity.UserRoleId
        };
    }

    public static User ToEntity(this UserModel model)
    {
        return new()
        {
            Id = model.Id,
            Email = model.Email,
            FirstName = model.FirstName,
            PhoneNumber = model.PhoneNumber,
            UserName = model.UserName,
            UserRoleId = model.UserRoleId
        };
    }
}
