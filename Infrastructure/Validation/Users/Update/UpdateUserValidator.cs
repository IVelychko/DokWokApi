using Domain.Entities;
using Domain.Validation;
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
            .NotNull()
            .WithMessage("The passed user is null")
            .MustAsync(UserToUpdateExists)
            .WithState(_ => new ValidationFailureState { IsNotFound = true })
            .WithMessage("There is no user with this ID in the database")
            .DependentRules(() =>
            {
                RuleFor(x => x)
                    .MustAsync(IsPhoneNumberNotTaken)
                    .WithMessage("The phone number is already taken");
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

        return !await _userManager.Users.AsNoTracking().AnyAsync(u => u.PhoneNumber == user.PhoneNumber, cancellationToken);
    }
}
