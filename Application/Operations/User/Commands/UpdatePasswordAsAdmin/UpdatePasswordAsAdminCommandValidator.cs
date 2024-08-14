using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.User.Commands.UpdatePasswordAsAdmin;

public sealed class UpdatePasswordAsAdminCommandValidator : AbstractValidator<UpdatePasswordAsAdminCommand>
{
    public UpdatePasswordAsAdminCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().Matches(RegularExpressions.Guid);

        RuleFor(x => x.NewPassword).NotEmpty().Matches(RegularExpressions.Password).MinimumLength(6);
    }
}
