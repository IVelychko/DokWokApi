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

        RuleFor(x => x.PhoneNumber)
            .MustAsync(IsPhoneNumberNotTaken)
            .WithMessage("The phone number is already taken");

        RuleFor(x => x.UserName)
            .MustAsync(IsUserNameNotTaken)
            .WithMessage("The user name is already taken");

        RuleFor(x => x.Email)
            .MustAsync(IsEmailNotTaken)
            .WithMessage("The email is already taken");


    }

    private async Task<bool> IsPhoneNumberNotTaken(string phoneNumber, CancellationToken cancellationToken)
    {
        return !await _userManager.Users.AsNoTracking().AnyAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
    }

    private async Task<bool> IsUserNameNotTaken(string userName, CancellationToken cancellationToken)
    {
        return !await _userManager.Users.AsNoTracking().AnyAsync(u => u.UserName == userName, cancellationToken);
    }

    private async Task<bool> IsEmailNotTaken(string email, CancellationToken cancellationToken)
    {
        return !await _userManager.Users.AsNoTracking().AnyAsync(u => u.Email == email, cancellationToken);
    }
}
