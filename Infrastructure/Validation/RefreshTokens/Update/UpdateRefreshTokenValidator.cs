using Domain.Entities;
using Domain.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Validation.RefreshTokens.Update;

public sealed class UpdateRefreshTokenValidator : AbstractValidator<UpdateRefreshTokenValidationModel>
{
    private readonly StoreDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdateRefreshTokenValidator(UserManager<ApplicationUser> userManager, StoreDbContext context)
    {
        _context = context;
        _userManager = userManager;

        RuleFor(x => x)
            .NotNull()
            .WithMessage("The passed refresh token is null")
            .MustAsync(RefreshTokenToUpdateExists)
            .WithState(_ => new ValidationFailureState { IsNotFound = true })
            .WithMessage("There is no refresh token with this ID in the database")
            .DependentRules(() =>
            {
                RuleFor(x => x.UserId)
                    .MustAsync(UserExists)
                    .WithMessage("There is no user with the ID specified in the UserId property of the RefreshToken entity");
            });
    }

    private async Task<bool> RefreshTokenToUpdateExists(UpdateRefreshTokenValidationModel refreshToken, CancellationToken cancellationToken)
    {
        return await _context.RefreshTokens.AsNoTracking().AnyAsync(x => x.Id == refreshToken.Id, cancellationToken);
    }

    private async Task<bool> UserExists(string userId, CancellationToken cancellationToken)
    {
        return await _userManager.Users.AsNoTracking().AnyAsync(x => x.Id == userId, cancellationToken);
    }
}
