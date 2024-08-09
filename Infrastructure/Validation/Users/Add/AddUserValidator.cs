using Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Validation.Users.Add;

public sealed class AddUserValidator : AbstractValidator<AddUserValidationModel>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AddUserValidator(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;

        RuleFor(x => x)
            .NotNull()
            .WithMessage("The passed user is null")
            .MustAsync(IsPhoneNumberNotTaken)
            .WithMessage("The phone number is already taken");
    }

    private async Task<bool> IsPhoneNumberNotTaken(AddUserValidationModel user, CancellationToken cancellationToken)
    {
        return !await _userManager.Users.AsNoTracking().AnyAsync(u => u.PhoneNumber == user.PhoneNumber, cancellationToken);
    }
}
