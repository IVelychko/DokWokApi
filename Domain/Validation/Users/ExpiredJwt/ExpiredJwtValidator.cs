using FluentValidation;

namespace Domain.Validation.Users.ExpiredJwt;

public sealed class ExpiredJwtValidator : AbstractValidator<ExpiredJwtValidationModel>
{
    public ExpiredJwtValidator()
    {
        RuleFor(x => x)
            .NotNull()
            .WithMessage("The passed expired jwt object is null")
            .Must(x => x.IsAlgorithmValid)
            .WithMessage("The token is not valid")
            .Must(IsTokenExpired)
            .WithMessage("The token has not expired yet");
    }

    private bool IsTokenExpired(ExpiredJwtValidationModel expiredJwt)
    {
        return expiredJwt.Jwt.ValidTo < DateTime.UtcNow;
    }
}
