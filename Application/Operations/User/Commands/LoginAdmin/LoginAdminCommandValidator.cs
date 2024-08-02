using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.User.Commands.LoginAdmin;

public sealed class LoginAdminCommandValidator : AbstractValidator<LoginAdminCommand>
{
    public LoginAdminCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().Matches(RegularExpressions.UserName);

        RuleFor(x => x.Password).NotEmpty().Matches(RegularExpressions.Password);
    }
}
