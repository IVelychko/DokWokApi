using Domain.DTOs.Requests.Users;
using FluentValidation;

namespace Application.Validators.Auth;

public sealed class LogOutUserRequestValidator : AbstractValidator<LogOutUserRequest>
{
    // TODO: Add refresh token validation when doing LogOut operation
    public LogOutUserRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}
