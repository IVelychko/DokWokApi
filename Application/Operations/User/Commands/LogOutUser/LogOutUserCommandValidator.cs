using Domain.DTOs.Commands.Users;
using FluentValidation;

namespace Application.Operations.User.Commands.LogOutUser;

public sealed class LogOutUserCommandValidator : AbstractValidator<LogOutUserCommand>
{
    public LogOutUserCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}
