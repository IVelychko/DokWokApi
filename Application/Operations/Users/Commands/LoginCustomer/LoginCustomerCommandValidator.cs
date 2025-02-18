using Domain.Abstractions.Repositories;
using Domain.DTOs.Commands.Users;
using Domain.Shared;
using FluentValidation;

namespace Application.Operations.Users.Commands.LoginCustomer;

public sealed class LoginCustomerCommandValidator : AbstractValidator<LoginCustomerCommand>
{
    private readonly IUserRepository _userRepository;
    private Domain.Entities.User? _userToLogin;

    public LoginCustomerCommandValidator(IUserRepository userRepository)
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
            .Must(IsCustomerOrAdmin)
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

    private bool IsCustomerOrAdmin(string userName)
    {
        var isCustomerOrAdmin = _userToLogin!.UserRole?.Name == UserRoles.Admin || 
                                _userToLogin.UserRole?.Name == UserRoles.Customer;
        return isCustomerOrAdmin;
    }
    
    private bool IsPasswordCorrect(string password)
    {
        return _userRepository.CheckUserPassword(_userToLogin!, password);
    }
}
