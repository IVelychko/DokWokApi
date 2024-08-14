using Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Validation.Users.Update;

public sealed class UpdateUserValidator : AbstractValidator<UpdateUserValidationModel>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdateUserValidator(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;

        RuleFor(x => x)
            .MustAsync(UserToUpdateExists)
            .WithName("user")
            .WithErrorCode("404")
            .WithMessage("There is no user with this ID in the database")
            .DependentRules(() =>
            {
                RuleFor(x => x)
                    .MustAsync(IsPhoneNumberNotTaken)
                    .WithName("phoneNumber")
                    .WithMessage("The phone number is already taken");

                RuleFor(x => x)
                    .MustAsync(IsUserNameNotTaken)
                    .WithName("userName")
                    .WithMessage("The user name is already taken");

                RuleFor(x => x)
                    .MustAsync(IsEmailNotTaken)
                    .WithName("email")
                    .WithMessage("The email is already taken");
            });
    }

    private async Task<bool> UserToUpdateExists(UpdateUserValidationModel user, CancellationToken cancellationToken)
    {
        return await _userManager.Users.AsNoTracking().AnyAsync(u => u.Id == user.Id, cancellationToken);
    }

    private async Task<bool> IsPhoneNumberNotTaken(UpdateUserValidationModel user, CancellationToken cancellationToken)
    {
        var existingEntity = await _userManager.FindByIdAsync(user.Id);
        if (existingEntity is null)
        {
            return false;
        }

        if (user.PhoneNumber != existingEntity.PhoneNumber
            && await _userManager.Users.AsNoTracking().AnyAsync(u => u.PhoneNumber == user.PhoneNumber, cancellationToken))
        {
            return false;
        }

        return true;
    }

    private async Task<bool> IsUserNameNotTaken(UpdateUserValidationModel user, CancellationToken cancellationToken)
    {
        var existingEntity = await _userManager.FindByIdAsync(user.Id);
        if (existingEntity is null)
        {
            return false;
        }

        if (user.UserName != existingEntity.UserName
            && await _userManager.Users.AsNoTracking().AnyAsync(u => u.UserName == user.UserName, cancellationToken))
        {
            return false;
        }

        return true;
    }

    private async Task<bool> IsEmailNotTaken(UpdateUserValidationModel user, CancellationToken cancellationToken)
    {
        var existingEntity = await _userManager.FindByIdAsync(user.Id);
        if (existingEntity is null)
        {
            return false;
        }

        if (user.Email != existingEntity.Email
            && await _userManager.Users.AsNoTracking().AnyAsync(u => u.Email == user.Email, cancellationToken))
        {
            return false;
        }

        return true;
    }
}
