using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.User.Commands.RegisterUser;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().Matches(RegularExpressions.FirstName);

        RuleFor(x => x.UserName).NotEmpty().Matches(RegularExpressions.UserName);

        RuleFor(x => x.Email).NotEmpty().EmailAddress();

        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(RegularExpressions.PhoneNumber);

        RuleFor(x => x.Password).NotEmpty().Matches(RegularExpressions.Password);
    }
}
