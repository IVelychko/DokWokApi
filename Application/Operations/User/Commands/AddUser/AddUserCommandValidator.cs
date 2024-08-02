using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.User.Commands.AddUser;

public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
{
    public AddUserCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().Matches(RegularExpressions.FirstName);

        RuleFor(x => x.UserName).NotEmpty().Matches(RegularExpressions.UserName);

        RuleFor(x => x.Email).NotEmpty().EmailAddress();

        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(RegularExpressions.PhoneNumber);

        RuleFor(x => x.Password).NotEmpty().Matches(RegularExpressions.Password);
    }
}
