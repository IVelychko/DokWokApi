using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.User.Commands.UpdateUser;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().Matches(RegularExpressions.Guid);

        RuleFor(x => x.FirstName).NotEmpty().Matches(RegularExpressions.FirstName).MinimumLength(2);

        RuleFor(x => x.UserName).NotEmpty().Matches(RegularExpressions.UserName).MinimumLength(5);

        RuleFor(x => x.Email).NotEmpty().EmailAddress();

        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(RegularExpressions.PhoneNumber).MinimumLength(9);
    }
}
