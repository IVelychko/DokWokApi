using Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Validation.RefreshTokens.Add;

public sealed class AddRefreshTokenValidator : AbstractValidator<AddRefreshTokenValidationModel>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AddRefreshTokenValidator(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;

        RuleFor(x => x).NotNull().WithMessage("The passed refresh token is null").DependentRules(() =>
        {
            RuleFor(x => x.UserId)
                .MustAsync(async (userId, cancellationToken) =>
                {
                    return await _userManager.Users.AsNoTracking().AnyAsync(x => x.Id == userId, cancellationToken);
                })
                .WithMessage("There is no user with the ID specified in the UserId property of the RefreshToken entity");
        });
    }
}
