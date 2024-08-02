using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.User.Commands.LoginCustomer;

public sealed class LoginCustomerCommandValidator : AbstractValidator<LoginCustomerCommand>
{
    public LoginCustomerCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().Matches(RegularExpressions.UserName);

        RuleFor(x => x.Password).NotEmpty().Matches(RegularExpressions.Password);
    }
}
