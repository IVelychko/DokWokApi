using Domain.Abstractions.Repositories;
using Domain.DTOs.Commands.Users;
using Domain.Entities;
using Domain.Shared;
using FluentValidation;

namespace Application.Operations.Users.Commands.LoginAdmin;

public sealed class LoginAdminCommandValidator : AbstractValidator<LoginAdminCommand>
{
    private readonly IUserRepository _userRepository;
    private User? _userToLogin;

    public LoginAdminCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.UserName)
            .NotEmpty()
            .Matches(RegularExpressions.UserName)
            .MinimumLength(5)
            .MustAsync(UserExists)
            .WithErrorCode("404")
            .WithMessage("The credentials are wrong.")
            .Must(IsAdmin)
            .WithMessage("The authorization is denied")
            .DependentRules(() =>
            {
                RuleFor(x => x.Password)
                    .NotEmpty()
                    .Matches(RegularExpressions.Password)
                    .MinimumLength(6)
                    .Must(IsPasswordCorrect)
                    .WithMessage("The credentials are invalid");
            });
    }

    private async Task<bool> UserExists(string userName, CancellationToken cancellationToken)
    {
        _userToLogin = await _userRepository.GetUserByUserNameWithDetailsAsNoTrackingAsync(userName);
        return _userToLogin is not null;
    }

    private bool IsAdmin(string userName)
    {
        return _userToLogin!.UserRole?.Name == UserRoles.Admin;
    }
    
    private bool IsPasswordCorrect(string password)
    {
        return _userRepository.CheckUserPassword(_userToLogin!, password);
    }
}
