using Domain.Abstractions.Repositories;
using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.User.Commands.UpdatePasswordAsAdmin;

public sealed class UpdatePasswordAsAdminCommandValidator : AbstractValidator<UpdatePasswordAsAdminCommand>
{
    private readonly IUserRepository _userRepository;

    public UpdatePasswordAsAdminCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.UserId)
            .NotEmpty()
            .Matches(RegularExpressions.Guid)
            .MustAsync(UserToUpdateExists)
            .WithErrorCode("404")
            .WithMessage("There is no user with this ID in the database")
            .MustAsync(IsNotAdmin)
            .WithName("user")
            .WithMessage("Forbidden action")
            .DependentRules(() =>
            {
                RuleFor(x => x.NewPassword)
                    .NotEmpty()
                    .Matches(RegularExpressions.Password)
                    .MinimumLength(6);
            });
    }

    private async Task<bool> UserToUpdateExists(string userId, CancellationToken cancellationToken)
    {
        return (await _userRepository.GetUserByIdAsync(userId)) is not null;
    }

    private async Task<bool> IsNotAdmin(string userId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null)
        {
            return false;
        }

        var result = await _userRepository.IsInRoleAsync(user, UserRoles.Admin);
        return result.Match(inRole => !inRole, error => false);
    }
}
