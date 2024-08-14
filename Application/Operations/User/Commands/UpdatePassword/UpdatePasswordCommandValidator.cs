using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.User.Commands.UpdatePassword;

public sealed class UpdatePasswordCommandValidator : AbstractValidator<UpdatePasswordCommand>
{
    public UpdatePasswordCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().Matches(RegularExpressions.Guid);

        RuleFor(x => x.OldPassword).NotEmpty().Matches(RegularExpressions.Password).MinimumLength(6);

        RuleFor(x => x.NewPassword).NotEmpty().Matches(RegularExpressions.Password).MinimumLength(6);
    }
}
