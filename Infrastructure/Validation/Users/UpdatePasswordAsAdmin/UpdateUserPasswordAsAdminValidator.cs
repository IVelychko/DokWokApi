using Domain.Entities;
using Domain.Helpers;
using Domain.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Validation.Users.UpdatePasswordAsAdmin;

public sealed class UpdateUserPasswordAsAdminValidator : AbstractValidator<UpdateUserPasswordAsAdminValidationModel>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdateUserPasswordAsAdminValidator(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;

        RuleFor(x => x)
            .NotNull()
            .WithMessage("The passed update password model is null")
            .MustAsync(UserToUpdateExists)
            .WithState(_ => new ValidationFailureState { IsNotFound = true })
            .WithMessage("There is no user with this ID in the database")
            .MustAsync(IsAdmin)
            .WithMessage("Forbidden action")
            .DependentRules(() =>
            {
                RuleFor(x => x.NewPassword)
                    .NotEmpty()
                    .WithMessage("The passed new password is null or empty");
            });
    }

    private async Task<bool> UserToUpdateExists(UpdateUserPasswordAsAdminValidationModel updateUserPassword, CancellationToken cancellationToken)
    {
        return updateUserPassword.UserId is not null
            && await _userManager.Users.AsNoTracking().AnyAsync(x => x.Id == updateUserPassword.UserId, cancellationToken);
    }

    private async Task<bool> IsAdmin(UpdateUserPasswordAsAdminValidationModel updateUserPassword, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(updateUserPassword.UserId);
        if (user is null)
        {
            return false;
        }

        return await _userManager.IsInRoleAsync(user, UserRoles.Admin);
    }
}
