using FluentValidation;
using System.IdentityModel.Tokens.Jwt;

namespace Domain.Validation.Users.ExpiredJwt;

public sealed class ExpiredJwtValidator : AbstractValidator<ExpiredJwtValidationModel>
{
    public ExpiredJwtValidator()
    {
        RuleFor(x => x.IsAlgorithmValid)
            .Must(x => x)
            .WithMessage("The token is not valid");

        RuleFor(x => x.Jwt)
            .NotNull()
            .WithMessage("The passed JWT is null")
            .DependentRules(() =>
            {
                RuleFor(x => x.Jwt)
                    .Must(IsTokenExpired)
                    .WithMessage("The token has not expired yet");
            });
    }

    private static bool IsTokenExpired(JwtSecurityToken expiredJwt)
    {
        return expiredJwt.ValidTo < DateTime.UtcNow;
    }
}
