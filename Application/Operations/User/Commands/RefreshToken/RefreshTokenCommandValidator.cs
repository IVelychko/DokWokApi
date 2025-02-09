using Domain.DTOs.Commands.Users;
using Domain.Shared;
using FluentValidation;

namespace Application.Operations.User.Commands.RefreshToken;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty();

        RuleFor(x => x.RefreshToken)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Matches(RegularExpressions.Guid);
    }
}
