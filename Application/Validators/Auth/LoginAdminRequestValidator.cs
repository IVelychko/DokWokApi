using Application.Constants;
using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Constants;
using Domain.DTOs.Requests.Users;
using Domain.Shared;
using Domain.Specifications.Users;
using FluentValidation;

namespace Application.Validators.Auth;

public sealed class LoginAdminRequestValidator : AbstractValidator<LoginAdminRequest>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public LoginAdminRequestValidator(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.UserName)
            .NotEmpty()
            .Matches(RegularExpressions.UserName)
            .MinimumLength(5)
            .MustAsync(UserExists)
            .WithErrorCode(ErrorCodes.EntityNotFound)
            .WithMessage("The credentials are invalid.")
            .DependentRules(() =>
            {
                RuleFor(x => x.Password)
                    .NotEmpty()
                    .Matches(RegularExpressions.Password)
                    .MinimumLength(6)
                    .MustAsync(IsPasswordCorrect)
                    .WithMessage("The credentials are invalid");
            });
    }

    private async Task<bool> UserExists(string userName, CancellationToken cancellationToken)
    {
        UserSpecification specification = new()
        {
            UserName = userName,
            IncludeRole = true,
            NoTracking = true
        };
        var user = await _userRepository.GetBySpecificationAsync(specification);
        return user is not null && user.UserRole!.Name == UserRoles.Admin;
    }
    
    private async Task<bool> IsPasswordCorrect(LoginAdminRequest request, string password, CancellationToken token)
    {
        UserSpecification specification = new()
        {
            UserName = request.UserName,
            NoTracking = true
        };
        var user = await _userRepository.GetBySpecificationAsync(specification);
        user = Ensure.EntityExists(user, "User not found");
        return _passwordHasher.Verify(password, user.PasswordHash);
    }
}
