using Application.Constants;
using Domain.Abstractions.Repositories;
using Domain.Constants;
using Domain.DTOs.Requests.Users;
using Domain.Shared;
using FluentValidation;

namespace Application.Validators.Users;

public sealed class UpdatePasswordAsAdminRequestValidator : AbstractValidator<UpdatePasswordAsAdminRequest>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    public UpdatePasswordAsAdminRequestValidator(
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.UserId)
            .NotEmpty()
            .MustAsync(UserToUpdateExists)
            .WithErrorCode(ErrorCodes.EntityNotFound)
            .WithMessage("There is no user with this ID in the database")
            .DependentRules(() =>
            {
                RuleFor(x => x.NewPassword)
                    .NotEmpty()
                    .Matches(RegularExpressions.Password)
                    .MinimumLength(6);
            });
    }

    private async Task<bool> UserToUpdateExists(long userId, CancellationToken cancellationToken)
    {
        var adminRole = await _userRoleRepository.GetByNameAsync(UserRoles.Admin);
        adminRole = Ensure.EntityExists(adminRole, "Admin role not found");
        return await _userRepository.UserExistsAsync(userId, adminRole.Id);
    }
}
