using Domain.Abstractions.Repositories;
using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.User.Commands.LoginAdmin;

public sealed class LoginAdminCommandValidator : AbstractValidator<LoginAdminCommand>
{
    private readonly IUserRepository _userRepository;

    public LoginAdminCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.UserName)
            .NotEmpty()
            .Matches(RegularExpressions.UserName)
            .MinimumLength(5)
            .MustAsync(UserExists)
            .WithName("user")
            .WithErrorCode("404")
            .WithMessage("The credentials are wrong.")
            .MustAsync(IsAdmin)
            .WithName("user")
            .WithMessage("The authorization is denied")
            .DependentRules(() =>
            {
                RuleFor(x => x.Password)
                    .NotEmpty()
                    .Matches(RegularExpressions.Password)
                    .MinimumLength(6)
                    .MustAsync(IsValidPassword)
                    .WithName("user")
                    .WithMessage("The credentials are wrong");
            });
    }

    private async Task<bool> UserExists(string userName, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByUserNameAsync(userName);
        return user is not null;
    }

    private async Task<bool> IsAdmin(string userName, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByUserNameAsync(userName);
        if (user is null)
        {
            return false;
        }

        var result = await _userRepository.IsInRoleAsync(user, UserRoles.Admin);
        return result.Match(x => x, e => false);
    }

    private async Task<bool> IsValidPassword(LoginAdminCommand adminLogin, string password, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByUserNameAsync(adminLogin.UserName);
        if (user is null)
        {
            return false;
        }

        return await _userRepository.CheckUserPasswordAsync(user, password);
    }
}
