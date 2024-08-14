using Domain.Entities;
using Infrastructure.Validation.Users.Add;
using Infrastructure.Validation.Users.Update;

namespace Infrastructure.Mapping.Extensions;

public static class UserMappingExtensions
{
    public static AddUserValidationModel ToAddValidationModel(this ApplicationUser user) =>
        new(user.FirstName!, user.UserName!, user.Email!, user.PhoneNumber!);

    public static UpdateUserValidationModel ToUpdateValidationModel(this ApplicationUser user) =>
        new(user.Id, user.FirstName!, user.UserName!, user.Email!, user.PhoneNumber!);
}
