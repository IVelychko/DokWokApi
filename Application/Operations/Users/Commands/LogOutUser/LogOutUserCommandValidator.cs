using Domain.DTOs.Commands.Users;
using FluentValidation;

namespace Application.Operations.Users.Commands.LogOutUser;

public sealed class LogOutUserCommandValidator : AbstractValidator<LogOutUserCommand>
{
    // TODO: Add refresh token validation when doing LogOut operation
    public LogOutUserCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}
