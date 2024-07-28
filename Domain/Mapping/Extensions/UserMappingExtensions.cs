using Domain.Entities;
using Domain.Models.User;

namespace Domain.Mapping.Extensions;

public static class UserMappingExtensions
{
    public static UserModel ToModel(this ApplicationUser entity)
    {
        return new()
        {
            Id = entity.Id,
            Email = entity.Email,
            FirstName = entity.FirstName,
            PhoneNumber = entity.PhoneNumber,
            UserName = entity.UserName
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
            RefreshToken = refreshToken
        };
    }

    public static AuthorizedUserModel ToAuthorizedModel(this ApplicationUser entity, string token, RefreshToken refreshToken)
    {
        return new()
        {
            Email = entity.Email,
            FirstName = entity.FirstName,
            PhoneNumber = entity.PhoneNumber,
            Id = entity.Id,
            UserName = entity.UserName,
            Token = token,
            RefreshToken = refreshToken
        };
    }

    public static ApplicationUser ToEntity(this UserModel model)
    {
        return new()
        {
            Id = model.Id,
            Email = model.Email,
            FirstName = model.FirstName,
            PhoneNumber = model.PhoneNumber,
            UserName = model.UserName
        };
    }
}
