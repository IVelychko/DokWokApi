using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.User.Commands.RegisterUser;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().Matches(RegularExpressions.FirstName).MinimumLength(2);

        RuleFor(x => x.UserName).NotEmpty().Matches(RegularExpressions.UserName).MinimumLength(5);

        RuleFor(x => x.Email).NotEmpty().EmailAddress();

        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(RegularExpressions.PhoneNumber).MinimumLength(9);

        RuleFor(x => x.Password).NotEmpty().Matches(RegularExpressions.Password).MinimumLength(6);
    }
}
