using Application.Constants;
using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Constants;
using Domain.DTOs.Requests.Users;
using Domain.Shared;
using Domain.Specifications.Users;
using FluentValidation;

namespace Application.Validators.Users;

public sealed class UpdatePasswordRequestValidator : AbstractValidator<UpdatePasswordRequest>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UpdatePasswordRequestValidator(
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _passwordHasher = passwordHasher;

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.UserId)
            .NotEmpty()
            .MustAsync(UserToUpdateExists)
            .WithErrorCode(ErrorCodes.EntityNotFound)
            .WithMessage("There is no user with this ID in the database")
            .DependentRules(() =>
            {
                RuleFor(x => x.OldPassword)
                    .NotEmpty()
                    .Matches(RegularExpressions.Password)
                    .MinimumLength(6)
                    .MustAsync(IsOldPasswordCorrect)
                    .WithMessage("The credentials are invalid");

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

    private async Task<bool> IsOldPasswordCorrect(
        UpdatePasswordRequest request, string oldPassword, CancellationToken token)
    {
        UserSpecification specification = new()
        {
            Id = request.UserId,
            NoTracking = true
        };
        var user = await _userRepository.GetBySpecificationAsync(specification);
        user = Ensure.EntityExists(user, "User not found");
        return _passwordHasher.Verify(oldPassword, user.PasswordHash);
    }
}
