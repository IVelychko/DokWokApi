using FluentValidation;

namespace Domain.Validation.Users.RefreshToken;

public sealed class RefreshTokenValidator : AbstractValidator<RefreshTokenValidationModel>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken).NotNull().WithMessage("The passed refresh token is null");

        RuleFor(x => x.JwtId).NotEmpty().WithMessage("The passed Jwt Id is null or empty");

        RuleFor(x => x)
            .Must(x => DateTime.UtcNow < x.RefreshToken.ExpiryDate)
            .WithName("refreshToken")
            .WithMessage("The refresh token has expired")
            .Must(x => !x.RefreshToken.Invalidated)
            .WithName("refreshToken")
            .WithMessage("The refresh token has been invalidated")
            .Must(x => !x.RefreshToken.Used)
            .WithName("refreshToken")
            .WithMessage("The refresh token has been used")
            .Must(x => x.RefreshToken.JwtId == x.JwtId)
            .WithName("refreshToken")
            .WithMessage("The refresh token does not match the JWT")
            .When(x => x.RefreshToken is not null && !string.IsNullOrEmpty(x.JwtId));
    }
}
